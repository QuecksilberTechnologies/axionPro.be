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
    /// Creates a Tenant through the existing registration flow.
    /// </summary>
    /// <param name="tenantCreateRequestDTO">The existing Tenant registration request.</param>
    /// <returns>The result returned by the existing Tenant creation command.</returns>
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
    /// Retrieves Host-managed Tenant records.
    /// An authenticated Host user must supply the requested ModuleId and OperationId. The current Host role is checked at runtime.
    /// </summary>
    /// <param name="requestDTO">The Tenant management filter and paging request.</param>
    /// <returns>The Tenant management list response.</returns>
    [Authorize]
    [HttpGet("get-all-tenants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllTenantsAsync([FromQuery] GetAllTenantsRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetAllTenantsQuery(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves one Host-managed Tenant by encrypted identifier.
    /// The identifier is decrypted only after Host authentication and runtime module-operation authorization succeed.
    /// </summary>
    /// <param name="requestDTO">The Tenant identifier request.</param>
    /// <returns>The requested Tenant response.</returns>
    [Authorize]
    [HttpGet("get-tenant-by-id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantByIdAsync([FromQuery] GetTenantByIdRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(requestDTO));
        return Ok(result);
    }

    #endregion

    #region Tenant Management Commands

    /// <summary>
    /// Updates editable Host-managed Tenant details.
    /// The route identifier is an encrypted string; ModuleId and OperationId are required query parameters for Host runtime authorization.
    /// </summary>
    /// <param name="id">The encrypted Tenant identifier from the route.</param>
    /// <param name="requestDTO">The client-editable Tenant details.</param>
    /// <param name="permissionRequest">The required Host module-operation permission metadata.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The updated Tenant response.</returns>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Soft deletes a Host-managed Tenant selected by encrypted route identifier.
    /// ModuleId and OperationId are required query parameters and are verified against the current Host role.
    /// </summary>
    /// <param name="id">The encrypted Tenant identifier from the route.</param>
    /// <param name="permissionRequest">The required Host module-operation permission metadata.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The Tenant soft-delete result.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Resends onboarding verification for an unverified Host-managed Tenant.
    /// The route identifier is encrypted and ModuleId and OperationId are required query parameters.
    /// </summary>
    /// <param name="id">The encrypted Tenant identifier from the route.</param>
    /// <param name="permissionRequest">The required Host module-operation permission metadata.</param>
    /// <param name="cancellationToken">The token used to observe request cancellation.</param>
    /// <returns>The resend-verification result.</returns>
    [Authorize]
    [HttpPost("{id}/resend-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Updates Host-managed Tenant details using an encrypted TenantId in the request body.
    /// The current Host role must grant the request ModuleId and OperationId.
    /// </summary>
    /// <param name="requestDTO">The editable Tenant details request.</param>
    /// <returns>The updated Tenant response.</returns>
    [Authorize]
    [HttpPost("update-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(
            new UpdateHostManagedTenantCommand(requestDTO.TenantId, requestDTO, requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Activates a Host-managed Tenant using an encrypted TenantId.
    /// The current Host role must grant the request ModuleId and OperationId.
    /// </summary>
    /// <param name="requestDTO">The Tenant activation request.</param>
    /// <returns>The activated Tenant response.</returns>
    [Authorize]
    [HttpPost("activate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateTenantAsync([FromBody] ActivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new ActivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Deactivates a Host-managed Tenant using an encrypted TenantId.
    /// The current Host role must grant the request ModuleId and OperationId.
    /// </summary>
    /// <param name="requestDTO">The Tenant deactivation request.</param>
    /// <returns>The deactivated Tenant response.</returns>
    [Authorize]
    [HttpPost("deactivate-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateTenantAsync([FromBody] DeactivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new DeactivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a Host-managed Tenant using an encrypted TenantId.
    /// The current Host role must grant the request ModuleId and OperationId.
    /// </summary>
    /// <param name="requestDTO">The Tenant deletion request.</param>
    /// <returns>The deletion result.</returns>
    [Authorize]
    [HttpPost("delete-tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTenantAsync([FromBody] DeleteTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(
            new DeleteHostManagedTenantCommand(requestDTO.TenantId, requestDTO));
        return Ok(result);
    }

    #endregion

    #region Existing Host User Command

    /// <summary>
    /// Creates a Host user through the existing Host user flow.
    /// </summary>
    /// <param name="tenantCreateRequestDTO">The Host user creation request.</param>
    /// <returns>The result returned by the existing Host user creation command.</returns>
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
    /// Retrieves Tenants associated with a subscription plan through the existing configuration query.
    /// </summary>
    /// <param name="code">The existing subscription-plan Tenant request.</param>
    /// <returns>The existing subscription-plan Tenant response.</returns>
    [HttpGet("get-all-tenant-by-subscription-plan-Id")]
    public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] application.DTOs.Tenant.TenantRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetAllTenantBySubscriptionPlanIdQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the existing employee-code pattern for a Tenant.
    /// Tenant callers remain token-scoped. Host callers require a Host token, an encrypted <c>TenantId</c>, and a runtime-granted <c>ModuleId</c>/<c>OperationId</c> pair.
    /// </summary>
    /// <param name="code">The employee-code pattern request.</param>
    /// <returns>The employee-code pattern response.</returns>
    [HttpGet("get-employee-code-pattern")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmployeeCodePatternAsync([FromQuery] EmployeeCodePatternRequestDTO code)
    {
        _logger.LogInfo("Fetching employee code pattern for tenant.");

        var query = new GetEmployeeCodePatternQuery(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the enabled module operations for a Tenant through the existing configuration command.
    /// </summary>
    /// <param name="code">The enabled-module request.</param>
    /// <returns>The enabled module operations response.</returns>
    [HttpPost("get")]
    public async Task<IActionResult> GetAllTenantEnabledModuleOperationsByTenantIdAsync([FromBody] TenantEnabledModuleRequestDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetTenantEnabledModuleCommand(code);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the existing Tenant operations configuration.
    /// </summary>
    /// <param name="code">The enabled operations request.</param>
    /// <returns>The Tenant operations configuration response.</returns>
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
    /// Updates the existing Tenant module and operation configuration.
    /// </summary>
    /// <param name="code">The Tenant module and operation update request.</param>
    /// <returns>The Tenant module and operation update response.</returns>
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
    /// Verifies a Tenant email address through the existing verification flow.
    /// </summary>
    /// <param name="request">The email verification request.</param>
    /// <returns>The email verification result.</returns>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
    {
        var command = new VerifyEmailCommand(request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    #endregion
}
