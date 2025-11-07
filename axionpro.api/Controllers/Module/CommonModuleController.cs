using axionpro.application.DTOs.Module;

using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Features.ModuleCmd.Common.Commands;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module
{
    /// <summary>
    /// Handles all module-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommonModuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public CommonModuleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Create Module

        /// <summary>
        /// Creates a new Common module.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddModule([FromBody] CreateCommonModuleRequestDTO? requestDto)
        {
            if (requestDto == null)
            {
                _logger.LogWarn("Received null request for AddModule.");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Module data is required." });
            }

            var command = new CreateCommonModuleCommand(requestDto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }


        #endregion

    }
}
