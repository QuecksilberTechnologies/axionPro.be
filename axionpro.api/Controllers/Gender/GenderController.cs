using axionpro.api.Controllers.Leave;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.GenderCmd.Queries;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Gender
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GenderController> _logger;

        public GenderController(IMediator mediator, ILogger<GenderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        

        //  ✅ Get All Gender 
        [HttpGet("get")]
        public async Task<IActionResult> GetAllGenderAsync([FromQuery] GetGenderRequestDTO? getGenderRequestDTO)
        {
            _logger.LogInformation("Fetching all LeavePolicies...");
            var query = new GetAllGenderQuery(getGenderRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
         

    }
}
