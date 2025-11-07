using axionpro.application.DTOs.Module;

using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Module.SubModule;
 
 
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Features.ModuleCmd.SubModule.Handlers;
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
    public class SubModuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public SubModuleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Create Module

        /// <summary>
        /// Creates a Sub module.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddModule([FromBody] CreateSubModuleRequestDTO? createModuleRequestDTO)
        {
            if (createModuleRequestDTO == null)
            {
                _logger.LogWarn("Received null request for AddModule.");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Module data is required." });
            }

            var command = new CreateSubModuleRequestCommand(createModuleRequestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new sub-module.
        /// </summary>
        //[HttpPost("create-sub-module")]
        //public async Task<IActionResult> AddSubModule([FromBody] Create? createSubModuleRequestDTO)
        //{
        //    if (createSubModuleRequestDTO == null)
        //    {
        //        _logger.LogWarn("Received null request for AddSubModule.");
        //        return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Sub-module data is required." });
        //    }

        //    var command = new CreateSubModuleCommand(createSubModuleRequestDTO);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //    {
        //        return Unauthorized(result);
        //    }

        //    return Ok(result);
        //}

        #endregion

        

        
    }
}
