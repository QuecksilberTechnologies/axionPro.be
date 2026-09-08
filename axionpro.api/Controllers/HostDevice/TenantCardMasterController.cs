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
    [HttpPost("create")] public async Task<IActionResult> Create(CreateTenantCardMasterRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new CreateTenantCardMasterCommand(dto), ct));
    [HttpGet("get-by-id/{id}")] public async Task<IActionResult> GetById(string id, [FromQuery] TenantDeviceAccessRequestDTO access, CancellationToken ct) => Ok(await mediator.Send(new GetTenantCardMasterByIdQuery(id, access), ct));
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] TenantCardMasterFilterRequestDTO filter, CancellationToken ct) => Ok(await mediator.Send(new GetTenantCardMastersQuery(filter), ct));
    [HttpPost("update")] public async Task<IActionResult> Update(UpdateTenantCardMasterRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new UpdateTenantCardMasterCommand(dto), ct));
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus(UpdateTenantCardMasterStatusRequestDTO dto, CancellationToken ct) => Ok(await mediator.Send(new UpdateTenantCardMasterStatusCommand(dto), ct));
    [HttpDelete("delete/{id}")] public async Task<IActionResult> Delete(string id, [FromQuery] TenantDeviceAccessRequestDTO access, CancellationToken ct) => Ok(await mediator.Send(new DeleteTenantCardMasterCommand(id, access), ct));
}
