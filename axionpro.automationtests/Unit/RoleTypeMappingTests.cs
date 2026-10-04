// ================================================================
// Purpose : Locks Tenant role-type display mapping across Role and
//           NewLogin response contracts.
// ================================================================

using AutoMapper;
using System.Reflection;
using axionpro.api.Controllers.Role;
using axionpro.application.Constants;
using axionpro.application.DTOS.Role;
using axionpro.application.Features.RoleCmd;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Mappings;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("RoleTypeMapping")]
public sealed class RoleTypeMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void SetUpMapper()
    {
        _mapper = new MapperConfiguration(configuration =>
            configuration.AddProfile<MappingProfile>()).CreateMapper();
    }

    [TestCase(1, "Tenant Administrator")]
    [TestCase(2, "Workforce User")]
    [TestCase(3, "People Manager")]
    [TestCase(4, "External User")]
    [TestCase(99, "Unknown")]
    public void Role_response_mappings_return_the_central_role_type_display_name(
        int roleType,
        string expectedDisplayName)
    {
        var role = new Role
        {
            Id = 10,
            RoleName = "Test Role",
            RoleType = roleType,
            IsActive = true
        };

        var roleResponse = _mapper.Map<GetRoleResponseDTO>(role);
        var loginResponse = _mapper.Map<NewLoginRoleDTO>(role);

        Assert.Multiple(() =>
        {
            Assert.That(
                ConstantValues.GetRoleTypeDisplayName(roleType),
                Is.EqualTo(expectedDisplayName));
            Assert.That(roleResponse.RoleType, Is.EqualTo(roleType));
            Assert.That(roleResponse.RoleTypeName, Is.EqualTo(expectedDisplayName));
            Assert.That(loginResponse.RoleTypeId, Is.EqualTo(roleType));
            Assert.That(loginResponse.RoleTypeName, Is.EqualTo(expectedDisplayName));
        });
    }

    [Test]
    public async Task Role_type_options_publish_all_centralized_values_in_stable_order()
    {
        var handler = new GetRoleTypeOptionsQueryHandler();

        var response = await handler.Handle(
            new GetRoleTypeOptionsQuery(),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSucceeded, Is.True);
            Assert.That(response.Data.Select(option => option.Id), Is.EqualTo(new[] { 1, 2, 3, 4 }));
            Assert.That(response.Data.Select(option => option.Name),
                Is.EqualTo(new[]
                {
                    "Tenant Administrator",
                    "Workforce User",
                    "People Manager",
                    "External User"
                }));
            Assert.That(
                response.Data.Select(option => option.Description),
                Is.EqualTo(new[]
                {
                    "Full company workspace administration, including users, roles, settings, and tenant-level configuration.",
                    "Employee self-service access to personal information, attendance, leave, documents, and assigned work features.",
                    "Team management access for supervisors and managers, subject to assigned role permissions.",
                    "Limited portal access for clients, consultants, vendors, and other external users, subject to assigned role permissions."
                }));
            Assert.That(
                response.Data.Single(option => option.Id == 4).Name,
                Is.EqualTo("External User"));
        });
    }

    [Test]
    public async Task Role_type_options_bypass_module_permission_lookup_and_reach_handler()
    {
        var unitOfWork = Proxy<IUnitOfWork>((method, _) =>
            throw new AssertionException($"Permission persistence must not be called: {method.Name}"));
        var commonRequestService = Proxy<ICommonRequestService>((method, _) =>
            throw new AssertionException($"Common request service must not be called: {method.Name}"));
        var behavior = new RolePermissionBehavior<
            GetRoleTypeOptionsQuery,
            ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>>(
                unitOfWork,
                commonRequestService,
                NullLogger<RolePermissionBehavior<
                    GetRoleTypeOptionsQuery,
                    ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>>>.Instance);
        var nextCalled = false;

        Task<ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>.Success([]));
        }

        var response = await behavior.Handle(
            new GetRoleTypeOptionsQuery(),
            Next,
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(nextCalled, Is.True);
            Assert.That(response.IsSucceeded, Is.True);
        });
    }

    [Test]
    public void Role_type_options_endpoint_is_authenticated_and_has_no_permission_inputs()
    {
        var method = typeof(RoleController).GetMethod(
            nameof(RoleController.GetRoleTypeOptions),
            BindingFlags.Instance | BindingFlags.Public);
        var route = method?.GetCustomAttribute<HttpMethodAttribute>();

        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null);
            Assert.That(method?.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null);
            Assert.That(route?.HttpMethods, Is.EqualTo(new[] { "GET" }));
            Assert.That(route?.Template, Is.EqualTo("type-options"));
            Assert.That(method?.GetParameters().Select(parameter => parameter.ParameterType),
                Is.EqualTo(new[] { typeof(CancellationToken) }));
        });
    }

    [Test]
    public void Existing_role_queries_still_require_module_and_operation_permissions()
    {
        var unitOfWork = Proxy<IUnitOfWork>((method, _) =>
            throw new AssertionException($"Permission persistence must not run for invalid input: {method.Name}"));
        var commonRequestService = Proxy<ICommonRequestService>((method, _) =>
            throw new AssertionException($"Common request service must not run for invalid input: {method.Name}"));
        var behavior = new RolePermissionBehavior<
            GetRoleOptionQuery,
            ApiResponse<List<GetRoleOptionResponseDTO>>>(
                unitOfWork,
                commonRequestService,
                NullLogger<RolePermissionBehavior<
                    GetRoleOptionQuery,
                    ApiResponse<List<GetRoleOptionResponseDTO>>>>.Instance);
        var nextCalled = false;

        Task<ApiResponse<List<GetRoleOptionResponseDTO>>> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(ApiResponse<List<GetRoleOptionResponseDTO>>.Success([]));
        }

        Assert.ThrowsAsync<ValidationErrorException>(async () => await behavior.Handle(
            new GetRoleOptionQuery(new GetRoleOptionRequestDTO()),
            Next,
            CancellationToken.None));
        Assert.That(nextCalled, Is.False);
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke)
        where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).InvokeHandler = invoke;
        return proxy;
    }

    private class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> InvokeHandler { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return InvokeHandler(targetMethod!, args);
        }
    }
}
