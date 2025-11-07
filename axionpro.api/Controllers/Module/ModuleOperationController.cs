
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.ModuleOperation;
 
 
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleOperationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public ModuleOperationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #region Create Module

        [HttpPost("create")]
        //public async Task<IActionResult> UpdateModuleOperation([FromBody] ModuleOperationMappingRequestDTO? dto)
        //{
        //    if (dto == null)
        //    {
        //        // _logger.LogWarning("Received null request for getting Assets.");
        //        // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
        //    }

        //    // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

        //    var query = new CreateModuleOperationMappingByProductOwnerCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
        //    var result = await _mediator.Send(query);

        //    if (!result.IsSucceeded)
        //    {
        //        return Unauthorized(result);
        //    }

        //    return Ok(result);
        //}

        [HttpPost("update")]
        public async Task<IActionResult> AddModuleOperation([FromBody] UpdateModuleOperationMappingByProductOwnerRequestDTO? dto)
        {
            if (dto == null)
            {
                // _logger.LogWarning("Received null request for getting Assets.");
                // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
            }

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new UpdateModuleOperationMappingByProductOwnerCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }



        #endregion




    }
}

