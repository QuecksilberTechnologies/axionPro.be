// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Rule operations.
// ================================================================


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
        /// <summary>
        /// Get All Day Combination By Tenant User.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all day combination by tenant user.
        /// </remarks>
        /// <param name="dTO">The request body used to get all day combination by tenant user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("/Sandwich/DayCombination/add")]
        public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] CreateDayCombinationRequestDTO dTO)
        {
         // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new CreateDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        #endregion
        #region Update--DAY-COMBINATION BY-TENANT-ADMIN
        /// <summary>
        /// Update Day Combination By Tenant User.
        /// </summary>
        /// <remarks>
        /// Handles the request to update day combination by tenant user.
        /// </remarks>
        /// <param name="dto">The request body used to update day combination by tenant user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("/Sandwich/DayCombination/update")]        
        public async Task<IActionResult> UpdateDayCombinationByTenantUser([FromBody] UpdateDayCombinationRequestDTO dto)
        {
            var query = new UpdateDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        #endregion
        #region Update-DAY-COMBINATION BY-TENANT-ADMIN
        /// <summary>
        /// Delete Day Combination By Tenant User.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete day combination by tenant user.
        /// </remarks>
        /// <param name="dto">The request body used to delete day combination by tenant user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("/Sandwich/DayCombination/delete")]                  
        public async Task<IActionResult> DeleteDayCombinationByTenantUser([FromBody] DeleteDayCombinationRequestDTO dto)
        {
                       // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new DeleteDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);                  

            return Ok(result);
        }
        #endregion
        /// <summary>
        /// Get All Day Combination By Tenant User.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all day combination by tenant user.
        /// </remarks>
        /// <param name="dTO">The request body used to get all day combination by tenant user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("/Sandwich/DayCombination/get")]        
        public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] GetDayCombinationRequestDTO dTO)
        {

            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new GetDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query); 

            return Ok(result);
        }

        #region CRUD-SANDWICH-RULE-BY-TENANT-ADMIN

        // 🔹 GET ALL
        /// <summary>
        /// Get All Sandwich Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all sandwich rule.
        /// </remarks>
        /// <param name="dto">The query parameters used to get all sandwich rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("/Sandwich/get")] 
        public async Task<IActionResult> GetAllSandwichRule([FromQuery] GetLeaveSandwitchRuleRequestDTO dto)
        { 

            var query = new GetSandwichRuleCommand(dto);
            var result = await _mediator.Send(query);  

            return Ok(result);
        }

        // 🔹 CREATE
        /// <summary>
        /// Create Sandwich Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to create sandwich rule.
        /// </remarks>
        /// <param name="dto">The request body used to create sandwich rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("/Sandwich/add")]        
        public async Task<IActionResult> CreateSandwichRule([FromBody] CreateLeaveSandwichRuleRequestDTO dto)
        {   

            var command = new CreateSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // 🔹 UPDATE
        /// <summary>
        /// Update Sandwich Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to update sandwich rule.
        /// </remarks>
        /// <param name="dto">The request body used to update sandwich rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("/Sandwich/update")]        
        public async Task<IActionResult> UpdateSandwichRule([FromBody] UpdateLeaveSandwitchRuleRequestDTO dto)
        {
           

            var command = new UpdateSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);


            return Ok(result);
        }

        // 🔹 DELETE (Soft Delete)
        /// <summary>
        /// Delete Sandwich Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete sandwich rule.
        /// </remarks>
        /// <param name="dto">The query parameters used to delete sandwich rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("/Sandwich/delete")]        
        public async Task<IActionResult> DeleteSandwichRule([FromQuery] DeleteLeaveSandwitchRuleRequestDTO dto)
        {

            var command = new DeleteSandwichRuleCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #endregion

    }

}
 
