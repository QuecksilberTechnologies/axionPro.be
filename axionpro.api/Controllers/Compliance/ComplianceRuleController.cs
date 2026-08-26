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
        /// Update Compliance Ruley.
        /// </summary>
        /// <remarks>
        /// Handles the request to update compliance ruley.
        /// </remarks>
        /// <param name="requestDTO">The request body used to update compliance ruley.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


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
