// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes Tenant creation, configuration, verification, and Host-side management endpoints.
// ================================================================

using axionpro.application.DTOs.Tenant;
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
using MediatR;
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
    /// Retrieves Tenant records for Host-side management using optional status, verification, search, and paging filters.
    /// </summary>
    /// <param name="requestDTO">The Tenant management filter and paging request.</param>
    /// <returns>The Tenant management list response.</returns>
    [HttpGet("get-all-tenants")]
    public async Task<IActionResult> GetAllTenantsAsync([FromQuery] GetAllTenantsRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetAllTenantsQuery(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a Tenant by its authoritative long identifier for Host-side management.
    /// </summary>
    /// <param name="requestDTO">The Tenant identifier request.</param>
    /// <returns>The requested Tenant response.</returns>
    [HttpGet("get-tenant-by-id")]
    public async Task<IActionResult> GetTenantByIdAsync([FromQuery] GetTenantByIdRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(requestDTO));
        return Ok(result);
    }

    #endregion

    #region Tenant Management Commands

    /// <summary>
    /// Updates the editable Tenant details supplied by a Host-side management request.
    /// </summary>
    /// <param name="requestDTO">The editable Tenant details request.</param>
    /// <returns>The updated Tenant response.</returns>
    [HttpPost("update-tenant")]
    public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new UpdateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Requests activation of a Tenant. Future handling will also activate its matching login credentials atomically.
    /// </summary>
    /// <param name="requestDTO">The Tenant activation request.</param>
    /// <returns>The activated Tenant response.</returns>
    [HttpPost("activate-tenant")]
    public async Task<IActionResult> ActivateTenantAsync([FromBody] ActivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new ActivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Requests deactivation of a Tenant. Future handling will also deactivate its matching login credentials atomically.
    /// </summary>
    /// <param name="requestDTO">The Tenant deactivation request.</param>
    /// <returns>The deactivated Tenant response.</returns>
    [HttpPost("deactivate-tenant")]
    public async Task<IActionResult> DeactivateTenantAsync([FromBody] DeactivateTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new DeactivateTenantCommand(requestDTO));
        return Ok(result);
    }

    /// <summary>
    /// Requests the future soft deletion of a Tenant independently from deactivation.
    /// </summary>
    /// <param name="requestDTO">The Tenant deletion request.</param>
    /// <returns>The deletion result.</returns>
    [HttpPost("delete-tenant")]
    public async Task<IActionResult> DeleteTenantAsync([FromBody] DeleteTenantRequestDTO requestDTO)
    {
        var result = await _mediator.Send(new DeleteTenantCommand(requestDTO));
        return Ok(result);
    }

    #endregion

    #region Existing Host User Command

    /// <summary>
    /// Creates a Host user through the existing Host user flow.
    /// </summary>
    /// <param name="tenantCreateRequestDTO">The Host user creation request.</param>
    /// <returns>The result returned by the existing Host user creation command.</returns>
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
    /// </summary>
    /// <param name="code">The employee-code pattern request.</param>
    /// <returns>The employee-code pattern response.</returns>
    [HttpGet("get-employee-code-pattern")]
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
