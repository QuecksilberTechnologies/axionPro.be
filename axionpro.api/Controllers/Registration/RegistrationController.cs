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

        [HttpPost("candidate")]       
        
        // [Authorize]
        public async Task<IActionResult> Login([FromBody] CandidateRequestDTO candidateRegistrationDTO)
        {
            _logger.LogInfo("Received request for register a new candidate" + candidateRegistrationDTO.ToString());
            var command = new CandidateRegistrationCommand(candidateRegistrationDTO);
            var result = await _mediator.Send(command);           
            return Ok(result);
        }

       
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
