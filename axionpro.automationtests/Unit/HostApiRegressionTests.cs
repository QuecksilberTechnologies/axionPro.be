// ================================================================
// Purpose : Regression coverage for authenticated Host administration APIs,
//           the Tenant onboarding verification state, and the deliberate
//           Host/Tenant device-configuration ownership boundary.
// ================================================================

using System.Reflection;
using axionpro.api.Controllers.DefaultEmailConfig;
using axionpro.api.Controllers.EmailTemplate;
using axionpro.api.Controllers.Host;
using axionpro.api.Controllers.HostDevice;
using axionpro.api.Controllers.Tenant;
using axionpro.api.Controllers.TenantEmailConfig;
using axionpro.api.Middlewares;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Features.HostDeviceCmd;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using axionpro.application.Features.TenantManagementCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>
/// Covers the Host pages shown in the administration UI. These tests use the
/// existing controller, behavior, mapping, and constant contracts; they do
/// not require a browser, device, broker, or live database.
/// </summary>
[TestFixture]
[Category("HostApi")]
public sealed class HostApiRegressionTests
{
    private static readonly Type[] HostControllerTypes =
    [
        typeof(HostController),
        typeof(HostAccessController),
        typeof(HostRolePermissionController),
        typeof(DeviceMasterController),
        typeof(TenantDeviceController),
        typeof(TenantDeviceConfigurationController),
        typeof(TenantCardMasterController),
        typeof(DeviceCommandController),
        typeof(DeviceDdlOptionsController),
        typeof(DefaultEmailConfigController),
        typeof(TenantEmailConfigController),
        typeof(EmailTemplateController),
        typeof(TenantController)
    ];

    private static readonly HashSet<string> ExplicitlyAnonymousTenantActions =
    [
        nameof(TenantController.TenantCreation),
        nameof(TenantController.VerifyEmail)
    ];

    [Test]
    public void Every_host_administration_http_action_has_a_route_and_requires_an_authenticated_session()
    {
        var failures = new List<string>();

        foreach (var controllerType in HostControllerTypes)
        {
            var controllerIsAuthorized = controllerType.IsDefined(typeof(AuthorizeAttribute), inherit: true);
            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>(inherit: true)?.Template;
            if (string.IsNullOrWhiteSpace(controllerRoute))
            {
                failures.Add($"{controllerType.Name} is missing its controller route.");
            }

            foreach (var action in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var verbs = action.GetCustomAttributes<HttpMethodAttribute>(inherit: true).ToArray();
                if (verbs.Length == 0)
                {
                    continue;
                }

                var isExplicitlyAnonymous =
                    controllerType == typeof(TenantController) &&
                    ExplicitlyAnonymousTenantActions.Contains(action.Name) &&
                    action.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
                var actionIsAuthorized = action.IsDefined(typeof(AuthorizeAttribute), inherit: true);

                if (!isExplicitlyAnonymous && !controllerIsAuthorized && !actionIsAuthorized)
                {
                    failures.Add($"{controllerType.Name}.{action.Name} must require [Authorize].");
                }

                if (verbs.Any(verb => string.IsNullOrWhiteSpace(verb.Template)))
                {
                    failures.Add($"{controllerType.Name}.{action.Name} is missing an HTTP route template.");
                }
            }
        }

        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    [Test]
    public void Only_the_two_token_based_tenant_onboarding_actions_remain_anonymous()
    {
        var anonymousActions = typeof(TenantController)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(inherit: true).Any())
            .Where(method => method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
            .Select(method => method.Name)
            .ToArray();

        Assert.That(anonymousActions, Is.EquivalentTo(ExplicitlyAnonymousTenantActions));
    }

    [Test]
    public void Verified_legacy_onboarding_credential_is_not_eligible_for_another_resend_email()
    {
        var tenant = CreateTenant(isVerified: false, credentialIsOnboard: true);
        var handler = new ResendTenantVerificationCommandHandler(
            CreateUnitOfWork(tenant),
            CreateHostCommonRequestService(),
            CreateProxy<ITokenService>((method, _) => throw new AssertionException($"Token generation must not run for {method.Name}.")),
            CreateProxy<IIdEncoderService>((method, args) => method.Name switch
            {
                nameof(IIdEncoderService.DecodeId_long) => 71L,
                _ => throw new AssertionException($"Unexpected identifier call: {method.Name}.")
            }),
            CreateProxy<IEmailService>((method, _) => throw new AssertionException($"Email delivery must not run for {method.Name}.")),
            new ConfigurationBuilder().Build());

        var exception = Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new ResendTenantVerificationCommand("opaque-tenant-id", new PermissionRequestDTO()),
            CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo(AppConstants.ErrorMessages.TenantAlreadyVerified));
    }

    [Test]
    public void Onboarding_credential_lookup_uses_only_the_current_tenant_email_and_active_employee()
    {
        var tenant = CreateTenant(isVerified: false, credentialIsOnboard: false);
        tenant.Employee.Add(new Employee
        {
            Id = 99,
            TenantId = tenant.Id + 1,
            IsSoftDeleted = false,
            LoginCredential =
            [
                new LoginCredential
                {
                    Id = 2,
                    TenantId = tenant.Id + 1,
                    EmployeeId = 99,
                    LoginId = tenant.TenantEmail,
                    IsOnboard = true,
                    IsSoftDeleted = false
                }
            ]
        });

        var credential = TenantOnboardingVerification.FindCredential(tenant);

        Assert.Multiple(() =>
        {
            Assert.That(credential, Is.Not.Null);
            Assert.That(credential!.TenantId, Is.EqualTo(tenant.Id));
            Assert.That(TenantOnboardingVerification.IsVerified(tenant, credential), Is.False);
        });
    }

