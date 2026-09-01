// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Employee Leave Policy operations.
// ================================================================



using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Leave
{

    [Route("api/[controller]")]
    public class EmployeeLeavePolicyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PolicyMappingLeaveTypeController> _logger;
        public EmployeeLeavePolicyController(IMediator mediator, ILogger<PolicyMappingLeaveTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        // ✅ Map EmployeeLeavePolicyMapping
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: creates leave balance.</para>
                /// <para>Handler flow: AddLeaveBalanceCommand is processed by AddLeaveBalanceCommandHandler; operation(s): UpdateIsLeaveBalanceAssigned, AddLeaveBalanceToEmployee.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeLeavePolicyMappingReponseDTO: Id (long), TenantId (long), EmployeeId (long), PolicyLeaveTypeMappingId (long), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), IsLeaveBalanceAssigned (bool?), LeaveName (string?), LeaveTypeId (int?), AddedById (long), AddedDateTime (DateTime)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/employeeleavepolicy/add.</para>
                /// </remarks>
                [HttpPost("add")]

                public async Task<IActionResult> MapEmployeeyAsync([FromBody] AddLeaveBalanceToEmployeeRequestDTO requestDTO)
                {
                    _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
                    var command = new AddLeaveBalanceCommand(requestDTO);
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }
        // ✅ Map EmployeeLeavePolicyMapping
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: updates leave balance.</para>
                /// <para>Handler flow: UpdateLeaveBalanceCommand is processed by UpdateLeaveBalanceCommandHandler; operation(s): UpdateLeaveBalanceToEmployee.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveBalanceToEmployeeResponseDTO: Id (long), TenantId (long), EmployeeLeavePolicyMappingId (long?), LeaveYear (int?), OpeningBalance (decimal?), Availed (decimal?), CurrentBalance (decimal?), CarryForwarded (decimal?), Encashed (decimal?), LeavesOnHold (decimal?), IsActive (bool?), IsAllBalanceOnHold (bool?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/employeeleavepolicy/leavebalance/update.</para>
                /// </remarks>
                [HttpPost("LeaveBalance/update")]
                public async Task<IActionResult> UpdateEmployeeyAsync([FromBody] UpdateLeaveBalanceToEmployeeRequestDTO requestDTO)
                {
                    _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
                    var command = new UpdateLeaveBalanceCommand(requestDTO);
                    var result = await _mediator.Send(command);

                    return Ok(result);
                }

        // ✅ Map EmployeeLeavePolicyMapping
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: performs the Angular function employee leave policy map.</para>
                /// <para>Handler flow: EmployeeLeavePolicyMapCommand is processed by EmployeeLeavePolicyMapCommandHandler; operation(s): UpdateLeaveAssignOnlyAsync, CreateEmployeeLeaveMapAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeLeavePolicyMappingReponseDTO: Id (long), TenantId (long), EmployeeId (long), PolicyLeaveTypeMappingId (long), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), IsLeaveBalanceAssigned (bool?), LeaveName (string?), LeaveTypeId (int?), AddedById (long), AddedDateTime (DateTime)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/employeeleavepolicy/map.</para>
                /// </remarks>
                [HttpPost("map")]
                public async Task<IActionResult> MapEmployeeyAsync([FromBody] CreateEmployeeLeavePolicyMappingRequestDTO requestDTO)
                {
                    _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
                    var command = new EmployeeLeavePolicyMapCommand(requestDTO);
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }

        //  ✅ Get All Employee mapped EmployeeLeavePolicyMapping
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves all leave policy.</para>
                /// <para>Handler flow: GetAllLeavePolicyQuery is processed by GetAllTicketTypeByModuleIdQueryHandler; operation(s): GetAllLeavePolicyByTenantIdAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveTypeWithPolicyMappingResponseDTO: Id (long), TenantId (long), PolicyTypeName (string), LeaveTypeId (int), LeaveTypeName (string), EmployeeTypeId (int), IsEmployeeMapped (bool?), EmployeeTypeName (string?), ApplicableGenderId (int?), IsMarriedApplicable (bool?), TotalLeavesPerYear (int), MonthlyAccrual (bool)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/employeeleavepolicy/mapped/leave/policy/get.</para>
                /// </remarks>
                [HttpGet("Mapped/Leave/Policy/get")]
                public async Task<IActionResult> GetAllLeavePoliciesAsync([FromQuery] GetLeaveTypeWithPolicyMappingRequestDTO getLeavePolicyRequestDTO)
                {
                    _logger.LogInformation("Fetching all Employee Mapped...");
                    var query = new GetAllLeavePolicyQuery(getLeavePolicyRequestDTO);
                    var result = await _mediator.Send(query);
                    return Ok(result);
                }
        //  ✅ Get All Employee mapped EmployeeLeavePolicyMapping
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves all employee leave policy.</para>
                /// <para>Handler flow: GetAllEmployeeLeavePolicyQuery is processed by GetAllEmployeeLeavePolicyQueryHandler; operation(s): GetAllEmployeeLeaveMap.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeLeavePolicyMappingReponseDTO: Id (long), TenantId (long), EmployeeId (long), PolicyLeaveTypeMappingId (long), EffectiveFrom (DateTime), EffectiveTo (DateTime?), IsActive (bool), IsLeaveBalanceAssigned (bool?), LeaveName (string?), LeaveTypeId (int?), AddedById (long), AddedDateTime (DateTime)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/employeeleavepolicy/employeeleavepolicy/mapped/get.</para>
                /// </remarks>
                [HttpGet("EmployeeLeavePolicy/Mapped/get")]
                public async Task<IActionResult> GetAllEmployeeLeavePoliciesAsync([FromQuery] GetEmployeeLeavePolicyMappingRequestDTO requestDTO)
                {
                    _logger.LogInformation("Fetching all Employee Mapped...");
                    var query = new GetAllEmployeeLeavePolicyQuery(requestDTO);
                    var result = await _mediator.Send(query);
                    return Ok(result);
                }
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: updates employee leave policy map.</para>
                /// <para>Handler flow: UpdateEmployeeLeavePolicyMapCommand is processed by UpdateEmployeeLeavePolicyMapCommandHandler; operation(s): UpdateEmployeeLeaveMap, UpdateLeaveAssignOnlyAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/employeeleavepolicy/update.</para>
                /// </remarks>

                [HttpPost("update")]

                public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateEmployeeLeavePolicyMappingRequestDTO requestDTO)
                {
                    _logger.LogInformation("Received request to update LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
                    var command = new UpdateEmployeeLeavePolicyMapCommand(requestDTO);
                    var result = await _mediator.Send(command);

                    return Ok(result);
                }
        // ✅ Delete EmployeeLeavePolicyMapping (Soft Delete)
        //[HttpPost("delete")]
        //// [Authorize]
        //public async Task<IActionResult> DeleteLeavePolicy([FromBody] DeletePolicyLeaveTypeMappingRequestDTO request)
        //{
        //    if (request == null)
        //    {
        //        _logger.LogWarning("DeleteLeave request is null.");
        //        return BadRequest(new ApiResponse<bool>
        //        {
        //            IsSucceeded = false,
        //            Message = "Invalid request data.",
        //            Data = false
        //        });
        //    }

        //    _logger.LogInformation("Received request to EmployeeLeavePolicyMapping LeaveType Id: {Id} by UserId: {UserId}", request.Id, request.UserId);

        //    var command = new DeleteLeavePolicyCommand(request);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //    {
        //        _logger.LogWarning("Failed to delete EmployeeLeavePolicyMapping Id: {Id}", request.Id);
        //        return BadRequest(result);
        //    }

        //    _logger.LogInformation("Successfully deleted EmployeeLeavePolicyMapping Id: {Id}", request.Id);
        //    return Ok(result);
        //}


    }
}
