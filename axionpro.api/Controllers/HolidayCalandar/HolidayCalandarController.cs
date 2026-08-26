// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Holiday Calandar operations.
// ================================================================

using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.OrganizationHolidayCalendar;

using axionpro.application.Features.HolidayCalandarCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HolidayCalandar
{
    /// <summary>
    /// handled-Holiday-Calandar-related-operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayCalandarController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration
        public HolidayCalandarController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;  // Logger service ko inject karna
        }
        /// <summary>
        /// Get All Employee Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee info.
        /// </remarks>
        /// <param name="basicRequestDTO">The query parameters used to get all employee info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpGet("get")]      
        public async Task<IActionResult> GetAllEmployeeInfo([FromQuery] BasicRequestDTO basicRequestDTO)
        {            
                var command = new GetHolidayCalandarQuery(basicRequestDTO);

                // ✅ Send command instead of DTO
                ApiResponse<List<OrganizationHolidayCalendarDTO>> result = await _mediator.Send(command);                
                    return Ok(result);
                
           
           
        }


    }
}
