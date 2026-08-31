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
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/auth/login.</para>
        //         /// </remarks>
        //
        //
        //         [HttpPost("login")]
        //         public async Task<IActionResult> Login([FromBody] LoginRequestDTO logindto)
        //          {
        //             _logger.LogInfo("Received login request for user: {LoginId}" + logindto.LoginId.ToString());
        //             var command = new LoginCommand(logindto);
        //             var result = await _mediator.Send(command);
        //             if (!result.IsSucceeded)
        //             {
        //                 return Unauthorized(result);
        //             }
        //            return Ok(result);
        //         }
        #endregion
        /// <summary>
        /// Used-In-Angular: performs the Angular function refresh token.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.refreshToken (app/core/services/auth-api.ts:178).</para>
        /// <para>Angular purpose: performs the Angular function refresh token.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): TokenRefreshService (app/core/services/token-refresh-service.ts); authInterceptor (app/core/interceptors/auth-interceptor.ts); appConfig (app/app.config.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: updates login password.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.updateLoginPassword (app/core/services/auth-api.ts:207).</para>
        /// <para>Angular purpose: updates login password.</para>
        /// <para>Integrated UI page(s): /auth/registration-password; /app/update-password</para>
        /// <para>Angular UI component(s): RegistrationPassword (app/features/authentication/registration/registration-password/registration-password.ts); UpdatePassword (app/features/user-menu/update-password/update-password.ts)</para>
        /// </remarks>

        [HttpPost("update-login-password")]
                
        
        public async Task<IActionResult> SetLoginPassword([FromBody] UpdatePasswordRequestDTO request)
        {
             
                var command = new UpdateLoginPasswordCommand(request);
               var result = await _mediator.Send(command);
            return Ok(result);
            

        }
        /// <summary>
        /// Used-In-Angular: performs the Angular function resend credential.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.resendCredential (app/core/services/auth-api.ts:220).</para>
        /// <para>Angular purpose: performs the Angular function resend credential.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): Employees (app/features/employees/employees.ts)</para>
        /// </remarks>
        [HttpPost("resend-credential")]        
        public async Task<IActionResult> CreateNewLoginPasswordURL([FromBody] SetNewPasswordLinkRequestDTO request)
        {
            

                var command = new GetNewLoginPasswordURLCommand(request);

                var result = await _mediator.Send(command);

              
                return Ok(result);
            
        }
        /// <summary>
        /// Used-In-Angular: creates new login password.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.createNewLoginPassword (app/core/services/auth-api.ts:214).</para>
        /// <para>Angular purpose: creates new login password.</para>
        /// <para>Integrated UI page(s): /auth/reset-password; /auth/set-password</para>
        /// <para>Angular UI component(s): ResetPassword (app/features/authentication/reset-password/reset-password.ts); SetPassword (app/features/authentication/set-password/set-password.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: performs the Angular function forgot password.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.forgotPassword (app/core/services/auth-api.ts:186).</para>
        /// <para>Angular purpose: performs the Angular function forgot password.</para>
        /// <para>Integrated UI page(s): /auth/forgot-password; /auth/verify-otp</para>
        /// <para>Angular UI component(s): ForgotPassword (app/features/authentication/forgot-password/forgot-password.ts); VerifyOtp (app/features/authentication/verify-otp/verify-otp.ts)</para>
        /// </remarks>


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
        /// Used-In-Angular: validates forgot pass otp.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AuthApi.validateForgotPassOtp (app/core/services/auth-api.ts:193).</para>
        /// <para>Angular purpose: validates forgot pass otp.</para>
        /// <para>Integrated UI page(s): /auth/verify-otp</para>
        /// <para>Angular UI component(s): VerifyOtp (app/features/authentication/verify-otp/verify-otp.ts)</para>
        /// </remarks>


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

 
