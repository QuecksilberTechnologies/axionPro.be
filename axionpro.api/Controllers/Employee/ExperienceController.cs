// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Experience operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;

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
        /// <summary>
        /// Create Experience.
        /// </summary>
        /// <remarks>
        /// Handles the request to create experience.
        /// </remarks>
        /// <param name="dto">The form data used to create experience.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]       
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateExperience([FromForm] CreateExperienceRequestDTO dto)
        {
            _logger.LogInfo("Received Experience Create Request for Employee: " + dto.EmployeeId);
            var command = new CreateExperienceInfoCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get Allexperince Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to get allexperince info.
        /// </remarks>
        /// <param name="commandDto">The query parameters used to get allexperince info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]
        //    
        //    
        public async Task<IActionResult> GetAllexperinceInfo([FromQuery] GetExperienceRequestDTO commandDto)
        {
             
                var command = new GetExperienceInfoQuery(commandDto);
                var result = await _mediator.Send(command);
                
                    return Ok(result);                 
            
            
        }
        /// <summary>
        /// Update.
        /// </summary>
        /// <remarks>
        /// Handles the request to update.
        /// </remarks>
        /// <param name="dto">The form data used to update.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateExperienceRequestDTO dto)
        {
              _logger.LogInfo($"Updating employee-experience record. EmployeeId: {dto.EmployeeId}");
                var command = new UpdateExperienceInfoCommand(dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-experience updated successfully.");
                return Ok(result);
            
        }
        /// <summary>
        /// Delete.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete.
        /// </remarks>
        /// <param name="dto">The query parameters used to delete.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

            var command = new DeleteExperienceCommand(dto);
            var result = await _mediator.Send(command);


            _logger.LogInfo("Education deleted successfully.");
            return Ok(result);


        }
        /// <summary>
        /// Delete Doc.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete doc.
        /// </remarks>
        /// <param name="dto">The query parameters used to delete doc.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete-doc")]
        public async Task<IActionResult> DeleteDoc([FromQuery] DeleteRequestDTO dto)
        {
            _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

            var command = new DeleteExperienceDocCommand(dto);
            var result = await _mediator.Send(command);


            _logger.LogInfo("Education deleted successfully.");
            return Ok(result);


        }
    }
}
