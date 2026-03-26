using axionpro.application.DTOS.Employee.BaseEmployee;
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
        /// Get all experience records (Paginated).
        /// </summary>
          [HttpGet("get")]
        //    
        //    
        public async Task<IActionResult> GetAllexperinceInfo([FromBody] GetExperienceRequestDTO commandDto)
        {
             
                var command = new GetExperienceInfoQuery(commandDto);
                var result = await _mediator.Send(command);
                
                    return Ok(result);                 
            
            
        }
        /// <summary>
        /// Updates employee details.
        /// </summary>
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
    }
}