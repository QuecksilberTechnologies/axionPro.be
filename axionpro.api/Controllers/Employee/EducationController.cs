// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Education operations.
// ================================================================

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
        /// Create Employee.
        /// </summary>
        /// <remarks>
        /// Handles the request to create employee.
        /// </remarks>
        /// <param name="dto">The form data used to create employee.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]        
        public async Task<IActionResult> CreateEmployee([FromForm] CreateEducationRequestDTO dto)

            {
                var command = new CreateEducationInfoCommand(dto);
                _logger.LogInfo("📩 Creating new employee education info...");

                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Get All Employee Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee info.
        /// </remarks>
        /// <param name="commandDto">The query parameters used to get all employee info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllEmployeeInfo([FromQuery] GetEducationRequestDTO commandDto)
        {
            
                var command = new GetEducationInfoQuery(commandDto);
                var result = await _mediator.Send(command);  
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
        public async Task<IActionResult> Delete([FromQuery] DeleteEducationRequestDTO dto)
        {
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteEducationInfoQuery(dto);
                var result = await _mediator.Send(command);


                _logger.LogInfo("Education deleted successfully.");
                return Ok(result);
   
          
        }
        /// <summary>
        /// Update Education.
        /// </summary>
        /// <remarks>
        /// Handles the request to update education.
        /// </remarks>
        /// <param name="dto">The dto used to update education.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
