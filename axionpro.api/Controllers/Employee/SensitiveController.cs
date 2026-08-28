// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Sensitive operations.
// ================================================================


using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee Personal & Related operations.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class SensitiveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public SensitiveController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

         /// <summary>
         /// Supports the Angular UI flow for createpersonalinfo.
         /// </summary>
         /// <remarks>
         /// <para>Angular purpose: creates employee sensitive.</para>
         /// <para>Angular page(s): /app/profile/identity-info.</para>
         /// <para>Angular API service call(s): EmployeeIdentityApi.createEmployeeSensitive (app/core/services/employee-identity-api.ts:72).</para>
         /// </remarks>
        
         [HttpPost("Create")]         
        public async Task<IActionResult> Createpersonalinfo([FromForm] CreateEmployeeIdentityRequestDTO dto)
        {
         
                var command = new CreateIdentityInfoCommand(dto);
                _logger.LogInfo("Creating new employee personal info info...");

                var result = await _mediator.Send(command);
                return Ok(result);          
           
        }

        /// <summary>
        /// Supports the Angular UI flow for get sensitive data.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee identities.</para>
        /// <para>Angular page(s): /app/profile/identity-info.</para>
        /// <para>Angular API service call(s): EmployeeIdentityApi.getEmployeeIdentities (app/core/services/employee-identity-api.ts:66).</para>
        /// </remarks>
        [HttpGet("get")]        
        public async Task<IActionResult> GetSensitiveData([FromQuery] GetIdentityRequestDTO commandDto)
        {
                var command = new GetIdentityInfoQuery(commandDto);
                var result = await _mediator.Send(command);
                               
                    return Ok(result);             
           
           
        }
        /// <summary>
        /// Updates employee details.
        /// </summary>
        //[HttpPost("update")]
        //
        //
        //
        //public async Task<IActionResult> Update([FromBody] GenricUpdateRequestDTO dto)
        //{
        //    try
        //    {
        //        _logger.LogInfo($"Updating employee-personal info record. EmployeeId: {dto._EmployeeId}");

        //        var command = new UpdateIdentityInfoCommand(dto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo($"Failed to update employee-personal info with Id: {dto._EmployeeId}");
        //            return BadRequest(result);
        //        }

        //        _logger.LogInfo("Employee-personal info updated successfully.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating employee-personal info: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-personal info info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        //}
  
    
    }
}