    [Test]
    public async Task Host_is_deliberately_denied_the_tenant_runtime_configuration_list_but_can_issue_an_initial_bootstrap_url()
    {
        var commonRequestService = CreateHostCommonRequestService();
        var behavior = new TenantDeviceConfigurationPermissionBehavior<
            GetAllTenantDeviceConfigurationsQuery,
            ApiResponse<List<TenantDeviceConfigurationResponseDTO>>>(
            CreateUnitOfWork(),
            commonRequestService,
            NullLogger<TenantDeviceConfigurationPermissionBehavior<
                GetAllTenantDeviceConfigurationsQuery,
                ApiResponse<List<TenantDeviceConfigurationResponseDTO>>>>.Instance);
        var listRequest = new GetAllTenantDeviceConfigurationsQuery(new GetTenantDeviceConfigurationListRequestDTO());

        Assert.ThrowsAsync<ForbiddenAccessException>(() => behavior.Handle(
            listRequest,
            _ => Task.FromResult(ApiResponse<List<TenantDeviceConfigurationResponseDTO>>.Success([])),
            CancellationToken.None));

        var bootstrapBehavior = new TenantDeviceConfigurationPermissionBehavior<
            IssueInitialDeviceBootstrapCommand,
            ApiResponse<InitialDeviceBootstrapResponseDTO>>(
            CreateUnitOfWork(),
            commonRequestService,
            NullLogger<TenantDeviceConfigurationPermissionBehavior<
                IssueInitialDeviceBootstrapCommand,
                ApiResponse<InitialDeviceBootstrapResponseDTO>>>.Instance);
        var nextWasCalled = false;

        await bootstrapBehavior.Handle(
            new IssueInitialDeviceBootstrapCommand(new IssueInitialDeviceBootstrapRequestDTO()),
            _ =>
            {
                nextWasCalled = true;
                return Task.FromResult(ApiResponse<InitialDeviceBootstrapResponseDTO>.Success(new InitialDeviceBootstrapResponseDTO()));
            },
            CancellationToken.None);

        Assert.That(nextWasCalled, Is.True);
    }

    [Test]
    public async Task Tenant_already_verified_conflict_is_serialised_as_http_409_in_the_standard_error_envelope()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ErrorHandlerMiddleware(
            _ => throw new ConflictException(AppConstants.ErrorMessages.TenantAlreadyVerified),
            NullLogger<ErrorHandlerMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
            Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
        });
    }

    private static Tenant CreateTenant(bool isVerified, bool credentialIsOnboard)
    {
        var tenant = new Tenant
        {
            Id = 71,
            CompanyName = "Regression Tenant",
            CompanyEmailDomain = "example.test",
            TenantEmail = "owner@example.test",
            IsVerified = isVerified,
            IsActive = true,
            IsSoftDeleted = false
        };
        tenant.Employee.Add(new Employee
        {
            Id = 12,
            TenantId = tenant.Id,
            IsSoftDeleted = false,
            FirstName = "Owner",
            LoginCredential =
            [
                new LoginCredential
                {
                    Id = 1,
                    TenantId = tenant.Id,
                    EmployeeId = 12,
                    LoginId = tenant.TenantEmail,
                    IsOnboard = credentialIsOnboard,
                    IsSoftDeleted = false
                }
            ]
        });
        return tenant;
    }

    private static IUnitOfWork CreateUnitOfWork(Tenant? tenant = null)
    {
        var tenantRepository = CreateProxy<ITenantRepository>((method, _) => method.Name switch
        {
            nameof(ITenantRepository.GetHostManagedTenantByIdAsync) => Task.FromResult(tenant),
            _ => throw new NotSupportedException($"Unexpected Tenant repository call: {method.Name}.")
        });
        return CreateProxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_TenantRepository" => tenantRepository,
            "get_StoreProcedureRepository" => CreateProxy<IStoreProcedureRepository>((unexpectedMethod, _) =>
                throw new NotSupportedException($"Unexpected store procedure call: {unexpectedMethod.Name}.")),
            _ => throw new NotSupportedException($"Unexpected unit-of-work call: {method.Name}.")
        });
    }

    private static ICommonRequestService CreateHostCommonRequestService() =>
        CreateProxy<ICommonRequestService>((method, _) => method.Name switch
        {
            nameof(ICommonRequestService.ValidateAuthenticatedRequestAsync) => Task.FromResult(new AuthenticatedRequestContext
            {
                UserType = LoginUserType.Host,
                AuthenticatedUserId = 1
            }),
            nameof(ICommonRequestService.ValidateHostUserPermissionRequestAsync) => Task.FromResult(new HostUserRequestContext
            {
                HostUserId = 1,
                TokenHostRoleId = AppConstants.SuperAdminHostRoleId,
                CurrentHostRoleId = AppConstants.SuperAdminHostRoleId,
                UserType = AppConstants.HostUserType,
                TenantEncryptionKey = "host-regression-key"
            }),
            _ => throw new NotSupportedException($"Unexpected common-request call: {method.Name}.")
        });

    private class InterfaceProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            Handler(targetMethod ?? throw new InvalidOperationException("A proxy method was not supplied."), args);
    }

    private static T CreateProxy<T>(Func<MethodInfo, object?[]?, object?> handler)
        where T : class
    {
        var proxy = DispatchProxy.Create<T, InterfaceProxy>();
        ((InterfaceProxy)(object)proxy).Handler = handler;
        return proxy;
    }
}
