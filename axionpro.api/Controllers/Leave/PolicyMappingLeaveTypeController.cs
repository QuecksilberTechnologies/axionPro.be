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
             /// Used-In-Angular: assigns or maps policy mapping leave type.
             /// </summary>
             /// <remarks>
             /// <para>Angular usage status: Used-In-Angular.</para>
             /// <para>API endpoint purpose: creates policy leave type mapping.</para>
             /// <para>Handler flow: CreatePolicyLeaveTypeMappingCommand is processed by CreatePolicyLeaveTypeMappingCommandHandler; operation(s): CreateLeavePolicyAsync.</para>
             /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveTypeWithPolicyMappingResponseDTO: Id (long), TenantId (long), PolicyTypeName (string), LeaveTypeId (int), LeaveTypeName (string), EmployeeTypeId (int), IsEmployeeMapped (bool?), EmployeeTypeName (string?), ApplicableGenderId (int?), IsMarriedApplicable (bool?), TotalLeavesPerYear (int), MonthlyAccrual (bool), CarryForward (bool), Encashable (bool), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), Remark (string?)</para>
             /// <para>Angular function(s): PolicyMappingLeaveTypeApi.mapPolicyMappingLeaveType (app/core/services/policy-mapping-leave-type-api.ts:83).</para>
             /// <para>Angular purpose: assigns or maps policy mapping leave type.</para>
             /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
             /// <para>Angular UI component(s): LeaveTypeMappingDialog (app/features/policies/policy-mapping-leave-type/leave-type-mapping-dialog/leave-type-mapping-dialog.ts)</para>
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
        /// Used-In-Angular: retrieves all policy mapping leave types.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves all leave policy.</para>
        /// <para>Handler flow: GetAllLeavePolicyQuery is processed by GetAllTicketTypeByModuleIdQueryHandler; operation(s): GetAllLeavePolicyByTenantIdAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveTypeWithPolicyMappingResponseDTO: Id (long), TenantId (long), PolicyTypeName (string), LeaveTypeId (int), LeaveTypeName (string), EmployeeTypeId (int), IsEmployeeMapped (bool?), EmployeeTypeName (string?), ApplicableGenderId (int?), IsMarriedApplicable (bool?), TotalLeavesPerYear (int), MonthlyAccrual (bool), CarryForward (bool), Encashable (bool), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), Remark (string?)</para>
        /// <para>Angular function(s): PolicyMappingLeaveTypeApi.getAllPolicyMappingLeaveTypes (app/core/services/policy-mapping-leave-type-api.ts:50).</para>
        /// <para>Angular purpose: retrieves all policy mapping leave types.</para>
        /// <para>Integrated UI page(s): /app/policies/leave-policy-type-mapping</para>
        /// <para>Angular UI component(s): PolicyMappingLeaveType (app/features/policies/policy-mapping-leave-type/policy-mapping-leave-type.ts)</para>
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
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves all policy leave type by emp type id.</para>
                /// <para>Handler flow: GetAllPolicyLeaveTypeByEmpTypeIdQuery is processed by GetAllPolicyLeaveTypeByEmpTypeIdQueryHandler; operation(s): GetAllLeavePolicyByEmployeeTypeIdAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveTypeWithPolicyMappingResponseDTO: Id (long), TenantId (long), PolicyTypeName (string), LeaveTypeId (int), LeaveTypeName (string), EmployeeTypeId (int), IsEmployeeMapped (bool?), EmployeeTypeName (string?), ApplicableGenderId (int?), IsMarriedApplicable (bool?), TotalLeavesPerYear (int), MonthlyAccrual (bool), CarryForward (bool), Encashable (bool), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), Remark (string?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/policymappingleavetype/leavepolicy/employeetype/get.</para>
                /// </remarks>
                [HttpGet("LeavePolicy/EmployeeType/get")]
                public async Task<IActionResult> GetAllLeavePoliciesByEmployeeIdAsync([FromQuery] GetPolicyLeaveTypeByEmpTypeIdRequestDTO dTO)
                {
                    _logger.LogInformation("Fetching all LeavePolicies...");
                    var query = new GetAllPolicyLeaveTypeByEmpTypeIdQuery(dTO);
                    var result = await _mediator.Send(query);
                    return Ok(result);
                }

        /// <summary>
        /// Used-In-Angular: updates policy mapping leave type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates employee leave policy map.</para>
        /// <para>Handler flow: UpdateEmployeeLeavePolicyMapCommand is processed by UpdateEmployeeLeavePolicyMapCommandHandler; operation(s): UpdateEmployeeLeaveMap, UpdateLeaveAssignOnlyAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): PolicyMappingLeaveTypeApi.updatePolicyMappingLeaveType (app/core/services/policy-mapping-leave-type-api.ts:95).</para>
        /// <para>Angular purpose: updates policy mapping leave type.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
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
        /// Used-In-Angular: deletes policy mapping leave type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes leave policy.</para>
        /// <para>Handler flow: DeleteLeavePolicyCommand is dispatched from the controller; no matching handler class was statically resolved.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): PolicyMappingLeaveTypeApi.deletePolicyMappingLeaveType (app/core/services/policy-mapping-leave-type-api.ts:107).</para>
        /// <para>Angular purpose: deletes policy mapping leave type.</para>
        /// <para>Integrated UI page(s): /app/policies/leave-policy-type-mapping</para>
        /// <para>Angular UI component(s): PolicyMappingLeaveType (app/features/policies/policy-mapping-leave-type/policy-mapping-leave-type.ts)</para>
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

