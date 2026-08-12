
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


        #region TIMMY HTTPS Connectivity Test

        /// <summary>
        /// Receives temporary, unauthenticated HTTPS requests from a TIMMY biometric device.
        /// This action logs the device payload only; it does not process or persist attendance data.
        /// </summary>
        /// <returns>A TIMMY protocol acknowledgement for supported test commands.</returns>
        [AllowAnonymous]
        [HttpPost("timmy-test")]
        public async Task<IActionResult> TimmyTest()
        {
            string rawJson = string.Empty;

            try
            {
                // Read and log the unmodified body so the vendor test can be verified end-to-end.
                using var reader = new StreamReader(Request.Body);
                rawJson = await reader.ReadToEndAsync();

                _logger.LogInfo($"TIMMY COMPLETE INCOMING JSON: {rawJson}");
                Console.WriteLine("========================================");
                Console.WriteLine("TIMMY DEVICE REQUEST RECEIVED");
                Console.WriteLine(rawJson);
                Console.WriteLine("========================================");

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    const string emptyBodyMessage = "TIMMY request body is empty.";
                    _logger.LogError(emptyBodyMessage);
                    Console.WriteLine(emptyBodyMessage);

                    return Ok(new
                    {
                        result = false,
                        reason = "Empty request body."
                    });
                }

                using var jsonDocument = JsonDocument.Parse(rawJson);
                var root = jsonDocument.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    const string invalidPayloadMessage = "TIMMY request JSON must contain an object.";
                    _logger.LogError(invalidPayloadMessage);
                    Console.WriteLine(invalidPayloadMessage);

                    return Ok(new
                    {
                        result = false,
                        reason = "Invalid JSON payload."
                    });
                }

                if (!root.TryGetProperty("cmd", out var commandProperty))
                {
                    const string missingCommandMessage = "TIMMY request command is missing.";
                    _logger.LogError($"{missingCommandMessage} Complete JSON: {rawJson}");
                    Console.WriteLine(missingCommandMessage);

                    return Ok(new
                    {
                        result = false,
                        reason = "Command not found."
                    });
                }

                var command = commandProperty.ValueKind == JsonValueKind.String
                    ? commandProperty.GetString()?.Trim().ToLowerInvariant()
                    : commandProperty.ToString().Trim().ToLowerInvariant();
                var cloudTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                switch (command)
                {
                    case "reg":
                    {
                        var serialNumber = root.TryGetProperty("sn", out var snProperty)
                            ? snProperty.ToString()
                            : "Not provided";

                        _logger.LogInfo($"TIMMY device registration request received. Device SN : {serialNumber}");
                        Console.WriteLine($"TIMMY REGISTRATION RECEIVED | Device SN : {serialNumber}");

                        return Ok(new
                        {
                            ret = "reg",
                            result = true,
                            cloudtime = cloudTime,
                            nosenduser = false
                        });
                    }

                    case "sendlog":
                    {
                        var serialNumber = root.TryGetProperty("sn", out var snProperty)
                            ? snProperty.ToString()
                            : "Not provided";
                        var count = root.TryGetProperty("count", out var countProperty)
                            ? countProperty.ToString()
                            : "Not provided";
                        var logIndex = root.TryGetProperty("logindex", out var logIndexProperty)
                            ? logIndexProperty.ToString()
                            : "Not provided";
                        var acknowledgementCount = int.TryParse(count, out var countValue)
                            ? countValue
                            : 0;
                        var acknowledgementLogIndex = int.TryParse(logIndex, out var logIndexValue)
                            ? logIndexValue
                            : 0;
                        var recordJson = root.TryGetProperty("record", out var recordProperty)
                            ? recordProperty.GetRawText()
                            : "Not provided";

                        _logger.LogInfo("========================================");
                        _logger.LogInfo("TIMMY ATTENDANCE RECEIVED");
                        _logger.LogInfo($"Device SN : {serialNumber}");
                        _logger.LogInfo($"Count     : {count}");
                        _logger.LogInfo($"LogIndex  : {logIndex}");
                        _logger.LogInfo($"Record    : {recordJson}");
                        _logger.LogInfo("========================================");

                        Console.WriteLine("========================================");
                        Console.WriteLine("TIMMY ATTENDANCE RECEIVED");
                        Console.WriteLine($"Device SN : {serialNumber}");
                        Console.WriteLine($"Count     : {count}");
                        Console.WriteLine($"LogIndex  : {logIndex}");
                        Console.WriteLine($"Record    : {recordJson}");
                        Console.WriteLine("========================================");

                        // Intentionally log-only: do not map, call a repository, or save attendance data.
                        return Ok(new
                        {
                            ret = "sendlog",
                            result = true,
                            count = acknowledgementCount,
                            logindex = acknowledgementLogIndex,
                            cloudtime = cloudTime,
                            access = 1
                        });
                    }

                    case "checklive":
                    {
                        var serialNumber = root.TryGetProperty("sn", out var snProperty)
                            ? snProperty.ToString()
                            : "Not provided";

                        _logger.LogInfo($"TIMMY checklive request received. Device SN : {serialNumber}");
                        Console.WriteLine($"TIMMY CHECKLIVE RECEIVED | Device SN : {serialNumber}");

                        return Ok(new
                        {
                            ret = "checklive",
                            result = true,
                            cloudtime = cloudTime
                        });
                    }

                    default:
                    {
                        _logger.LogWarn($"Unknown TIMMY command received: {command ?? "Not provided"}. Complete JSON: {rawJson}");
                        Console.WriteLine($"UNKNOWN TIMMY COMMAND RECEIVED | Command: {command ?? "Not provided"}");

                        return Ok(new
                        {
                            ret = command,
                            result = false,
                            reason = "Command not supported in test endpoint."
                        });
                    }
                }
            }
            catch (JsonException exception)
            {
                _logger.LogError($"Malformed TIMMY JSON received. Complete body: {rawJson}. Exception: {exception}");
                Console.WriteLine($"TIMMY TEST MALFORMED JSON ERROR: {exception}");

                return Ok(new
                {
                    result = false,
                    reason = "Malformed JSON."
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"TIMMY test endpoint error. Complete body: {rawJson}. Exception: {exception}");
                Console.WriteLine($"TIMMY TEST ERROR: {exception}");

                return Ok(new
                {
                    result = false,
                    reason = "Server error."
                });
            }
        }

        #endregion


    }
}
