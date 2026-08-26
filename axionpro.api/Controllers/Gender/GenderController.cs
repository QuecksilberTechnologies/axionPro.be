// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Gender operations.
// ================================================================

using axionpro.api.Controllers.Leave;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.application.Features.GenderCmd.Handlers;
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

        /// <summary>
        /// get Gender.
        /// </summary>
        /// <remarks>
        /// Handles the request to get gender.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get gender.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("option")]               
        public async Task<IActionResult> getGender([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInformation($"Received request to get Gender : {requestDTO.UserEmployeeId}");

            var command = new GetGenderOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        //  ✅ Get All Gender 
        /// <summary>
        /// Get All Gender.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all gender.
        /// </remarks>
        /// <param name="getGenderRequestDTO">The query parameters used to get all gender.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
