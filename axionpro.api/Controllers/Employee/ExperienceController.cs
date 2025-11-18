using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Features.EmployeeCmd.Contact.Command;
using axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee Experience & Related operations.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ExperienceController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // <summary>
        // Create new employee experience record.
        // </summary>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateExperience([FromForm] CreateExperienceRequestDTO dto)
        {
            _logger.LogInfo("Received Experience Create Request for Employee: " + dto.EmployeeId);

            var command = new CreateExperienceInfoCommand(dto);

            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all experience records (Paginated).
        /// </summary>
        //  [HttpPost("get")]
        //    [ProducesResponseType(StatusCodes.Status200OK)]
        //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetAllexperinceInfo([FromBody] GetExperienceRequestDTO commandDto)
        //{
        //    try
        //    {
        //        var command = new GetExperienceInfoQuery(commandDto);
        //        var result = await _mediator.Send(command);

        //        if (result.IsSucceeded)
        //            return Ok(result);

        //        return BadRequest(result);
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, ApiResponse<string>.Fail("Internal server error.", new List<string> { ex.Message }));
        //    }
        //}
        /// <summary>
        /// Updates employee details.
        /// </summary>
        //[HttpPost("update")]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Update([FromBody] GenricUpdateRequestDTO dto)
        //{
        //    try
        //    {
        //        _logger.LogInfo($"Updating employee-experience record. EmployeeId: {dto.EmployeeId}");

        //        var command = new UpdateExperienceInfoCommand(dto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo($"Failed to update employee-experience with Id: {dto.EmployeeId}");
        //            return BadRequest(result);
        //        }

        //        _logger.LogInfo("Employee-experience updated successfully.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating employee-experience: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-experience info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        // }
    }
}