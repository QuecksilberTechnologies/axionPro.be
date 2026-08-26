// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Registration operations.
// ================================================================

using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Registration
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController :  ControllerBase
    {
        private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    public RegistrationController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
        /// <summary>
        /// Login.
        /// </summary>
        /// <remarks>
        /// Handles the request to login.
        /// </remarks>
        /// <param name="candidateRegistrationDTO">The request body used to login.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("candidate")]       
        
        // [Authorize]
        public async Task<IActionResult> Login([FromBody] CandidateRequestDTO candidateRegistrationDTO)
        {
            _logger.LogInfo("Received request for register a new candidate" + candidateRegistrationDTO.ToString());
            var command = new CandidateRegistrationCommand(candidateRegistrationDTO);
            var result = await _mediator.Send(command);           
            return Ok(result);
        }
        /// <summary>
        /// User Access Details.
        /// </summary>
        /// <remarks>
        /// Handles the request to user access details.
        /// </remarks>
        /// <param name="accessDetailsDTO">The request body used to user access details.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

       
        [HttpPost("AccessDetails")]   
        // [Authorize] // Ensures the user is authenticated via token
        public async Task<IActionResult> UserAccessDetailsAsync([FromBody] AccessDetailRequestDTO accessDetailsDTO)
        { 

            // Create and send the command
            var command = new EmployeeTypeBasicMenuCommand(accessDetailsDTO);
            var result = await _mediator.Send(command);

            // Success response
            //  _logger.LogInformation("AccessDetail successfully retrieved for EmployeeId: {EmployeeId}", accessDetailsDTO.EmployeeId);
            return Ok(result);
        }
        
   
}
}
