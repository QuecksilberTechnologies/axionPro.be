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
        /// Supports the Angular UI flow for create leave rule async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates leave rule.</para>
        /// <para>Angular page(s): /app/leave/rules.</para>
        /// <para>Angular API service call(s): LeaveRule.createLeaveRule (app/core/services/leave-rule.ts:42).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get all leave rule async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: performs constructor.</para>
        /// <para>Angular page(s): No Angular component caller was statically resolved; the Angular API-service wrapper is documented below..</para>
        /// <para>Angular API service call(s): LeaveRule.constructor (app/core/services/leave-rule.ts:36).</para>
        /// </remarks>
        [HttpGet("get")]        
        public async Task<IActionResult> GetAllLeaveRuleAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        //  ✅ Get All leave rule sandwich 
        [HttpGet("LeaveRule/Sandwich/get")]       
        
        public async Task<IActionResult> GetAllLeaveRuleSandwichAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for update leave policy async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates leave rule.</para>
        /// <para>Angular page(s): /app/leave/rules.</para>
        /// <para>Angular API service call(s): LeaveRule.updateLeaveRule (app/core/services/leave-rule.ts:48).</para>
        /// </remarks>
        [HttpPost("update")]    
        
        public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateLeaveRuleRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to update LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateLeaveRuleCommand(requestDTO);
            var result = await _mediator.Send(command);         

            return Ok(result);
        }
        /// <summary>
        /// Supports the Angular UI flow for delete leave policy.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes leave rule.</para>
        /// <para>Angular page(s): /app/leave/rules; /app/leave/history; /app/leave/my; /app/leave/balances; /app/leave/inbox; /app/leave/status; /app/leave/types; /app/profile/leave-info; and 1 more.</para>
        /// <para>Angular API service call(s): LeaveRule.deleteLeaveRule (app/core/services/leave-rule.ts:54).</para>
        /// </remarks>
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
