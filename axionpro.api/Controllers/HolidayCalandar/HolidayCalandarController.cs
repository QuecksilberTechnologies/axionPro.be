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
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves holiday calandar.</para>
                /// <para>Handler flow: GetHolidayCalandarQuery is processed by GetHolidayCalandarQueryHandler; operation(s): GetAllHolidaysAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); OrganizationHolidayCalendarDTO: TenantId (long), StateCode (string?), HolidayYear (int), HolidayName (string), HolidayDate (DateTime)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/holidaycalandar/get.</para>
                /// </remarks>

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
