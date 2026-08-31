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
        /// Used-In-Angular: creates leave rule.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LeaveRule.createLeaveRule (app/core/services/leave-rule.ts:43).</para>
        /// <para>Angular purpose: creates leave rule.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): LeaveRuleDialog (app/features/leaves/leave-requests/leave-rules/leave-rule-dialog/leave-rule-dialog.ts)</para>
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
        /// Used-In-Angular: retrieves all leave rules.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LeaveRule.getAllLeaveRules (app/core/services/leave-rule.ts:37).</para>
        /// <para>Angular purpose: retrieves all leave rules.</para>
        /// <para>Integrated UI page(s): /app/leave/history; /app/leave/my; /app/leave/balances; /app/leave/inbox; /app/leave/rules; /app/leave/status; /app/leave/types; /app/profile/leave-info</para>
        /// <para>Angular UI component(s): LeaveStore (app/features/leaves/leave.store.ts); LeaveHistory (app/features/user-menu/employee-profile/employee-leave-info/leave-history/leave-history.ts); LeaveManagement (app/features/leaves/leave-management/leave-management.ts); RequestLeaveDialog (app/features/leaves/leave-management/request-leave-dialog/request-leave-dialog.ts); LeaveBalances (app/features/leaves/leave-requests/leave-balances/leave-balances.ts); LeaveRequests (app/features/leaves/leave-requests/leave-requests.ts); LeaveRuleDialog (app/features/leaves/leave-requests/leave-rules/leave-rule-dialog/leave-rule-dialog.ts); LeaveRules (app/features/leaves/leave-requests/leave-rules/leave-rules.ts)</para>
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
        /// <summary>
        /// Not-Used-In-Angular.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Not-Used-In-Angular.</para>
        /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        /// <para>Backend endpoint: GET /api/leaverule/leaverule/sandwich/get.</para>
        /// </remarks>
        [HttpGet("LeaveRule/Sandwich/get")]       
        
        public async Task<IActionResult> GetAllLeaveRuleSandwichAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: updates leave rule.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LeaveRule.updateLeaveRule (app/core/services/leave-rule.ts:49).</para>
        /// <para>Angular purpose: updates leave rule.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): LeaveRuleDialog (app/features/leaves/leave-requests/leave-rules/leave-rule-dialog/leave-rule-dialog.ts)</para>
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
        /// Used-In-Angular: deletes leave rule.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LeaveRule.deleteLeaveRule (app/core/services/leave-rule.ts:55).</para>
        /// <para>Angular purpose: deletes leave rule.</para>
        /// <para>Integrated UI page(s): /app/leave/history; /app/leave/my; /app/leave/balances; /app/leave/inbox; /app/leave/rules; /app/leave/status; /app/leave/types; /app/profile/leave-info</para>
        /// <para>Angular UI component(s): LeaveStore (app/features/leaves/leave.store.ts); LeaveHistory (app/features/user-menu/employee-profile/employee-leave-info/leave-history/leave-history.ts); LeaveManagement (app/features/leaves/leave-management/leave-management.ts); RequestLeaveDialog (app/features/leaves/leave-management/request-leave-dialog/request-leave-dialog.ts); LeaveBalances (app/features/leaves/leave-requests/leave-balances/leave-balances.ts); LeaveRequests (app/features/leaves/leave-requests/leave-requests.ts); LeaveRuleDialog (app/features/leaves/leave-requests/leave-rules/leave-rule-dialog/leave-rule-dialog.ts); LeaveRules (app/features/leaves/leave-requests/leave-rules/leave-rules.ts)</para>
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
