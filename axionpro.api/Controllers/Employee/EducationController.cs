using axionpro.application.DTOS.Employee.Bank;
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
        public async Task<IActionResult> CreateEmployee([FromForm] CreateEducationRequestDTO dto)

            {
                var command = new CreateEducationInfoCommand(dto);
                _logger.LogInfo("📩 Creating new employee education info...");

                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Get all education records (Paginated).
        /// </summary>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllEmployeeInfo([FromQuery] GetEducationRequestDTO commandDto)
        {
            
                var command = new GetEducationInfoQuery(commandDto);
                var result = await _mediator.Send(command);  
                    return Ok(result);             
          
            
        }

        
       /// <summary>
       /// Deletes employee education record by Id.
       /// </summary>
       [HttpDelete("delete")]
        
        
        
        public async Task<IActionResult> Delete([FromQuery] DeleteEducationRequestDTO dto)
        {
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteEducationInfoQuery(dto);
                var result = await _mediator.Send(command);


                _logger.LogInfo("Education deleted successfully.");
                return Ok(result);
   
          
        }
        /// <summary>
        /// Updates employee details.
        /// </summary>
        [HttpPost("update-education")]     
        public async Task<IActionResult> UpdateEducation(UpdateEducationRequestDTO dto)

        {
          
                _logger.LogInfo($"Updating employee-education record. EmployeeId: {dto.Id}");

                var command = new UpdateEducationInfoCommand(dto);
                var result = await _mediator.Send(command);

              

                _logger.LogInfo("Employee-education updated successfully.");
                return Ok(result);
            }
            
        }    
    
   
}