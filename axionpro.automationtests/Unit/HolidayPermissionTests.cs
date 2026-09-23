using System.Reflection;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOs.Holiday;
using axionpro.application.Exceptions;
using axionpro.application.Features.HolidayCmd;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("HolidayPermission")]
public sealed class HolidayPermissionTests
{
    [TestCase("TENANT_POLICY_HOLIDAY", "Add", 1, true)]
    [TestCase("TENANT_POLICY_HOLIDAY", "View", 1, false)]
    [TestCase("TENANT_POLICY_TYPES", "Add", 1, false)]
    [TestCase("TENANT_POLICY_HOLIDAY", "Add", 0, false)]
    public async Task Create_reaches_handler_only_with_matching_module_operation_and_grant(
        string moduleCode,
        string operationName,
        int permissionResult,
        bool allowed)
    {
        var common = Proxy<ICommonRequestService>((method, _) => method.Name switch
        {
            "GetModuleCodeAsync" => Task.FromResult<string?>(moduleCode),
            "ValidateTenantUserRequestAsync" => Task.FromResult(new CommonDecodedResult
            {
                Success = true,
                TenantId = 8,
                LoggedInEmployeeId = 4,
                RoleId = 2
            }),
            _ => throw new AssertionException($"Unexpected common request: {method.Name}")
        });
        var operation = Proxy<IOperationRepository>((_, _) => Task.FromResult<Operation?>(new Operation
        {
            OperationName = operationName
        }));
        var stored = Proxy<IStoreProcedureRepository>((_, _) => Task.FromResult(
            new TenantsUserPermissionCheckResponseDTO { ResultCode = permissionResult }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_OperationRepository" => operation,
            "get_StoreProcedureRepository" => stored,
            _ => throw new AssertionException($"Unexpected unit request: {method.Name}")
        });

        var behavior = new HolidayPermissionBehavior<CreateHolidayCommand, bool>(unit, common);
        var invoked = false;
        Task<bool> Next(CancellationToken _)
        {
            invoked = true;
            return Task.FromResult(true);
        }

        var command = new CreateHolidayCommand(new SaveHolidayRequestDTO
        {
            ModuleId = 118,
            OperationId = 1
        });

        if (allowed)
        {
            Assert.That(await behavior.Handle(command, Next, CancellationToken.None), Is.True);
        }
        else
        {
            Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await behavior.Handle(command, Next, CancellationToken.None));
        }

        Assert.That(invoked, Is.EqualTo(allowed));
    }

    [TestCase("Import", true)]
    [TestCase("View", false)]
    public async Task Import_requires_import_operation(string operationName, bool allowed)
    {
        var common = Proxy<ICommonRequestService>((method, _) => method.Name switch
        {
            "GetModuleCodeAsync" => Task.FromResult<string?>("TENANT_POLICY_HOLIDAY"),
            "ValidateTenantUserRequestAsync" => Task.FromResult(new CommonDecodedResult
            {
                Success = true, TenantId = 8, LoggedInEmployeeId = 4, RoleId = 2
            }),
            _ => throw new AssertionException(method.Name)
        });
        var operation = Proxy<IOperationRepository>((_, _) => Task.FromResult<Operation?>(new Operation
        {
            OperationName = operationName
        }));
        var stored = Proxy<IStoreProcedureRepository>((_, _) => Task.FromResult(
            new TenantsUserPermissionCheckResponseDTO { ResultCode = 1 }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_OperationRepository" => operation,
            "get_StoreProcedureRepository" => stored,
            _ => throw new AssertionException(method.Name)
        });
        var behavior = new HolidayPermissionBehavior<ImportHolidaysCommand, bool>(unit, common);
        var invoked = false;
        Task<bool> Next(CancellationToken _)
        {
            invoked = true;
            return Task.FromResult(true);
        }

        var command = new ImportHolidaysCommand(new ImportHolidayRequestDTO
        {
            ModuleId = 118, OperationId = 12
        });
        if (allowed)
        {
            Assert.That(await behavior.Handle(command, Next, CancellationToken.None), Is.True);
        }
        else
        {
            Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await behavior.Handle(command, Next, CancellationToken.None));
        }

        Assert.That(invoked, Is.EqualTo(allowed));
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }
}
