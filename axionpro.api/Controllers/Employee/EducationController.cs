using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee Education & Related operations.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public EducationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create new employee education record.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateEmployee([FromForm] CreateEducationRequestDTO dto)

        {
            try
            {
                var command = new CreateEducationInfoCommand(dto);
                _logger.LogInfo("📩 Creating new employee education info...");

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
        /// Get all education records (Paginated).
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEmployeeInfo([FromQuery] GetEducationRequestDTO commandDto)
        {
            try
            {
                var command = new GetEducationInfoQuery(commandDto);
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
        [HttpPost("update-education")]      

        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateEducation(UpdateEducationRequestDTO dto)

        {
            try
            {
                _logger.LogInfo($"Updating employee-education record. EmployeeId: {dto.EmployeeId}");

                var command = new UpdateEducationInfoCommand(dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo($"Failed to update employee-education with Id: {dto.EmployeeId}");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee-education updated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employee-education: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-education info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }
    
    
    }
}