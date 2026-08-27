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
    /// Tenant Creation.
    /// </summary>
    /// <remarks>
    /// Handles the request to tenant creation.
    /// </remarks>
    /// <param name="tenantCreateRequestDTO">The request body used to tenant creation.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Retrieves a page of Host-managed Tenants.
    /// </summary>
    /// <remarks>
    /// Requires an authenticated Host request. The current Super Admin may omit ModuleId and
    /// OperationId; every other Host role must provide an assigned module-operation permission pair.
    /// </remarks>
    /// <param name="requestDTO">The query parameters used to get all tenants.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [Authorize]
    [HttpGet("get-all-tenants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTenantsAsync([FromQuery] GetAllTenantsRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetAllTenantsQuery(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Get Tenant By ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get tenant by id.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The query parameters used to get tenant by id.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Update Host Managed Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to update host managed tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="requestDTO">The request body used to update host managed tenant.</param>
    /// <param name="permissionRequest">The query parameters used to update host managed tenant.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Delete Host Managed Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete host managed tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="permissionRequest">The query parameters used to delete host managed tenant.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Resend Tenant Verification.
    /// </summary>
    /// <remarks>
    /// Handles the request to resend tenant verification.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="permissionRequest">The query parameters used to resend tenant verification.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [Authorize]
    [HttpPost("{id}/resend-verification")]
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
    /// Update Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to update tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to update tenant.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Activate Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to activate tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to activate tenant.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [Authorize]
    [HttpPost("activate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateTenantAsync([FromBody] ActivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new ActivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Deactivate Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to deactivate tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to deactivate tenant.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [Authorize]
    [HttpPost("deactivate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateTenantAsync([FromBody] DeactivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new DeactivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Delete Tenant.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete tenant.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="requestDTO">The request body used to delete tenant.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Create Host User.
    /// </summary>
    /// <remarks>
    /// Handles the request to create host user.
    /// Requires an authenticated user.
    /// </remarks>
    /// <param name="tenantCreateRequestDTO">The request body used to create host user.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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

    /// <summary>
    /// Get All Tenant By Subscription ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all tenant by subscription id.
    /// </remarks>
    /// <param name="code">The query parameters used to get all tenant by subscription id.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-all-tenant-by-subscription-plan-Id")]
    public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] application.DTOs.Tenant.TenantRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetAllTenantBySubscriptionPlanIdQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get Employee Code Pattern.
    /// </summary>
    /// <remarks>
    /// Handles the request to get employee code pattern.
    /// </remarks>
    /// <param name="code">The query parameters used to get employee code pattern.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-employee-code-pattern")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeCodePatternAsync([FromQuery] EmployeeCodePatternRequestDTO code)
    {
        _logger.LogInfo("Fetching employee code pattern for tenant.");

        var query = new GetEmployeeCodePatternQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get All Tenant Enabled Module Operations By Tenant ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all tenant enabled module operations by tenant id.
    /// </remarks>
    /// <param name="code">The request body used to get all tenant enabled module operations by tenant id.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("get")]
    public async Task<IActionResult> GetAllTenantEnabledModuleOperationsByTenantIdAsync([FromBody] TenantEnabledModuleRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetTenantEnabledModuleCommand(code);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get All Node Leafe With Operations.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all node leafe with operations.
    /// </remarks>
    /// <param name="code">The query parameters used to get all node leafe with operations.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Tenant Module Operations Update.
    /// </summary>
    /// <remarks>
    /// Handles the request to tenant module operations update.
    /// </remarks>
    /// <param name="code">The request body used to tenant module operations update.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Verify Email.
    /// </summary>
    /// <remarks>
    /// Handles the request to verify email.
    /// </remarks>
    /// <param name="request">The request body used to verify email.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
    {
        var command = new VerifyEmailCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion
}
