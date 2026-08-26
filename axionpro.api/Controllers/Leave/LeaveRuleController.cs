// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Leave Rule operations.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
 
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Features.LeaveRuleCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Leave
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaveRuleController> _logger;
        public LeaveRuleController(IMediator mediator, ILogger<LeaveRuleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ✅ Create PolicyMappingLeaveType
        /// <summary>
        /// Create Leave Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to create leave rule.
        /// </remarks>
        /// <param name="requestDTO">The request body used to create leave rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]        
        public async Task<IActionResult> CreateLeaveRuleAsync([FromBody] CreateLeaveRuleDTORequest requestDTO)
        {
            _logger.LogInformation("Received request to create PolicyMappingLeaveType: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new CreateLeaveRuleCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //  ✅ Get All LeavePolicies
        /// <summary>
        /// Get All Leave Rule.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all leave rule.
        /// </remarks>
        /// <param name="getLeavePolicyRequestDTO">The query parameters used to get all leave rule.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]        
        public async Task<IActionResult> GetAllLeaveRuleAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        //  ✅ Get All leave rule sandwich 
        /// <summary>
        /// Get All Leave Rule Sandwich.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all leave rule sandwich.
        /// </remarks>
        /// <param name="getLeavePolicyRequestDTO">The query parameters used to get all leave rule sandwich.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("LeaveRule/Sandwich/get")]       
        
        public async Task<IActionResult> GetAllLeaveRuleSandwichAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Update Leave Policy.
        /// </summary>
        /// <remarks>
        /// Handles the request to update leave policy.
        /// </remarks>
        /// <param name="requestDTO">The request body used to update leave policy.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]    
        
        public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateLeaveRuleRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to update LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateLeaveRuleCommand(requestDTO);
            var result = await _mediator.Send(command);         

            return Ok(result);
        }
        /// <summary>
        /// Delete Leave Policy.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete leave policy.
        /// </remarks>
        /// <param name="request">The request body used to delete leave policy.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("delete")]      
        
        // [Authorize]
        public async Task<IActionResult> DeleteLeavePolicy([FromBody] DeleteLeaveRuleDTO request)
        {            
            _logger.LogInformation("Received request to delete Leave Rule Type Id: {Id} by UserId: {UserId}", request.Id, request.UserId);
            var command = new DeleteLeaveRuleCommand(request);
            var result = await _mediator.Send(command);
            _logger.LogInformation("Successfully deleted Leave Rule Id: {Id}", request.Id);
            return Ok(result);
        }
    }
}
