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
        /// Supports the Angular UI flow for create experience.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates employee experience.</para>
        /// <para>Angular page(s): /app/profile/experience-info.</para>
        /// <para>Angular API service call(s): EmployeeExperienceAPI.createEmployeeExperience (app/core/services/employee-experience-api.ts:100).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get allexperince info.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee experience.</para>
        /// <para>Angular page(s): /app/profile/experience-info.</para>
        /// <para>Angular API service call(s): EmployeeExperienceAPI.getEmployeeExperience (app/core/services/employee-experience-api.ts:94).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee experience.</para>
        /// <para>Angular page(s): /app/profile/experience-info.</para>
        /// <para>Angular API service call(s): EmployeeExperienceAPI.updateEmployeeExperience (app/core/services/employee-experience-api.ts:106).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes employee experience.</para>
        /// <para>Angular page(s): /app/profile/experience-info.</para>
        /// <para>Angular API service call(s): EmployeeExperienceAPI.deleteEmployeeExperience (app/core/services/employee-experience-api.ts:113).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete doc.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes employee experience doc.</para>
        /// <para>Angular page(s): /app/profile/experience-info.</para>
        /// <para>Angular API service call(s): EmployeeExperienceAPI.deleteEmployeeExperienceDoc (app/core/services/employee-experience-api.ts:119).</para>
        /// </remarks>
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
