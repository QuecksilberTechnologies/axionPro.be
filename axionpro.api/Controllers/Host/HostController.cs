using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostCmd.Handler;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Host
{
    /// <summary>
    /// handled-Tenant-related-operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    public HostController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;  // Logger service ko inject karna
    }

        [HttpPost("create-host-user")]


        // [Authorize]
        public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
        {
            _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
            var command = new CreateHostUserCommand(tenantCreateRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpPost("create-host-role")]
        // [Authorize]
        public async Task<IActionResult> CreateHostRole( [FromBody] CreateHostRoleRequestDTO hostRoleRequestDTO)
        {
            _logger.LogInfo(
                "Received request to create a new Host Role.");

            var command = new CreateHostRoleCommand(hostRoleRequestDTO);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}
