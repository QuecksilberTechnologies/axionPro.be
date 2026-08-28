// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Policy Mapping Leave Type operations.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Leave
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyMappingLeaveTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PolicyMappingLeaveTypeController> _logger;

        public PolicyMappingLeaveTypeController(IMediator mediator, ILogger<PolicyMappingLeaveTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        // ✅ Create LeavePolicy
             /// <summary>
             /// Supports the Angular UI flow for create leave policy async.
             /// </summary>
             /// <remarks>
             /// <para>Angular purpose: updates policy mapping leave types.</para>
             /// <para>Angular page(s): /app/policies/leave-policy-type-mapping.</para>
             /// <para>Angular API service call(s): PolicyMappingLeaveTypeApi.setPolicyMappingLeaveTypes (app/core/services/policy-mapping-leave-type-api.ts:79).</para>
             /// </remarks>
             [HttpPost("map")]         
        public async Task<IActionResult> CreateLeavePolicyAsync([FromBody] GetPolicyLeaveTypeMappingRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to create LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new CreatePolicyLeaveTypeMappingCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

       //  ✅ Get All LeavePolicies
        /// <summary>
        /// Supports the Angular UI flow for get all leave policies async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates policy mapping leave types.</para>
        /// <para>Angular page(s): /app/policies/leave-policy-type-mapping.</para>
        /// <para>Angular API service call(s): PolicyMappingLeaveTypeApi.setPolicyMappingLeaveTypes (app/core/services/policy-mapping-leave-type-api.ts:46).</para>
        /// </remarks>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllLeavePoliciesAsync([FromQuery]  GetLeaveTypeWithPolicyMappingRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all LeavePolicies...");
            var query = new GetAllLeavePolicyQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //  ✅ Get All LeavePolicies
        [HttpGet("LeavePolicy/EmployeeType/get")] 
        public async Task<IActionResult> GetAllLeavePoliciesByEmployeeIdAsync([FromQuery] GetPolicyLeaveTypeByEmpTypeIdRequestDTO dTO)
        {
            _logger.LogInformation("Fetching all LeavePolicies...");
            var query = new GetAllPolicyLeaveTypeByEmpTypeIdQuery(dTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for update leave policy async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates policy mapping leave types.</para>
        /// <para>Angular page(s): /app/policies/leave-policy-type-mapping.</para>
        /// <para>Angular API service call(s): PolicyMappingLeaveTypeApi.setPolicyMappingLeaveTypes (app/core/services/policy-mapping-leave-type-api.ts:91).</para>
        /// </remarks>
        [HttpPost("update")]       
        public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to update EmployeeLeavePolicyMap: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateEmployeeLeavePolicyMapCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Supports the Angular UI flow for delete leave policy.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates policy mapping leave types.</para>
        /// <para>Angular page(s): /app/policies/leave-policy-type-mapping.</para>
        /// <para>Angular API service call(s): PolicyMappingLeaveTypeApi.setPolicyMappingLeaveTypes (app/core/services/policy-mapping-leave-type-api.ts:103).</para>
        /// </remarks>
        [HttpPost("delete")]        
        // [Authorize]
        public async Task<IActionResult> DeleteLeavePolicy([FromQuery] DeletePolicyLeaveTypeMappingRequestDTO request)
        {
            
            var command = new DeleteLeavePolicyCommand(request);
            var result = await _mediator.Send(command);
             _logger.LogInformation("Successfully deleted LeaveType Id: {Id}", request.UserId);
            return Ok(result);
        }


        //// ✅ Delete LeavePolicy (Soft Delete)
        //[HttpDelete("delete/{id:long}")]
        //public async Task<IActionResult> DeleteLeavePolicyAsync(long id, [FromQuery] long userId)
        //{
        //    _logger.LogInformation("Received request to delete LeavePolicy with Id {Id} by UserId {UserId}", id, userId);
        //    var command = new DeleteLeavePolicyCommand(id, userId);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //        return BadRequest(result);

        //    return Ok(result);
        //}
    }
}

