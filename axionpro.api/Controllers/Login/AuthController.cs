// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Auth operations.
// ================================================================

using axionpro.application.DTOs.PageTypeEnum;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.DTOS.UserLogin;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;

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
        /// <summary>
        /// Login.
        /// </summary>
        /// <remarks>
        /// Handles the request to login.
        /// </remarks>
        /// <param name="logindto">The request body used to login.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
       

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
        /// <summary>
        /// Refresh Token.
        /// </summary>
        /// <remarks>
        /// Handles the request to refresh token.
        /// </remarks>
        /// <param name="request">The request body used to refresh token.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("refresh-token")]    
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            var command = new RefreshTokenCommand(request);
            var result = await _mediator.Send(command);
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
        /// <summary>
        /// Set Login Password.
        /// </summary>
        /// <remarks>
        /// Handles the request to set login password.
        /// </remarks>
        /// <param name="request">The request body used to set login password.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("update-login-password")]
                
        
        public async Task<IActionResult> SetLoginPassword([FromBody] UpdatePasswordRequestDTO request)
        {
             
                var command = new UpdateLoginPasswordCommand(request);
               var result = await _mediator.Send(command);
            return Ok(result);
            

        }
        /// <summary>
        /// Create New Login Password URL.
        /// </summary>
        /// <remarks>
        /// Handles the request to create new login password url.
        /// </remarks>
        /// <param name="request">The request body used to create new login password url.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("resend-credential")]        
        public async Task<IActionResult> CreateNewLoginPasswordURL([FromBody] SetNewPasswordLinkRequestDTO request)
        {
            

                var command = new GetNewLoginPasswordURLCommand(request);

                var result = await _mediator.Send(command);

              
                return Ok(result);
            
        }
        /// <summary>
        /// Create Login Password.
        /// </summary>
        /// <remarks>
        /// Handles the request to create login password.
        /// </remarks>
        /// <param name="request">The request body used to create login password.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create-new-password")]                
        public async Task<IActionResult> CreateLoginPassword([FromBody] NewLoginPasswordRequestDTO request)
        {
           
                var command = new NewLoginPasswordCommand(request);

                var result = await _mediator.Send(command);              

                return Ok(result);
           
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
        /// <summary>
        /// Enter Login ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to enter login id.
        /// </remarks>
        /// <param name="request">The request body used to enter login id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


        [HttpPost("forgot-password")]       
        
        public async Task<IActionResult> EnterLoginId([FromBody] ForgotPasswordUserIdRequestDTO request)
        {
                var command = new ForgotPasswordCommand(request);
                var result = await _mediator.Send(command);         
                return Ok(result);
           
          
        }

        //[HttpPost("set-login-new-password")]
        //public async Task<IActionResult> ValidateForgotPasswordOtp([FromBody] ResetLoginPasswordRequestDTO request)
        //{
        //    try
        //    {

        //        var command = new ResetLoginPasswordCommand(request);


        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //            return BadRequest(result);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Exception occurred while setting login password.");

        //        return StatusCode(500, new ApiResponse<UpdatePasswordResponseDTO>
        //        {
        //            IsSucceeded = false,
        //            Message = "Internal server error occurred.",
        //            Data = null
        //        });
        //    }




        //}
        /// <summary>
        /// Validate Forgot Password Otp.
        /// </summary>
        /// <remarks>
        /// Handles the request to validate forgot password otp.
        /// </remarks>
        /// <param name="request">The request body used to validate forgot password otp.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


        [HttpPost("validate-forgot-password-otp")]    
        public async Task<IActionResult> ValidateForgotPasswordOtp([FromBody] ValidateOtpRequestDTO request)
        {
            
              
                var command = new ValidateOtpCommand(request);
                var result = await _mediator.Send(command);               

                return Ok(result);
           
        }

        //...


    }
}

 
