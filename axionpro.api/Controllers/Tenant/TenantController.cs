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
    /// Supports the Angular UI flow for tenant creation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: performs register tenant.</para>
    /// <para>Angular page(s): /auth/register-tenant; /app/tenants/new; /app/tenants/:tenantId/edit.</para>
    /// <para>Angular API service call(s): TenantsApi.registerTenant (app/core/services/tenants-api.ts:98).</para>
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
    /// Adds missing entitlement snapshot rows for the selected Tenant from its currently active subscription plan.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: explicitly synchronizes a selected Tenant's active-plan modules and operations into its entitlement snapshot.</para>
    /// <para>Provide an encrypted <c>tenantId</c> in the body. Super Admin may use zero <c>moduleId</c>/<c>operationId</c>; other Host roles require valid permission identifiers.</para>
    /// <para>The operation is additive and idempotent: existing TenantEnabledModule and TenantEnabledOperation rows remain unchanged and are never duplicated or removed.</para>
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

    [Authorize]
    [HttpGet("{id}/delete-dependencies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenantDeleteDependencyInfoAsync(
        string id,
        [FromQuery] PermissionRequestDTO permissionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTenantDeleteDependencyInfoQuery(id, permissionRequest),
            cancellationToken);
        return Ok(result);
    }

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
    /// Supports the Angular UI flow for get all tenants async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves tenants.</para>
    /// <para>Angular page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit; /app/host-dashboard; /app/subscriptions; /app/tenant-devices; /app/tenants.</para>
    /// <para>Angular API service call(s): TenantsApi.getTenants (app/core/services/tenants-api.ts:111).</para>
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
    /// Supports the Angular UI flow for get tenant by id async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves tenant by id.</para>
    /// <para>Angular page(s): /app/tenants/new; /app/tenants/:tenantId/edit.</para>
    /// <para>Angular API service call(s): TenantsApi.getTenantById (app/core/services/tenants-api.ts:119).</para>
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

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateHostManagedTenantAsync(
        string id,
        [FromBody] UpdateHostManagedTenantRequestDTO? requestDTO,
        [FromQuery] PermissionRequestDTO? permissionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateHostManagedTenantCommand(id, requestDTO, permissionRequest),
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteHostManagedTenantAsync(
        string id,
        [FromQuery] PermissionRequestDTO? permissionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteHostManagedTenantCommand(id, permissionRequest),
            cancellationToken);

        return Ok(result);
    }

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
    /// Supports the Angular UI flow for update tenant async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates modules and operations.</para>
    /// <para>Angular page(s): /app/tenants/:tenantId/modules.</para>
    /// <para>Angular API service call(s): TenantsApi.updateModulesAndOperations (app/core/services/tenants-api.ts:140).</para>
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
    /// Supports the Angular UI flow for activate tenant async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deactivates tenant.</para>
    /// <para>Angular page(s): /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants.</para>
    /// <para>Angular API service call(s): TenantsApi.deactivateTenant (app/core/services/tenants-api.ts:152).</para>
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
    /// Supports the Angular UI flow for deactivate tenant async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes tenant.</para>
    /// <para>Angular page(s): /app/tenants.</para>
    /// <para>Angular API service call(s): TenantsApi.deleteTenant (app/core/services/tenants-api.ts:158).</para>
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
    /// Supports the Angular UI flow for delete tenant async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: resends verification email.</para>
    /// <para>Angular page(s): /app/tenants.</para>
    /// <para>Angular API service call(s): TenantsApi.resendVerificationEmail (app/core/services/tenants-api.ts:164).</para>
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

    [Authorize]
    [HttpPost("create-host-user")]
    public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
    {
        _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
        var command = new CreateHostUserCommand(tenantCreateRequestDTO);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    #endregion

    #region Existing Tenant Configuration Queries

    [HttpGet("get-all-tenant-by-subscription-plan-Id")]
    public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] application.DTOs.Tenant.TenantRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetAllTenantBySubscriptionPlanIdQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for get employee code pattern async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves employee code pattern.</para>
    /// <para>Angular page(s): /app/tenants/new; /app/tenants/:tenantId/edit.</para>
    /// <para>Angular API service call(s): TenantsApi.getEmployeeCodePattern (app/core/services/tenants-api.ts:134).</para>
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

    [HttpPost("get")]
    public async Task<IActionResult> GetAllTenantEnabledModuleOperationsByTenantIdAsync([FromBody] TenantEnabledModuleRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetTenantEnabledModuleCommand(code);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for get all node leafe with operations async.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves tenant operations.</para>
    /// <para>Angular page(s): /app/policies/attendance-policies; /auth/login; /app/admin-dashboard; /app/departments; /app/designations; /app/device-masters; /app/modules/module-operations; /app/modules/operations; and 25 more.</para>
    /// <para>Angular API service call(s): TenantsApi.getTenantOperations (app/core/services/tenants-api.ts:126).</para>
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
    /// Supports the Angular UI flow for tenant module operations update.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: activates tenant.</para>
    /// <para>Angular page(s): /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants.</para>
    /// <para>Angular API service call(s): TenantsApi.activateTenant (app/core/services/tenants-api.ts:146).</para>
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
    /// Supports the Angular UI flow for verify email.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: validates email.</para>
    /// <para>Angular page(s): /auth/registration-verify.</para>
    /// <para>Angular API service call(s): TenantsApi.verifyEmail (app/core/services/tenants-api.ts:104).</para>
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
