
// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Provides attendance API endpoints and a temporary TIMMY HTTPS
//               connectivity test endpoint.
// ============================================================================

using axionpro.application.DTOs.Attendance;
using axionpro.application.DTOs.UserLogin;
//using axionpro.application.Features.AttendanceCmd.Command;
 
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;

using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


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


        [HttpPost("markattendance")]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceRequestDTO? attendanceRequestDTO)
        {
            //_logger.LogInfo("Received mark attendance request for user: {LoginId}" + attendanceRequestDTO.LoginId.ToString());
            //  var command = new AttendanceCommand(attendanceRequestDTO);
            //var result = await _mediator.Send(command);
            //if (!result.IsSuccecced)
            //{
            //    return Unauthorized(result);
            //}
            //return Ok(result);
            return null;
        }


        #region TIMMY HTTPS Test

        /// <summary>
        /// Temporary endpoint used only to verify direct HTTPS communication
        /// from the TIMMY biometric device.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("timmy-test")]
        public async Task<IActionResult> TimmyTest()
        {
            try
            {
                // IMPORTANT:
                // Log immediately when the controller action is reached.
                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY TEST ENDPOINT HIT");
                Console.WriteLine($"Time : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine("========================================");

                _logger.LogInfo("TIMMY TEST ENDPOINT HIT");

                using var reader = new StreamReader(Request.Body);
                var rawJson = await reader.ReadToEndAsync();

                Console.WriteLine("TIMMY RAW REQUEST:");
                Console.WriteLine(rawJson);

                _logger.LogInfo(
                    "TIMMY RAW REQUEST RECEIVED : " + rawJson);

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return Ok(new
                    {
                        result = false,
                        reason = "Empty request body"
                    });
                }

                using var jsonDocument = JsonDocument.Parse(rawJson);
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("cmd", out var cmdProperty))
                {
                    return Ok(new
                    {
                        result = false,
                        reason = "cmd not found"
                    });
                }

                var command = cmdProperty.GetString()?.ToLowerInvariant();

                var cloudTime = DateTime.UtcNow
                    .ToString("yyyy-MM-dd HH:mm:ss");

                #region Register

                if (command == "reg")
                {
                    var sn = root.TryGetProperty("sn", out var snProperty)
                        ? snProperty.GetString()
                        : null;

                    Console.WriteLine("========================================");
                    Console.WriteLine("TIMMY REG RECEIVED");
                    Console.WriteLine($"SN : {sn}");
                    Console.WriteLine("========================================");

                    _logger.LogInfo(
                        "TIMMY REG RECEIVED. SN : " + sn);

                    return Ok(new
                    {
                        ret = "reg",
                        result = true,
                        cloudtime = cloudTime,

                        // Follow TIMMY protocol example during initial handshake.
                        nosenduser = true
                    });
                }

                #endregion

                #region Attendance

                if (command == "sendlog")
                {
                    var sn = root.TryGetProperty("sn", out var snProperty)
                        ? snProperty.GetString()
                        : null;

                    var count = root.TryGetProperty("count", out var countProperty)
                        ? countProperty.GetInt32()
                        : 0;

                    var logIndex = root.TryGetProperty("logindex", out var logIndexProperty)
                        ? logIndexProperty.GetInt32()
                        : 0;

                    Console.WriteLine("========================================");
                    Console.WriteLine("TIMMY ATTENDANCE RECEIVED");
                    Console.WriteLine($"SN       : {sn}");
                    Console.WriteLine($"Count    : {count}");
                    Console.WriteLine($"LogIndex : {logIndex}");

                    if (root.TryGetProperty("record", out var recordProperty))
                    {
                        Console.WriteLine("Record:");
                        Console.WriteLine(recordProperty.GetRawText());
                    }

                    Console.WriteLine("========================================");

                    return Ok(new
                    {
                        ret = "sendlog",
                        result = true,
                        count,
                        logindex = logIndex,
                        cloudtime = cloudTime,
                        access = 1,
                        message = "OK"
                    });
                }

                #endregion

                #region Heartbeat

                if (command == "checklive")
                {
                    var sn = root.TryGetProperty("sn", out var snProperty)
                        ? snProperty.GetString()
                        : null;

                    Console.WriteLine(
                        $"TIMMY CHECKLIVE RECEIVED. SN : {sn}");

                    return Ok(new
                    {
                        ret = "checklive",
                        result = true,
                        cloudtime = cloudTime
                    });
                }

                #endregion

                return Ok(new
                {
                    ret = command,
                    result = false,
                    reason = "Unsupported command"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("TIMMY TEST ERROR:");
                Console.WriteLine(ex);

                _logger.LogError(
                    
                    "Error while processing TIMMY test request.");

                return Ok(new
                {
                    result = false,
                    reason = "Server error"
                });
            }
        }

        #endregion

    }
}
