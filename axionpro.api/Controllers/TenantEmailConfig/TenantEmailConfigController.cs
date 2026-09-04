// ================================================================
// Purpose : Exposes Host- and Tenant-authorized CRUD endpoints for Tenant SMTP configuration.
// ================================================================

using axionpro.application.DTOS.Configruations;
using axionpro.application.Features.TenantEmailConfigCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.TenantEmailConfig;

/// <summary>
/// Manages Tenant-specific SMTP settings. Host users supply the encrypted
/// Tenant identifier; Tenant users are automatically scoped to their own
/// Tenant. SMTP secrets are write-only and never included in API responses.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantEmailConfigController(
    IMediator mediator,
    ILogger<TenantEmailConfigController> logger) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantEmailConfigRequestDTO dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received Tenant email configuration create request.");
        return Ok(await mediator.Send(new CreateTenantEmailConfigCommand(dto), cancellationToken));
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] TenantEmailConfigAccessRequestDTO filter,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received Tenant email configuration list request.");
        return Ok(await mediator.Send(new GetAllTenantEmailConfigsQuery(filter), cancellationToken));
    }

    [HttpGet("get-by-id/{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        [FromQuery] TenantEmailConfigAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received Tenant email configuration read request for {TenantEmailConfigId}.", id);
        return Ok(await mediator.Send(new GetTenantEmailConfigByIdQuery(id, accessRequest), cancellationToken));
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateTenantEmailConfigRequestDTO dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received Tenant email configuration update request for {TenantEmailConfigId}.", dto.Id);
        return Ok(await mediator.Send(new UpdateTenantEmailConfigCommand(dto), cancellationToken));
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromQuery] TenantEmailConfigAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received Tenant email configuration delete request for {TenantEmailConfigId}.", id);
        return Ok(await mediator.Send(new DeleteTenantEmailConfigCommand(id, accessRequest), cancellationToken));
    }
}
