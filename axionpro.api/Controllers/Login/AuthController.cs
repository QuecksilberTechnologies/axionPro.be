using axionpro.application.DTOs.PageTypeEnum;
using axionpro.application.DTOs.UserLogin;
 
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
 
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Login
{
    // UserLoginController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration
      
        public AuthController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
       

        [HttpPost("login")]
      
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO logindto)
        {
            _logger.LogInfo("Received login request for user: {LoginId}" + logindto.LoginId.ToString());
            var command = new LoginCommand(logindto);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
           return Ok(result);
        }
        //[HttpPost("AccessDetails")]
        //[Authorize] // Ensures the user is authenticated via token
        //public async Task<IActionResult> UserAccessDetailsAsync([FromBody] AccessDetailRequestDTO accessDetailsDTO)
        //{
        //    try
        //    {
        //        // Log the request
        //     //  _logger.LogInformation("Accessing AccessDetail for user: {EmployeeId}", accessDetailsDTO.EmployeeId);

        //        // Validate input
        //        if (accessDetailsDTO == null || accessDetailsDTO.EmployeeId <= 0)
        //        {
        //          //  _logger.LogWarning("Invalid request data provided for AccessDetail.");
        //            return BadRequest(new { Message = "Invalid request data." });
        //        }

        //        // Create and send the command
        //        var command = new EmployeeTypeBasicMenuCommand(accessDetailsDTO);
        //        var result = await _mediator.Send(command);

        //        // Check the result of the command
        //        if (!result.IsSucceeded)
        //        {
        //          //  _logger.LogWarning("AccessDetail retrieval failed for EmployeeId: {EmployeeId}", accessDetailsDTO.EmployeeId);
        //            return Unauthorized(result);
        //        }

        //        // Success response
        //      //  _logger.LogInformation("AccessDetail successfully retrieved for EmployeeId: {EmployeeId}", accessDetailsDTO.EmployeeId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error
        //       // _logger.LogError(ex, "An error occurred while processing AccessDetail for EmployeeId: {EmployeeId}", accessDetailsDTO?.EmployeeId);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing the request." });
        //    }
        //}


        //[HttpPost("accessrolepermissions")]
        //[Authorize] // Ensures the user is authenticated via token
        //public async Task<IActionResult> UserAccessRolesAsync([FromBody] AccessDetailRequestDTO accessDetailsDTO)
        //{
        //    try
        //    {
        //        // Log the request
        //        //  _logger.LogInformation("Accessing AccessDetail for user: {EmployeeId}", accessDetailsDTO.EmployeeId);

        //        // Validate input
        //        if (accessDetailsDTO == null || accessDetailsDTO.EmployeeId <= 0)
        //        {
        //            //  _logger.LogWarning("Invalid request data provided for AccessDetail.");
        //            return BadRequest(new { Message = "Invalid request data." });
        //        }

        //        // Create and send the command
        //        var command = new UserRolesPermissionOnModuleCommand(accessDetailsDTO);
        //        var result = await _mediator.Send(command);

        //        // Check the result of the command
        //        if (!result.IsSucceeded)
        //        {
        //            //  _logger.LogWarning("AccessDetail retrieval failed for EmployeeId: {EmployeeId}", accessDetailsDTO.EmployeeId);
        //            return Unauthorized(result);
        //        }

        //        // Success response
        //        //  _logger.LogInformation("AccessDetail successfully retrieved for EmployeeId: {EmployeeId}", accessDetailsDTO.EmployeeId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error
        //        // _logger.LogError(ex, "An error occurred while processing AccessDetail for EmployeeId: {EmployeeId}", accessDetailsDTO?.EmployeeId);
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing the request." });
        //    }
        //}


        // ...

        [HttpPost("update-login-password")] 
        public async Task<IActionResult> SetLoginPassword([FromBody] LoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiResponse<UpdateLoginPasswordResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "LoginId and Password are required.",
                        Data = null
                    });
                }

                var command = new UpdateLoginPasswordCommand(request);
                

                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError( "Exception occurred while setting login password.");

                return StatusCode(500, new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Internal server error occurred.",
                    Data = null
                });
            }




        }

        //[HttpGet("get-page-type")]        
        //public async Task<IActionResult> GetPageTypes([FromQuery] PageTypeEnumRequestDTO request)
        //{
        //    try
        //    {
        //        // 🔁 Static method ko direct call kar rahe hain
        //        var result = StaticPageTypeData.GetSamplePageTypes();

        //        if (result == null || !result.Any())
        //            return NotFound("❌ No Page Types found for the provided criteria.");

        //        return Ok(result); // ✅ Return 200 with data
        //    }
        //    catch (Exception ex)
        //    {
        //      //  _logger.LogError(ex, "❌ Error fetching PageTypes for TenantId {TenantId}", request.EmployeeId);
        //        return StatusCode(500, "An error occurred while fetching page types.");
        //    }
        //}


        [HttpPost("forgot-password")]
        public async Task<IActionResult> EnterLoginId([FromBody] ForgotPasswordUserIdRequestDTO request)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.p))
                //{
                //    return BadRequest(new ApiResponse<ForgotPasswordResponseDTO>
                //    {
                //        IsSucceeded = false,
                //        Message = "LoginId and not found required.",
                //        Data = null
                //    });
                //}

                var command = new ForgotPasswordCommand(request);


                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while setting login password.");

                return StatusCode(500, new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Internal server error occurred.",
                    Data = null
                });
            }




        }

        [HttpPost("set-login-new-password")]
        public async Task<IActionResult> ValidateForgotPasswordOtp([FromBody] NewLoginPasswordRequestDTO request)
        {
            try
            {

                var command = new SetNewLoginPasswordCommand(request);


                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while setting login password.");

                return StatusCode(500, new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Internal server error occurred.",
                    Data = null
                });
            }




        }


        [HttpPost("validate-forgot-password-otp")]
        public async Task<IActionResult> ValidateForgotPasswordOtp([FromBody] ValidateOtpRequestDTO request)
        {
            try
            {
              
                var command = new ValidateOtpCommand(request);


                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while setting login password.");

                return StatusCode(500, new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Internal server error occurred.",
                    Data = null
                });
            }




        }

        //...


    }
}

 
