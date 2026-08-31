// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes Tenant creation, configuration, verification, and Host-side management endpoints.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.Verify;
using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Features.HostCmd.Handler;
using axionpro.application.Features.RegistrationCmd.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Features.TenantManagementCmd.Commands;
using axionpro.application.Features.TenantManagementCmd.Queries;
using axionpro.application.Features.VerifyEmailCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Tenant;

/// <summary>
/// Provides API endpoints for Tenant registration, configuration, verification, and Host-side management requests.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to dispatch application requests.</param>
    /// <param name="logger">The application logger.</param>
    public TenantController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #endregion

    #region Existing Tenant Creation Command

    /// <summary>
    /// Used-In-Angular: creates tenant.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.registerTenant (app/core/services/tenants-api.ts:105).</para>
    /// <para>Angular purpose: creates tenant.</para>
    /// <para>Integrated UI page(s): /auth/register-tenant</para>
    /// <para>Angular UI component(s): Registration (app/features/authentication/registration/registration.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpPost("create-tenant")]
    public async Task<IActionResult> TenantCreation([FromBody] application.DTOs.Registration.TenantCreateRequestDTO tenantCreateRequestDTO)
    {
        _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
        var command = new CreateTenantCommand(tenantCreateRequestDTO);
        var result = await _mediator.Send(command);
       
        return Ok(result);
    }

    #endregion

    #region Tenant Management Queries
    /// <summary>
    /// Used-In-Angular: updates tenant by host.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.updateTenantByHost (app/core/services/tenants-api.ts:169).</para>
    /// <para>Angular purpose: updates tenant by host.</para>
    /// <para>Integrated UI page(s): /app/tenants/new; /app/tenants/:tenantId/edit</para>
    /// <para>Angular UI component(s): TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
    /// </remarks>

    [Authorize]
    [HttpPut("new-tenant-update-by-host/{encryptedTenantId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateNewTenantAsync(
        string encryptedTenantId,
        [FromBody] NewTenantUpdateRequestDTO? requestDTO,
        [FromQuery] PermissionRequestDTO? permissionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateNewTenantCommand(encryptedTenantId, requestDTO, permissionRequest),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: performs the Angular function sync active plan entitlements.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.syncActivePlanEntitlements (app/core/services/tenants-api.ts:216).</para>
    /// <para>Angular purpose: performs the Angular function sync active plan entitlements.</para>
    /// <para>Integrated UI page(s): /app/tenants</para>
    /// <para>Angular UI component(s): TenantsStore (app/features/host/tenants/tenants.store.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpPost("sync-active-plan-entitlements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SynchronizeTenantPlanEntitlementsAsync(
        [FromBody] SynchronizeTenantPlanEntitlementsRequestDTO? requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SynchronizeTenantPlanEntitlementsCommand(requestDTO),
            cancellationToken);
        return Ok(result);
    }
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: GET /api/tenant/{}/delete-dependencies.</para>
    //     /// </remarks>
    //
    //     [Authorize]
    //     [HttpGet("{id}/delete-dependencies")]
    //     [ProducesResponseType(StatusCodes.Status200OK)]
    //     public async Task<IActionResult> GetTenantDeleteDependencyInfoAsync(
    //         string id,
    //         [FromQuery] PermissionRequestDTO permissionRequest,
    //         CancellationToken cancellationToken)
    //     {
    //         var result = await _mediator.Send(
    //             new GetTenantDeleteDependencyInfoQuery(id, permissionRequest),
    //             cancellationToken);
    //         return Ok(result);
    //     }
    #endregion
    /// <summary>
    /// Used-In-Angular: creates tenant by host.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.createTenantByHost (app/core/services/tenants-api.ts:117).</para>
    /// <para>Angular purpose: creates tenant by host.</para>
    /// <para>Integrated UI page(s): /app/tenants/new; /app/tenants/:tenantId/edit</para>
    /// <para>Angular UI component(s): TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
    /// </remarks>

    [Authorize]
    [HttpPost("new-tentant-creation-by-host")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewTenantAsync(
        [FromBody] NewTenantCreationRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateNewTenantCommand(requestDTO), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: validates email.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.verifyEmail (app/core/services/tenants-api.ts:131).</para>
    /// <para>Angular purpose: validates email.</para>
    /// <para>Integrated UI page(s): /auth/registration-verify</para>
    /// <para>Angular UI component(s): RegistrationVerify (app/features/authentication/registration/registration-verify/registration-verify.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpGet("get-all-tenants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTenantsAsync([FromQuery] GetAllTenantsRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetAllTenantsQuery(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: retrieves tenant by id.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.getTenantById (app/core/services/tenants-api.ts:139).</para>
    /// <para>Angular purpose: retrieves tenant by id.</para>
    /// <para>Integrated UI page(s): /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants</para>
    /// <para>Angular UI component(s): TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpGet("get-tenant-by-id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenantByIdAsync([FromQuery] GetTenantByIdRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(requestDTO));
        return Ok(result);
    }

    #endregion

    #region Tenant Management Commands
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: PUT /api/tenant/{}.</para>
    //     /// </remarks>
    //
    //     [Authorize]
    //     [HttpPut("{id}")]
    //     [ProducesResponseType(StatusCodes.Status200OK)]
    //     public async Task<IActionResult> UpdateHostManagedTenantAsync(
    //         string id,
    //         [FromBody] UpdateHostManagedTenantRequestDTO? requestDTO,
    //         [FromQuery] PermissionRequestDTO? permissionRequest,
    //         CancellationToken cancellationToken)
    //     {
    //         var result = await _mediator.Send(
    //             new UpdateHostManagedTenantCommand(id, requestDTO, permissionRequest),
    //             cancellationToken);
    //
    //         return Ok(result);
    //     }
    #endregion
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: DELETE /api/tenant/{}.</para>
    //     /// </remarks>
    //
    //     [Authorize]
    //     [HttpDelete("{id}")]
    //     [ProducesResponseType(StatusCodes.Status200OK)]
    //     public async Task<IActionResult> DeleteHostManagedTenantAsync(
    //         string id,
    //         [FromQuery] PermissionRequestDTO? permissionRequest,
    //         CancellationToken cancellationToken)
    //     {
    //         var result = await _mediator.Send(
    //             new DeleteHostManagedTenantCommand(id, permissionRequest),
    //             cancellationToken);
    //
    //         return Ok(result);
    //     }
    #endregion
    /// <summary>
    /// Used-In-Angular: performs the Angular function resend verification email.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.resendVerificationEmail (app/core/services/tenants-api.ts:208).</para>
    /// <para>Angular purpose: performs the Angular function resend verification email.</para>
    /// <para>Integrated UI page(s): /app/tenants</para>
    /// <para>Angular UI component(s): TenantsStore (app/features/host/tenants/tenants.store.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>

    [Authorize]
    [HttpPost("{id}/resend-verification-by-host")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResendTenantVerificationAsync(
        string id,
        [FromQuery] PermissionRequestDTO? permissionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ResendTenantVerificationCommand(id, permissionRequest),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: updates tenant.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.updateTenant (app/core/services/tenants-api.ts:177).</para>
    /// <para>Angular purpose: updates tenant.</para>
    /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
    /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
    /// </remarks>
    [Authorize]
    [HttpPost("update-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(
            new UpdateHostManagedTenantCommand(requestDTO.TenantId, requestDTO, requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: performs the Angular function activate tenant.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.activateTenant (app/core/services/tenants-api.ts:189).</para>
    /// <para>Angular purpose: performs the Angular function activate tenant.</para>
    /// <para>Integrated UI page(s): /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants</para>
    /// <para>Angular UI component(s): TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); TenantsStore (app/features/host/tenants/tenants.store.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpPost("activate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateTenantAsync([FromBody] ActivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new ActivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: performs the Angular function deactivate tenant.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.deactivateTenant (app/core/services/tenants-api.ts:195).</para>
    /// <para>Angular purpose: performs the Angular function deactivate tenant.</para>
    /// <para>Integrated UI page(s): /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants</para>
    /// <para>Angular UI component(s): TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); TenantsStore (app/features/host/tenants/tenants.store.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpPost("deactivate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateTenantAsync([FromBody] DeactivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new DeactivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: deletes tenant.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.deleteTenant (app/core/services/tenants-api.ts:201).</para>
    /// <para>Angular purpose: deletes tenant.</para>
    /// <para>Integrated UI page(s): /app/tenants</para>
    /// <para>Angular UI component(s): TenantsStore (app/features/host/tenants/tenants.store.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
    /// </remarks>
    [Authorize]
    [HttpPost("delete-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteTenantAsync([FromBody] DeleteTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(
            new DeleteHostManagedTenantCommand(requestDTO.TenantId, requestDTO));
        return Ok(result);
    }

    #endregion

    #region Existing Host User Command
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: POST /api/tenant/create-host-user.</para>
    //     /// </remarks>
    //
    //     [Authorize]
    //     [HttpPost("create-host-user")]
    //     public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
    //     {
    //         _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
    //         var command = new CreateHostUserCommand(tenantCreateRequestDTO);
    //         var result = await _mediator.Send(command);
    //
    //         return Ok(result);
    //     }
    #endregion

    #endregion

    #region Existing Tenant Configuration Queries
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: GET /api/tenant/get-all-tenant-by-subscription-plan-id.</para>
    //     /// </remarks>
    //
    //     [HttpGet("get-all-tenant-by-subscription-plan-Id")]
    //     public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] application.DTOs.Tenant.TenantRequestDTO code)
    //     {
    //         _logger.LogInfo($"Getting email templates for code: {code}");
    //
    //         var query = new GetAllTenantBySubscriptionPlanIdQuery(code);
    //         var result = await _mediator.Send(query);
    //
    //         return Ok(result);
    //     }
    #endregion

    /// <summary>
    /// Used-In-Angular: retrieves employee code pattern.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.getEmployeeCodePattern (app/core/services/tenants-api.ts:154).</para>
    /// <para>Angular purpose: retrieves employee code pattern.</para>
    /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
    /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
    /// </remarks>
    [HttpGet("get-employee-code-pattern")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeCodePatternAsync([FromQuery] EmployeeCodePatternRequestDTO code)
    {
        _logger.LogInfo("Fetching employee code pattern for tenant.");

        var query = new GetEmployeeCodePatternQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
    #region Unused
    //     /// <summary>
    //     /// Not-Used-In-Angular.
    //     /// </summary>
    //     /// <remarks>
    //     /// <para>Angular usage status: Not-Used-In-Angular.</para>
    //     /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
    //     /// <para>Backend endpoint: POST /api/tenant/get.</para>
    //     /// </remarks>
    //
    //     [HttpPost("get")]
    //     public async Task<IActionResult> GetAllTenantEnabledModuleOperationsByTenantIdAsync([FromBody] TenantEnabledModuleRequestDTO code)
    //     {
    //         _logger.LogInfo($"Getting email templates for code: {code}");
    //
    //         var query = new GetTenantEnabledModuleCommand(code);
    //         var result = await _mediator.Send(query);
    //         return Ok(result);
    //     }
    #endregion

    /// <summary>
    /// Used-In-Angular: retrieves tenant operations.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.getTenantOperations (app/core/services/tenants-api.ts:146).</para>
    /// <para>Angular purpose: retrieves tenant operations.</para>
    /// <para>Integrated UI page(s): /app/policies/attendance-policies; /auth/login; /app/admin-dashboard; /app/departments; /app/designations; /app/device-masters; /app/modules/module-operations; /app/modules/operations</para>
    /// <para>Angular UI component(s): CurrentUserPermissionsStore (app/core/stores/current-user-permissions.store.ts); EmployeesPermissionsStore (app/features/employees/employees-permissions/employees-permissions.store.ts); RolePermissionsStore (app/features/roles/role-permissions/role-permissions.store.ts); hasModuleOperationGuard (app/core/guards/has-module-operation-guard.ts); hasModulePermissionGuard (app/core/guards/has-module-permission-guard.ts); superAdminGuard (app/core/guards/super-admin-guard.ts); AttendancePolicies (app/features/attendance-policies/attendance-policies.ts); Login (app/features/authentication/login/login.ts)</para>
    /// </remarks>
    [HttpGet("get-all-tenant-operations")]
    public async Task<IActionResult> GetAllNodeLeafeWithOperationsAsync([FromQuery] TenantEnabledOperationsRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetAllTenantOperationsCommand(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    #endregion

    #region Existing Tenant Configuration Command

    /// <summary>
    /// Used-In-Angular: updates modules and operations.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.updateModulesAndOperations (app/core/services/tenants-api.ts:183).</para>
    /// <para>Angular purpose: updates modules and operations.</para>
    /// <para>Integrated UI page(s): /app/tenants/:tenantId/modules</para>
    /// <para>Angular UI component(s): TenantModulesStore (app/features/host/tenants/tenant-modules/tenant-modules.store.ts); TenantModules (app/features/host/tenants/tenant-modules/tenant-modules.ts)</para>
    /// </remarks>
    [HttpPost("update-modules-and-operations")]
    public async Task<IActionResult> TenantModuleOperationsUpdate([FromBody] TenantModuleOperationsUpdateRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new TenantEnabledModuleOperationsUpdateCommand(code);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Existing Tenant Verification Command

    /// <summary>
    /// Used-In-Angular: validates email.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantsApi.verifyEmail (app/core/services/tenants-api.ts:124).</para>
    /// <para>Angular purpose: validates email.</para>
    /// <para>Integrated UI page(s): /auth/registration-verify</para>
    /// <para>Angular UI component(s): RegistrationVerify (app/features/authentication/registration/registration-verify/registration-verify.ts)</para>
    /// </remarks>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
    {
        var command = new VerifyEmailCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion
}
