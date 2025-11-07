
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
 
using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.SandwichRule
{
    /// <summary>
    /// handled-sandwich-related-operations.
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]


    public class RuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public RuleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #region CRUD-GET-DAY-COMBINATION BY-TENANT-ADMIN

        [HttpPost("/Sandwich/DayCombination/add")]
        public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] CreateDayCombinationRequestDTO dTO)
        {
            if (dTO == null)
            {
                // _logger.LogWarning("Received null request for getting Assets.");
                // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
            }

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new CreateDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        #endregion
        #region Update--DAY-COMBINATION BY-TENANT-ADMIN

        [HttpPost("/Sandwich/DayCombination/update")]
        public async Task<IActionResult> UpdateDayCombinationByTenantUser([FromBody] UpdateDayCombinationRequestDTO dto)
        {
            if (dto == null)
            {
                // _logger.LogWarning("Received null request for getting Assets.");
                // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
            }

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new UpdateDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        #endregion
        #region Update-DAY-COMBINATION BY-TENANT-ADMIN

        [HttpPost("/Sandwich/DayCombination/delete")]
        public async Task<IActionResult> DeleteDayCombinationByTenantUser([FromBody] DeleteDayCombinationRequestDTO dto)
        {
            if (dto == null)
            {
                // _logger.LogWarning("Received null request for getting Assets.");
                // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
            }

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new DeleteDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        #endregion

        [HttpPost("/Sandwich/DayCombination/get")]
        public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] GetDayCombinationRequestDTO dTO)
        {
            if (dTO == null)
            {
                // _logger.LogWarning("Received null request for getting Assets.");
                // return BadRequest(new ApiResponse<List<GetAllAssetDTO>>(false, "Invalid request", null));
            }

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new GetDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        #region CRUD-SANDWICH-RULE-BY-TENANT-ADMIN

        // 🔹 GET ALL
        [HttpGet("/Sandwich/get")]
        public async Task<IActionResult> GetAllSandwichRule([FromQuery] GetLeaveSandwitchRuleRequestDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "❌ Request body cannot be null."
                });
            }

            var query = new GetSandwichRuleCommand(dto);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // 🔹 CREATE
        [HttpPost("/Sandwich/add")]
        public async Task<IActionResult> CreateSandwichRule([FromBody] CreateLeaveSandwichRuleRequestDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "❌ Request body cannot be null."
                });
            }

            var command = new CreateSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // 🔹 UPDATE
        [HttpPost("/Sandwich/update")]
        public async Task<IActionResult> UpdateSandwichRule([FromBody] UpdateLeaveSandwitchRuleRequestDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid or missing SandwichRule Id."
                });
            }

            var command = new UpdateSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // 🔹 DELETE (Soft Delete)
        [HttpDelete("/Sandwich/delete")]
        public async Task<IActionResult> DeleteSandwichRule([FromQuery] DeleteLeaveSandwitchRuleRequestDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid or missing SandwichRule Id."
                });
            }

            var command = new DeleteSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

    }

}
 
