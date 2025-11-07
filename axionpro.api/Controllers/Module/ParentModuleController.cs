using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Features.AssetFeatures.Assets.Commands;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
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
    public class ParentModuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ParentModuleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Create Module

        /// <summary>
        /// Creates a new main module.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddModule([FromBody] CreateParentModuleRequestDTO? createModuleRequestDTO)
        {
            if (createModuleRequestDTO == null)
            {
                _logger.LogWarn("Received null request for AddModule.");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Module data is required." });
            }

            var command = new CreateParentModuleRequestCommand(createModuleRequestDTO);
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

        #region Update Module

        /// <summary>
        /// Updates an existing main module.
        /// </summary>
        [HttpPost("update-module")]
        public async Task<IActionResult> UpdateModule([FromBody] UpdateAssetRequestDTO updateAssetDTO)
        {
            if (updateAssetDTO == null)
            {
                _logger.LogWarn("Received null request for UpdateModule.");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Update data is required." });
            }

            // _logger.LogInfo("Received request to update module: {@UpdateAssetDTO}", updateAssetDTO);


            var command = new UpdateAssetCommand(updateAssetDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing sub-module.
        /// </summary>
        [HttpPost("update-sub-module")]
        public async Task<IActionResult> UpdateSubModule([FromBody] UpdateAssetRequestDTO updateAssetDTO)
        {
            if (updateAssetDTO == null)
            {
                _logger.LogWarn("Received null request for UpdateSubModule.");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Update data is required." });
            }

            // _logger.LogInfo("Received request to update sub-module: {0}", updateAssetDTO.ToString());
            var command = new UpdateAssetCommand(updateAssetDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #endregion

        #region Get Modules



        /// <summary>
        /// Get operational (leaf node) modules.
        /// </summary>
        [HttpPost("get-non-leafe")]
        public async Task<IActionResult> GetOperationalModule([FromBody] GetModuleChildInversRequestDTO? getModuleDDLRequestDTO)
        {
            if (getModuleDDLRequestDTO == null)
            {
                _logger.LogWarn("Received null request for GetOperationalModule (leaf modules).");
                return BadRequest(new { IsSucceeded = false, Message = "Invalid request. Filter data is required." });
            }

            var command = new GetModuleHeadersCommand(getModuleDDLRequestDTO);
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
