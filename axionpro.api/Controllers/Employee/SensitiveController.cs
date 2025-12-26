
using axionpro.application.DTOS.Employee.Sensitive;
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
        /// Create new employee personal info record.
        /// </summary>
       
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Createpersonalinfo([FromForm] CreateEmployeeIdentityRequestDTO dto)
        {
            try
            {
                var command = new CreateIdentityInfoCommand(dto);
                _logger.LogInfo("Creating new employee personal info info...");

                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ApiResponse<string>.Fail("Internal server error.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get all personal info records (Paginated).
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSensitiveData([FromQuery] GetIdentityRequestDTO commandDto)
        {
            try
            {
                var command = new GetIdentityInfoQuery(commandDto);
                var result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ApiResponse<string>.Fail("Internal server error.", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Updates employee details.
        /// </summary>
        //[HttpPost("update")]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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