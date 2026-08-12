
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
                #region Request Diagnostics

                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY TEST ENDPOINT HIT");
                Console.WriteLine($"Time           : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Method         : {Request.Method}");
                Console.WriteLine($"Path           : {Request.Path}");
                Console.WriteLine($"Content-Type   : {Request.ContentType}");
                Console.WriteLine($"Content-Length : {Request.ContentLength}");
                Console.WriteLine($"QueryString    : {Request.QueryString}");
                Console.WriteLine("========================================");

                _logger.LogInfo(
                    $"TIMMY TEST ENDPOINT HIT | Method: {Request.Method} | " +
                    $"Path: {Request.Path} | Content-Type: {Request.ContentType} | " +
                    $"Content-Length: {Request.ContentLength}");

                #endregion


                #region Read Raw Request

                using var reader = new StreamReader(Request.Body);
                var rawJson = await reader.ReadToEndAsync();

                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY RAW REQUEST RECEIVED");
                Console.WriteLine(rawJson);
                Console.WriteLine("========================================");

                _logger.LogInfo(
                    "TIMMY RAW REQUEST RECEIVED : " + rawJson);

                #endregion


                var cloudTime = DateTime.UtcNow
                    .ToString("yyyy-MM-dd HH:mm:ss");


                #region Temporary Initial Registration ACK

                /*
                 * TEMPORARY TEST BEHAVIOUR:
                 *
                 * Vendor device currently reaches AxionPro over HTTPS,
                 * but the initial request has been observed as an empty body
                 * or "{}".
                 *
                 * During this connectivity test only, return the TIMMY
                 * registration acknowledgement so we can verify whether the
                 * physical device completes the registration handshake.
                 *
                 * REMOVE this fallback before production implementation.
                 */

                if (string.IsNullOrWhiteSpace(rawJson) ||
                    rawJson.Trim() == "{}")
                {
                    Console.WriteLine("========================================");
                    Console.WriteLine("TIMMY EMPTY INITIAL REQUEST RECEIVED");
                    Console.WriteLine("RETURNING TEMPORARY REG ACK");
                    Console.WriteLine("========================================");

                    _logger.LogInfo(
                        "TIMMY initial empty request received. " +
                        "Returning temporary REG acknowledgement.");

                    return Ok(new
                    {
                        ret = "reg",
                        result = true,
                        cloudtime = cloudTime,
                        nosenduser = true
                    });
                }

                #endregion


                #region Parse JSON

                using var jsonDocument = JsonDocument.Parse(rawJson);
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("cmd", out var cmdProperty))
                {
                    Console.WriteLine("========================================");
                    Console.WriteLine("TIMMY CMD MISSING");
                    Console.WriteLine($"RAW JSON : {rawJson}");
                    Console.WriteLine("RETURNING TEMPORARY REG ACK");
                    Console.WriteLine("========================================");

                    _logger.LogInfo(
                        "TIMMY cmd missing. Returning temporary REG acknowledgement. " +
                        "Raw JSON: " + rawJson);

                    return Ok(new
                    {
                        ret = "reg",
                        result = true,
                        cloudtime = cloudTime,
                        nosenduser = true
                    });
                }

                var command = cmdProperty
                    .GetString()?
                    .Trim()
                    .ToLowerInvariant();

                #endregion


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
                        && countProperty.TryGetInt32(out var parsedCount)
                            ? parsedCount
                            : 0;

                    var logIndex = root.TryGetProperty("logindex", out var logIndexProperty)
                        && logIndexProperty.TryGetInt32(out var parsedLogIndex)
                            ? parsedLogIndex
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

                    _logger.LogInfo(
                        $"TIMMY ATTENDANCE RECEIVED. " +
                        $"SN: {sn}, Count: {count}, LogIndex: {logIndex}");

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

                    Console.WriteLine("========================================");
                    Console.WriteLine("TIMMY CHECKLIVE RECEIVED");
                    Console.WriteLine($"SN : {sn}");
                    Console.WriteLine("========================================");

                    _logger.LogInfo(
                        "TIMMY CHECKLIVE RECEIVED. SN : " + sn);

                    return Ok(new
                    {
                        ret = "checklive",
                        result = true,
                        cloudtime = cloudTime
                    });
                }

                #endregion


                #region Unsupported Command

                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY UNSUPPORTED COMMAND");
                Console.WriteLine($"Command : {command}");
                Console.WriteLine($"Raw JSON: {rawJson}");
                Console.WriteLine("========================================");

                _logger.LogInfo(
                    $"TIMMY unsupported command received. " +
                    $"Command: {command}. Raw JSON: {rawJson}");

                return Ok(new
                {
                    ret = command,
                    result = false,
                    reason = "Unsupported command"
                });

                #endregion
            }
            catch (JsonException ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY INVALID JSON");
                Console.WriteLine(ex);
                Console.WriteLine("========================================");

                _logger.LogError(
                    
                    "Invalid JSON received from TIMMY biometric device.");

                return Ok(new
                {
                    result = false,
                    reason = "Invalid JSON"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY TEST ERROR");
                Console.WriteLine(ex);
                Console.WriteLine("========================================");

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
