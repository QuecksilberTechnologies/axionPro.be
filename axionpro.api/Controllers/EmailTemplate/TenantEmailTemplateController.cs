using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Features.TenantEmailTemplateCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmailTemplate;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantEmailTemplateController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a template owned by the authenticated Tenant.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantEmailTemplateRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new CreateTenantEmailTemplateCommand(dto),
            cancellationToken));
    }

    /// <summary>Returns the authenticated Tenant's paged template list.</summary>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] TenantEmailTemplateListRequestDTO? filter,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetAllTenantEmailTemplatesQuery(filter ?? new TenantEmailTemplateListRequestDTO()),
            cancellationToken));
    }

    /// <summary>Returns one template from the authenticated Tenant.</summary>
    [HttpGet("get-by-id/{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        [FromQuery] PermissionRequestDTO permissionRequest,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetTenantEmailTemplateByIdQuery(id, permissionRequest),
            cancellationToken));
    }

    /// <summary>Updates template content inside the authenticated Tenant.</summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateTenantEmailTemplateRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new UpdateTenantEmailTemplateCommand(dto),
            cancellationToken));
    }

    /// <summary>Activates or deactivates a Tenant template.</summary>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus(
        [FromBody] UpdateTenantEmailTemplateStatusRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new UpdateTenantEmailTemplateStatusCommand(dto),
            cancellationToken));
    }

    /// <summary>Deletes an inactive template from the authenticated Tenant.</summary>
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromQuery] PermissionRequestDTO permissionRequest,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new DeleteTenantEmailTemplateCommand(id, permissionRequest),
            cancellationToken));
    }
}
