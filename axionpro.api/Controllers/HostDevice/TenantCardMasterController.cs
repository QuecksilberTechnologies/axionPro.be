using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Host-only physical card procurement inventory for a selected Tenant.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantCardMasterController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a Host-owned physical card inventory item so a Tenant can later assign an approved card to an employee device user.</summary>
    [HttpPost("create")] public async Task<IActionResult> Create(CreateTenantCardMasterRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new CreateTenantCardMasterCommand(dto), ct));
    /// <summary>Reads one Host card inventory item with its number masked, so procurement data can be edited without exposing the credential.</summary>
    [HttpGet("get-by-id/{id}")] public async Task<IActionResult> GetById(string id, [FromQuery] TenantDeviceAccessRequestDTO access, CancellationToken ct) => Ok(await mediator.Send(new GetTenantCardMasterByIdQuery(id, access), ct));
    /// <summary>Lists Host card inventory for selected-Tenant procurement and availability screens; it is the source for tenant card binding selection.</summary>
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] TenantCardMasterFilterRequestDTO filter, CancellationToken ct) => Ok(await mediator.Send(new GetTenantCardMastersQuery(filter), ct));
    /// <summary>Updates procurement and tax information for an unassigned card; assigned cards are deliberately protected from editing.</summary>
    [HttpPost("update")] public async Task<IActionResult> Update(UpdateTenantCardMasterRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new UpdateTenantCardMasterCommand(dto), ct));
    /// <summary>Activates or deactivates an unassigned card so only eligible inventory can be bound to employees.</summary>
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus(UpdateTenantCardMasterStatusRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new UpdateTenantCardMasterStatusCommand(dto), ct));
    /// <summary>Soft-deletes an unassigned card when it should no longer appear in Host inventory; active device assignments block deletion.</summary>
    [HttpDelete("delete/{id}")] public async Task<IActionResult> Delete(string id, [FromQuery] TenantDeviceAccessRequestDTO access, CancellationToken ct) => Ok(await mediator.Send(new DeleteTenantCardMasterCommand(id, access), ct));
}
