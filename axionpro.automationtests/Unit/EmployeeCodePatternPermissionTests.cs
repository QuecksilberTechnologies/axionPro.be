using System.Reflection;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd;
using axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeBulkCode")]
public sealed class EmployeeCodePatternPermissionTests
{
    [TestCase(LoginUserType.Host, "HOST_TENANT_LOCATION_LIST", true)]
    [TestCase(LoginUserType.Host, "TENANT_LOCATIONS", false)]
    [TestCase(LoginUserType.TenantEmployee, "TENANT_LOCATIONS", true)]
    [TestCase(LoginUserType.TenantEmployee, "HOST_TENANT_LOCATION_LIST", false)]
    public async Task Location_module_binding_separates_host_and_tenant(
        LoginUserType userType, string moduleCode, bool allowed)
    {
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult<string?>(moduleCode));
        var unit = Proxy<IUnitOfWork>((_, _) => throw new AssertionException("Binding must not query permissions."));
        var behavior = new TenantLocationPermissionBehavior<axionpro.application.Features.TenantConfigurationCmd.Handlers.GetTenantLocationsQuery, bool>(
            unit, common, NullLogger<TenantLocationPermissionBehavior<axionpro.application.Features.TenantConfigurationCmd.Handlers.GetTenantLocationsQuery, bool>>.Instance);
        var method = behavior.GetType().GetMethod("EnsureExpectedModuleCodeAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var task = (Task)method.Invoke(behavior, new object[]
        {
            new axionpro.application.DTOs.BaseDTO.PermissionRequestDTO { ModuleId = 78, OperationId = 4 },
            CancellationToken.None, userType
        })!;
        if (allowed)
        {
            await task;
        }
        else
        {
            Assert.ThrowsAsync<ForbiddenAccessException>(async () => await task);
        }
    }

    [Test]
    public void Tenant_permission_denial_is_forbidden_while_invalid_permission_context_is_unauthorized()
    {
        Assert.Throws<ForbiddenAccessException>(() => TenantRuntimePermissionValidator.EnsureAllowed(
            new TenantsUserPermissionCheckResponseDTO { ResultCode = 0 }));
        Assert.Throws<UnauthorizedAccessException>(() => TenantRuntimePermissionValidator.EnsureAllowed(
            new TenantsUserPermissionCheckResponseDTO { ResultCode = -1 }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Host_aggregate_cannot_bypass_pattern_preview(bool change)
    {
        var pattern = new axionpro.domain.Entity.EmployeeCodePattern
            { Prefix = "EMP", Separator = "-", RunningNumberLength = 4 };
        var dto = new axionpro.application.DTOs.Tenant.NewTenantEmployeeCodePatternUpdateRequestDTO
            { Prefix = change ? "NEW" : "EMP" };
        var method = typeof(axionpro.application.Features.TenantManagementCmd.Commands.UpdateNewTenantCommandHandler)
            .GetMethod("ApplyEmployeeCodePattern", BindingFlags.NonPublic | BindingFlags.Static)!;
        if (change)
        {
            var error = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { dto, pattern }));
            Assert.That(error!.InnerException, Is.TypeOf<ConflictException>());
        }
        else
        {
            Assert.That(method.Invoke(null, new object[] { dto, pattern }), Is.False);
        }
        Assert.That(pattern.Prefix, Is.EqualTo("EMP"));
    }

    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task Existing_pipeline_rejects_wrong_module_or_denied_grant_before_handler(bool correctModule, bool granted)
    {
        var common = Proxy<ICommonRequestService>((method, _) => method.Name switch
        {
            "ValidateAuthenticatedRequestAsync" => Task.FromResult(new AuthenticatedRequestContext { UserType = LoginUserType.TenantEmployee }),
            "ValidateTenantUserRequestAsync" => Task.FromResult(new CommonDecodedResult { Success = true, TenantId = 8, LoggedInEmployeeId = 1, RoleId = 22 }),
            "GetModuleCodeAsync" => Task.FromResult<string?>(correctModule ? "TENANT_EMPLOYEE_CODE" : "TENANT_LOCATIONS"),
            _ => throw new InvalidOperationException(method.Name)
        });
        var stored = Proxy<IStoreProcedureRepository>((_, _) => Task.FromResult(new TenantsUserPermissionCheckResponseDTO { ResultCode = granted ? 1 : 0 }));
        var unit = Proxy<IUnitOfWork>((_, _) => stored);
        var behavior = new TenantLocationPermissionBehavior<UpdateEmployeeCodePatternCommand, bool>(
            unit, common, NullLogger<TenantLocationPermissionBehavior<UpdateEmployeeCodePatternCommand, bool>>.Instance);
        var invoked = false;
        Task<bool> Next(CancellationToken _)
        {
            invoked = true;
            return Task.FromResult(true);
        }

        var command = new UpdateEmployeeCodePatternCommand(new SaveEmployeeCodePatternRequestDTO { ModuleId = 29, OperationId = 2 });
        if (correctModule && granted)
        {
            Assert.That(await behavior.Handle(command, Next, CancellationToken.None), Is.True);
        }
        else
        {
            Assert.ThrowsAsync<ForbiddenAccessException>(async () => await behavior.Handle(command, Next, CancellationToken.None));
        }
        Assert.That(invoked, Is.EqualTo(correctModule && granted));
    }

    [TestCase("AddEmployeeCodePattern", "add-employee-code-pattern", typeof(HttpPostAttribute))]
    [TestCase("UpdateEmployeeCodePattern", "update-employee-code-pattern", typeof(HttpPutAttribute))]
    public void Pattern_routes_require_authentication(string method, string route, Type verb)
    {
        var info = typeof(axionpro.api.Controllers.Tenant.TenantController).GetMethod(method)!;
        Assert.That(info.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null);
        var attribute = info.GetCustomAttributes().Single(item => item.GetType() == verb);
        Assert.That(((Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute)attribute).Template, Is.EqualTo(route));
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }

    [Test]
    public void Host_cannot_use_tenant_pattern_write()
    {
        var common = Proxy<ICommonRequestService>((_, _) =>
            Task.FromResult(new AuthenticatedRequestContext { UserType = LoginUserType.Host }));
        var unit = Proxy<IUnitOfWork>((_, _) => throw new AssertionException("Tenant grants must not be queried for Host."));
        var behavior = new TenantLocationPermissionBehavior<UpdateEmployeeCodePatternCommand, bool>(
            unit, common, NullLogger<TenantLocationPermissionBehavior<UpdateEmployeeCodePatternCommand, bool>>.Instance);
        Assert.ThrowsAsync<ForbiddenAccessException>(async () => await behavior.Handle(
            new UpdateEmployeeCodePatternCommand(new SaveEmployeeCodePatternRequestDTO()),
            _ => throw new AssertionException("Host must not reach the tenant write handler."), CancellationToken.None));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void View_permission_cannot_create_or_update_pattern(bool create)
    {
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(
            new CommonDecodedResult { Success = true, TenantId = 8, LoggedInEmployeeId = 1, RoleId = 22 }));
        var operations = Proxy<IOperationRepository>((_, _) => Task.FromResult<axionpro.domain.Entity.Operation?>(
            new axionpro.domain.Entity.Operation { IsActive = true, OperationType = (int)OperationType.View }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_OperationRepository"
            ? operations : throw new AssertionException("View permission must not reach persistence."));
        var handler = new SaveEmployeeCodePatternCommandHandler(unit, common);
        var dto = new SaveEmployeeCodePatternRequestDTO { OperationId = 1 };
        Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
        {
            if (create)
            {
                await handler.Handle(new CreateEmployeeCodePatternCommand(dto), CancellationToken.None);
            }
            else
            {
                await handler.Handle(new UpdateEmployeeCodePatternCommand(dto), CancellationToken.None);
            }
        });
    }
}
