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
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/compliancerule/update.</para>
        //         /// </remarks>
        //
        //
        //         [HttpPost("update")]
        //         public async Task<IActionResult> UpdateComplianceRuleyAsync([FromBody] UpdateLeaveBalanceToEmployeeRequestDTO requestDTO)
        //         {
        //             _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
        //             var command = new UpdateLeaveBalanceCommand(requestDTO);
        //             var result = await _mediator.Send(command);
        //
        //             return Ok(result);
        //         }
        #endregion

    }


}
