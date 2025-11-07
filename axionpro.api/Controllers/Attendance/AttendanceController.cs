 
using axionpro.application.DTOs.UserLogin;
//using axionpro.application.Features.AttendanceCmd.Command;
 
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;

using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Attendance
{
    // UserLoginController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public AttendanceController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        //[HttpPost("markattendance")]
        //public async Task<IActionResult> MarkAttendance([FromBody] AttendanceRequestDTO? attendanceRequestDTO)
        //{
        //    _logger.LogInfo("Received mark attendance request for user: {LoginId}" + attendanceRequestDTO.LoginId.ToString());
        //   // var command = new AttendanceCommand(attendanceRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


    }
}
