// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Compliance Rule operations.
// ================================================================

using axionpro.api.Controllers.Leave;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Compliance
{

    [Route("api/[controller]")]
    public class ComplianceRuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ComplianceRuleController> _logger;
        public ComplianceRuleController(IMediator mediator, ILogger<ComplianceRuleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: updates leave balance.</para>
                /// <para>Handler flow: UpdateLeaveBalanceCommand is processed by UpdateLeaveBalanceCommandHandler; operation(s): UpdateLeaveBalanceToEmployee.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveBalanceToEmployeeResponseDTO: Id (long), TenantId (long), EmployeeLeavePolicyMappingId (long?), LeaveYear (int?), OpeningBalance (decimal?), Availed (decimal?), CurrentBalance (decimal?), CarryForwarded (decimal?), Encashed (decimal?), LeavesOnHold (decimal?), IsActive (bool?), IsAllBalanceOnHold (bool?), AddedById (long?), UpdatedById (long?), AddedDateTime (DateTime?), UpdatedDateTime (DateTime?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/compliancerule/update.</para>
                /// </remarks>


                [HttpPost("update")]
                public async Task<IActionResult> UpdateComplianceRuleyAsync([FromBody] UpdateLeaveBalanceToEmployeeRequestDTO requestDTO)
                {
                    _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
                    var command = new UpdateLeaveBalanceCommand(requestDTO);
                    var result = await _mediator.Send(command);

                    return Ok(result);
                }

    }


}
