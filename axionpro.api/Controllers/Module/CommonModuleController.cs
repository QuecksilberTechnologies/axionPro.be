// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Common Module operations.
// ================================================================

using axionpro.application.DTOs.Module;

using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Features.ModuleCmd.Common.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
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
        /// Not-Used-In-Angular.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Not-Used-In-Angular.</para>
        /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        /// <para>Backend endpoint: POST /api/commonmodule/add.</para>
        /// </remarks>

        [HttpPost("add")]       
        public async Task<IActionResult> AddModule([FromBody] CreateCommonModuleRequestDTO? requestDto)
        {


            var command = new CreateCommonModuleCommand(requestDto);
            var result = await _mediator.Send(command);


            return Ok(result);
        }


        #endregion

    }
}
