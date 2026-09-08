// ================================================================
// Purpose : Regression coverage for authenticated Host administration APIs,
//           the Tenant onboarding verification state, and the deliberate
//           Host/Tenant device-configuration ownership boundary.
// ================================================================

using System.Reflection;
using System.Text.RegularExpressions;
using axionpro.api.Controllers.DefaultEmailConfig;
using axionpro.api.Controllers.EmailTemplate;
using axionpro.api.Controllers.Host;
using axionpro.api.Controllers.HostDevice;
using axionpro.api.Controllers.Tenant;
using axionpro.api.Controllers.TenantEmailConfig;
using axionpro.api.Middlewares;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.Navigation;
using axionpro.application.Exceptions;
using axionpro.application.Features.HostCmd.Handler;
using axionpro.application.Features.HostDeviceCmd;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using axionpro.application.Features.NavigationCmd.Handlers;
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
using Microsoft.AspNetCore.Identity;
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
            CreateUnitOfWork(hostPermissionAllowed: true),
            commonRequestService,
            NullLogger<TenantDeviceConfigurationPermissionBehavior<
                GetAllTenantDeviceConfigurationsQuery,
                ApiResponse<List<TenantDeviceConfigurationResponseDTO>>>>.Instance);
        var listRequest = new GetAllTenantDeviceConfigurationsQuery(
            new GetTenantDeviceConfigurationListRequestDTO
            {
                ModuleId = 100,
                OperationId = 1
            });

        Assert.ThrowsAsync<ForbiddenAccessException>(() => behavior.Handle(
            listRequest,
            _ => Task.FromResult(ApiResponse<List<TenantDeviceConfigurationResponseDTO>>.Success([])),
            CancellationToken.None));

        var bootstrapBehavior = new TenantDeviceConfigurationPermissionBehavior<
            IssueInitialDeviceBootstrapCommand,
            ApiResponse<InitialDeviceBootstrapResponseDTO>>(
            CreateUnitOfWork(hostPermissionAllowed: true),
            commonRequestService,
            NullLogger<TenantDeviceConfigurationPermissionBehavior<
                IssueInitialDeviceBootstrapCommand,
                ApiResponse<InitialDeviceBootstrapResponseDTO>>>.Instance);
        var nextWasCalled = false;

        await bootstrapBehavior.Handle(
            new IssueInitialDeviceBootstrapCommand(
                new IssueInitialDeviceBootstrapRequestDTO
                {
                    ModuleId = 100,
                    OperationId = 1
                }),
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

    [TestCase(AppConstants.SuperAdminHostRoleId)]
    [TestCase(2L)]
    public async Task My_menu_uses_the_current_host_role_for_all_host_users(long currentHostRoleId)
    {
        long? receivedRoleId = null;
        var moduleRepository = CreateProxy<IModuleRepository>((method, args) =>
        {
            if (method.Name != nameof(IModuleRepository.GetHostNavigationMenuAsync))
            {
                throw new NotSupportedException($"Unexpected module repository call: {method.Name}.");
            }

            receivedRoleId = Convert.ToInt64(args![0]);
            return Task.FromResult<IReadOnlyCollection<NavigationMenuItemResponseDTO>>(
            [
                new NavigationMenuItemResponseDTO
                {
                    Id = 44,
                    ModuleCode = "HOST_REGRESSION",
                    ModuleName = "Host Regression",
                    ModuleScope = (short)AppConstants.HostModuleScope,
                    IsLeafNode = true
                }
            ]);
        });
        var unitOfWork = CreateProxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_ModuleRepository" => moduleRepository,
            _ => throw new NotSupportedException($"Unexpected unit-of-work call: {method.Name}.")
        });
        var handler = new GetMyNavigationMenuQueryHandler(
            CreateHostCommonRequestService(currentHostRoleId),
            unitOfWork,
            NullLogger<GetMyNavigationMenuQueryHandler>.Instance);

        var response = await handler.Handle(new GetMyNavigationMenuQuery(), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(receivedRoleId, Is.EqualTo(currentHostRoleId));
            Assert.That(response.IsSucceeded, Is.True);
            Assert.That(response.Data!.Items.Single().ModuleCode, Is.EqualTo("HOST_REGRESSION"));
        });
    }

    [Test]
    public async Task Host_role_permission_editor_marks_only_persisted_host_grants_as_allowed()
    {
        var hostMapping = new ModuleOperationMapping
        {
            Id = 41,
            ModuleId = 11,
            OperationId = 7,
            IsActive = true,
            IsOperational = true,
            Module = new axionpro.domain.Entity.Module
            {
                Id = 11,
                ModuleScope = (short)AppConstants.HostModuleScope,
                ModuleName = "Host Regression",
                IsActive = true
            },
            Operation = new Operation
            {
                Id = 7,
                OperationName = "View",
                IsActive = true
            }
        };
        var tenantMapping = new ModuleOperationMapping
        {
            Id = 42,
            ModuleId = 12,
            OperationId = 7,
            IsActive = true,
            IsOperational = true,
            Module = new axionpro.domain.Entity.Module
            {
                Id = 12,
                ModuleScope = (short)AppConstants.TenantModuleScope,
                ModuleName = "Tenant Regression",
                IsActive = true
            },
            Operation = new Operation
            {
                Id = 7,
                OperationName = "View",
                IsActive = true
            }
        };
        var hostRoleRepository = CreateProxy<IHostRoleRepository>((method, _) => method.Name switch
        {
            nameof(IHostRoleRepository.GetByIdAsync) => Task.FromResult<HostRole?>(new HostRole
            {
                Id = AppConstants.SuperAdminHostRoleId,
                Name = "Host-Super-Admin",
                IsActive = true,
                IsSoftDeleted = false
            }),
            _ => throw new NotSupportedException($"Unexpected Host role repository call: {method.Name}.")
        });
        var hostPermissionRepository = CreateProxy<IHostRolePermissionRepository>((method, _) => method.Name switch
        {
            nameof(IHostRolePermissionRepository.GetHostUserPermissionsAsync) =>
                Task.FromResult(new List<HostUserPermissionResponseDTO>
                {
                    new()
                    {
                        ModuleId = hostMapping.ModuleId,
                        OperationId = hostMapping.OperationId,
                        ModuleName = hostMapping.Module.ModuleName,
                        OperationName = hostMapping.Operation.OperationName
                    }
                }),
            _ => throw new NotSupportedException($"Unexpected Host permission repository call: {method.Name}.")
        });
        var moduleRepository = CreateProxy<IModuleRepository>((method, _) => method.Name switch
        {
            nameof(IModuleRepository.GetAllModuleOperationMappingsAsync) =>
                Task.FromResult(new List<ModuleOperationMapping> { hostMapping, tenantMapping }),
            _ => throw new NotSupportedException($"Unexpected module repository call: {method.Name}.")
        });
        var unitOfWork = CreateProxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_HostRoleRepository" => hostRoleRepository,
            "get_HostRolePermissionRepository" => hostPermissionRepository,
            "get_ModuleRepository" => moduleRepository,
            _ => throw new NotSupportedException($"Unexpected unit-of-work call: {method.Name}.")
        });
        var commonRequestService = CreateProxy<ICommonRequestService>((method, _) => method.Name switch
        {
            nameof(ICommonRequestService.ValidateHostSuperAdminRequestAsync) => Task.FromResult(new HostUserRequestContext
            {
                HostUserId = 1,
                TokenHostRoleId = AppConstants.SuperAdminHostRoleId,
                CurrentHostRoleId = AppConstants.SuperAdminHostRoleId,
                UserType = AppConstants.HostUserType
            }),
            _ => throw new NotSupportedException($"Unexpected common-request call: {method.Name}.")
        });
        var handler = new GetHostRoleModulePermissionsQueryHandler(unitOfWork, commonRequestService);

        var response = await handler.Handle(
            new GetHostRoleModulePermissionsQuery(AppConstants.SuperAdminHostRoleId),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSucceeded, Is.True);
            Assert.That(response.Data!.Modules, Has.Count.EqualTo(1));
            Assert.That(response.Data.Modules.Single().ModuleId, Is.EqualTo(hostMapping.ModuleId));
            Assert.That(response.Data.Modules.Single().Operations.Single().IsAllowed, Is.True);
        });
    }

    [Test]
    public void Host_super_admin_is_not_exempt_from_the_persisted_permission_check()
    {
        var storeProcedureRepository = CreateProxy<IStoreProcedureRepository>((method, _) => method.Name switch
        {
            nameof(IStoreProcedureRepository.CheckHostUserPermissionAsync) =>
                Task.FromResult(new HostUserPermissionCheckResponseDTO { ResultCode = 0 }),
            _ => throw new NotSupportedException($"Unexpected store procedure call: {method.Name}.")
        });

        var exception = Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            HostRuntimePermissionValidator.ValidateAsync(
                CreateHostCommonRequestService(AppConstants.SuperAdminHostRoleId),
                storeProcedureRepository,
                moduleId: 10,
                operationId: 20,
                CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo(AppConstants.ErrorMessages.PermissionDenied));
    }

    [Test]
    public void Host_seed_contains_the_management_tree_and_canonical_host_auth_reset()
    {
        var seed = LoadProductionSeed();
        var hostSection = Slice(seed, "-- SECTION 8E", "-- SECTION 9");
        var cardUpgrade = File.ReadAllText(
            FindRepositoryFile("database-scripts/TenantCardMaster_EmployeeDeviceCredential_Upgrade.sql"));
        var moduleCodes = new[]
        {
            "HOST_MANAGEMENT", "HOST_TENANT_RFID_MANAGEMENT", "HOST_USERS", "HOST_ROLES", "HOST_ROLE_PERMISSIONS",
            "HOST_MODULES", "HOST_SUBMODULES", "HOST_OPERATIONS",
            "HOST_MODULE_OPERATIONS", "HOST_SUBSCRIPTIONS", "HOST_TENANT_CARD_INVENTORY"
        };
        var moduleSeed = Slice(seed, "INSERT INTO module_seed", "UPDATE axionpro.\"Module\" module");

        Assert.Multiple(() =>
        {
            foreach (var moduleCode in moduleCodes)
            {
                Assert.That(seed, Does.Contain($"'{moduleCode}'"), $"Missing Host module code {moduleCode}.");
            }

            foreach (var childModuleCode in moduleCodes.Skip(2).Where(code => code != "HOST_TENANT_CARD_INVENTORY"))
            {
                var tuple = Regex.Match(
                    moduleSeed,
                    $"\\(\\s*'{Regex.Escape(childModuleCode)}'.*?\\n\\),",
                    RegexOptions.Singleline).Value;
                Assert.That(tuple, Is.Not.Empty, $"Missing module_seed tuple for {childModuleCode}.");
                Assert.That(tuple, Does.Contain("'HOST_MANAGEMENT'"), $"{childModuleCode} must be under HOST_MANAGEMENT.");
                Assert.That(tuple, Does.Match(@"\n\s*2,\s*\n"), $"{childModuleCode} must be Host scope 2.");
            }

            Assert.That(moduleSeed, Does.Contain("'/app/host-users'"));
            Assert.That(moduleSeed, Does.Contain("'/app/host-roles'"));
            Assert.That(moduleSeed, Does.Contain("'/app/host-roles/permissions'"));
            Assert.That(moduleSeed, Does.Contain("'/app/modules'"));
            Assert.That(moduleSeed, Does.Contain("'/app/modules/sub-modules'"));
            Assert.That(moduleSeed, Does.Contain("'/app/modules/operations'"));
            Assert.That(moduleSeed, Does.Contain("'/app/modules/module-operations'"));
            Assert.That(moduleSeed, Does.Contain("'/app/subscriptions'"));
            var cardTuple = Regex.Match(
                moduleSeed,
                "\\(\\s*'HOST_TENANT_CARD_INVENTORY'.*?\\n\\),",
                RegexOptions.Singleline).Value;
            Assert.That(cardTuple, Does.Contain("'HOST_TENANT_RFID_MANAGEMENT'"));
            Assert.That(seed, Does.Contain("'HOST_TENANT_RFID_MANAGEMENT'"));
            Assert.That(seed, Does.Contain("'bi bi-broadcast-pin'"));
            Assert.That(cardUpgrade, Does.Contain("'HOST_TENANT_RFID_MANAGEMENT'"));
            Assert.That(cardUpgrade, Does.Contain("parent.\"Id\""));
            Assert.That(cardUpgrade, Does.Contain("m.\"ModuleCode\"='HOST_TENANT_CARD_INVENTORY'"));
            Assert.That(cardUpgrade, Does.Not.Contain("m.\"ModuleCode\"='HOST_TENANT_RFID_MANAGEMENT'"));
            Assert.That(seed, Does.Contain("'bi bi-shield-lock'"));
            Assert.That(hostSection, Does.Contain("DELETE FROM axionpro.\"RefreshToken\""));
            Assert.That(hostSection, Does.Contain("DELETE FROM axionpro.\"HostRoleModuleAndPermission\""));
            Assert.That(hostSection, Does.Contain("DELETE FROM axionpro.\"HostUser\""));
            Assert.That(hostSection, Does.Contain("DELETE FROM axionpro.\"HostRole\""));
            Assert.That(hostSection, Does.Contain("'Host-Super-Admin'"));
            Assert.That(Regex.Matches(hostSection, "'mca.deepesh@gmail.com'").Count, Is.EqualTo(2));
            Assert.That(hostSection, Does.Contain("'9111161399'"));
            Assert.That(hostSection, Does.Contain("PasswordHash is ASP.NET Core Identity PasswordHasher V3 for 12344321."));
            Assert.That(hostSection, Does.Contain("AQAAAAIAAYagAAAAEEDpT6tXHxx4OhhP394Aqp4vlsVunbyd3qQGOnszn4oghxYFlkERmuDjy0ATNqawgw=="));
            Assert.That(hostSection, Does.Contain("pg_get_serial_sequence('axionpro.\"HostRoleModuleAndPermission\"', 'Id')"));
            Assert.That(hostSection, Does.Contain("module.\"ModuleScope\" = 2"));
            Assert.That(hostSection, Does.Contain("mapping.\"IsOperational\" = TRUE"));
        });

        var refreshDelete = hostSection.IndexOf("DELETE FROM axionpro.\"RefreshToken\"", StringComparison.Ordinal);
        var permissionDelete = hostSection.IndexOf("DELETE FROM axionpro.\"HostRoleModuleAndPermission\"", StringComparison.Ordinal);
        var userDelete = hostSection.IndexOf("DELETE FROM axionpro.\"HostUser\"", StringComparison.Ordinal);
        var roleDelete = hostSection.IndexOf("DELETE FROM axionpro.\"HostRole\"", StringComparison.Ordinal);

        Assert.That(refreshDelete, Is.LessThan(permissionDelete));
        Assert.That(permissionDelete, Is.LessThan(userDelete));
        Assert.That(userDelete, Is.LessThan(roleDelete));
    }

    [Test]
    public void Host_seed_sql_has_balanced_plpgsql_blocks_and_no_runtime_bypass_marker()
    {
        var seed = LoadProductionSeed();
        var dollarQuoteCount = Regex.Matches(seed, "(?m)^\\$\\$$").Count;

        Assert.Multiple(() =>
        {
            Assert.That(seed.TrimStart(), Does.StartWith("--"));
            Assert.That(seed, Does.Contain("BEGIN;"));
            Assert.That(dollarQuoteCount % 2, Is.EqualTo(0), "PL/pgSQL dollar-quote delimiters must be paired.");
            Assert.That(seed, Does.Not.Contain("Host-Super-Admin bypass is used"));
            Assert.That(File.ReadAllText(FindRepositoryFile("axionpro.application/Common/Helpers/HostRuntimePermissionValidator.cs")),
                Does.Not.Contain("IsHostAdmin"));
            Assert.That(File.ReadAllText(FindRepositoryFile("axionpro.persistance/Repositories/ModuleRepository.cs")),
                Does.Not.Contain("includeAllScopedModules"));
        });
    }

    [Test]
    public void Host_seed_password_hash_accepts_the_documented_canonical_password()
    {
        var seed = LoadProductionSeed();
        var hash = Regex.Match(seed, @"AQAAAA[A-Za-z0-9+/]+=*").Value;
        var verification = new PasswordHasher<string>().VerifyHashedPassword(
            "mca.deepesh@gmail.com",
            hash,
            "12344321");

        Assert.Multiple(() =>
        {
            Assert.That(hash, Is.Not.Empty);
            Assert.That(verification, Is.EqualTo(PasswordVerificationResult.Success));
        });
    }

    [Test]
    public void Host_permission_sql_function_contract_matches_repository_and_persisted_grant_rules()
    {
        var functionSql = File.ReadAllText(
            FindRepositoryFile("database-scripts/AddCheckHostUserPermissionFunction.sql"));
        var repositorySql = File.ReadAllText(
            FindRepositoryFile("axionpro.persistance/Repositories/StoreProcedureRepository.cs"));

        Assert.Multiple(() =>
        {
            Assert.That(functionSql, Does.Contain("p_hostuserid bigint"));
            Assert.That(functionSql, Does.Contain("p_tokenhostroleid bigint"));
            Assert.That(functionSql, Does.Contain("p_moduleid integer"));
            Assert.That(functionSql, Does.Contain("p_operationid integer"));
            Assert.That(functionSql, Does.Contain("'AUTH_CONTEXT_CHANGED'"));
            Assert.That(functionSql, Does.Contain("'PERMISSION_DENIED'"));
            Assert.That(functionSql, Does.Contain("v_currenthostroleid <> p_tokenhostroleid"));
            Assert.That(functionSql, Does.Contain("axionpro.\"HostRoleModuleAndPermission\""));
            Assert.That(functionSql, Does.Contain("permission.\"IsActive\" = TRUE"));
            Assert.That(functionSql, Does.Contain("permission.\"IsSoftDeleted\" = FALSE"));
            Assert.That(repositorySql, Does.Contain("axionpro.\"CheckHostUserPermission\"("));
            Assert.That(repositorySql, Does.Contain("@p_hostuserid"));
            Assert.That(repositorySql, Does.Contain("@p_tokenhostroleid"));
            Assert.That(repositorySql, Does.Contain("@p_moduleid"));
            Assert.That(repositorySql, Does.Contain("@p_operationid"));
        });
    }

    private static string LoadProductionSeed() =>
        File.ReadAllText(FindRepositoryFile("database-scripts/AxionPro_New_Production_Module_Operation_Seed.sql"));

    private static string FindRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new AssertionException($"Could not locate repository file '{relativePath}'.");
    }

    private static string Slice(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        if (start < 0 || end < 0 || end <= start)
        {
            throw new AssertionException($"Could not isolate SQL section '{startMarker}' to '{endMarker}'.");
        }

        return source[start..end];
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

    private static IUnitOfWork CreateUnitOfWork(
        Tenant? tenant = null,
        bool hostPermissionAllowed = false)
    {
        var tenantRepository = CreateProxy<ITenantRepository>((method, _) => method.Name switch
        {
            nameof(ITenantRepository.GetHostManagedTenantByIdAsync) => Task.FromResult(tenant),
            _ => throw new NotSupportedException($"Unexpected Tenant repository call: {method.Name}.")
        });
        return CreateProxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_TenantRepository" => tenantRepository,
            "get_StoreProcedureRepository" => CreateProxy<IStoreProcedureRepository>((method, _) => method.Name switch
            {
                nameof(IStoreProcedureRepository.CheckHostUserPermissionAsync) when hostPermissionAllowed =>
                    Task.FromResult(new HostUserPermissionCheckResponseDTO { ResultCode = 1 }),
                _ => throw new NotSupportedException($"Unexpected store procedure call: {method.Name}.")
            }),
            _ => throw new NotSupportedException($"Unexpected unit-of-work call: {method.Name}.")
        });
    }

    private static ICommonRequestService CreateHostCommonRequestService(
        long currentHostRoleId = AppConstants.SuperAdminHostRoleId) =>
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
                TokenHostRoleId = currentHostRoleId,
                CurrentHostRoleId = currentHostRoleId,
                UserType = AppConstants.HostUserType,
                TenantEncryptionKey = "host-regression-key"
            }),
            nameof(ICommonRequestService.GetModuleCodeAsync) => Task.FromResult<string?>("HOST_INITIAL_DEVICE_CONFIGURATION"),
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
