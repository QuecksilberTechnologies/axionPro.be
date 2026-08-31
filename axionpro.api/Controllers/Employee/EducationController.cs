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
        /// Used-In-Angular: creates employee education.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeEducationAPI.createEmployeeEducation (app/core/services/employee-education-api.ts:81).</para>
        /// <para>Angular purpose: creates employee education.</para>
        /// <para>Integrated UI page(s): /app/profile/education-info</para>
        /// <para>Angular UI component(s): EmployeeEducationForm (app/features/user-menu/employee-profile/employee-education-info/employee-education-form/employee-education-form.ts); EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts)</para>
        /// </remarks>
        [HttpPost("create")]        
        public async Task<IActionResult> CreateEmployee([FromForm] CreateEducationRequestDTO dto)

            {
                var command = new CreateEducationInfoCommand(dto);
                _logger.LogInfo("📩 Creating new employee education info...");

                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Used-In-Angular: retrieves employee educations.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeEducationAPI.getEmployeeEducations (app/core/services/employee-education-api.ts:88).</para>
        /// <para>Angular purpose: retrieves employee educations.</para>
        /// <para>Integrated UI page(s): /app/profile/education-info</para>
        /// <para>Angular UI component(s): EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts)</para>
        /// </remarks>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllEmployeeInfo([FromQuery] GetEducationRequestDTO commandDto)
        {
            
                var command = new GetEducationInfoQuery(commandDto);
                var result = await _mediator.Send(command);  
                    return Ok(result);             
          
            
        }

        
       /// <summary>
       /// Used-In-Angular: deletes employee education.
       /// </summary>
       /// <remarks>
       /// <para>Angular usage status: Used-In-Angular.</para>
       /// <para>Angular function(s): EmployeeEducationAPI.deleteEmployeeEducation (app/core/services/employee-education-api.ts:101).</para>
       /// <para>Angular purpose: deletes employee education.</para>
       /// <para>Integrated UI page(s): /app/profile/education-info</para>
       /// <para>Angular UI component(s): EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts)</para>
       /// </remarks>
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
        /// Used-In-Angular: updates employee education.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeEducationAPI.updateEmployeeEducation (app/core/services/employee-education-api.ts:94).</para>
        /// <para>Angular purpose: updates employee education.</para>
        /// <para>Integrated UI page(s): /app/profile/education-info</para>
        /// <para>Angular UI component(s): EmployeeEducationForm (app/features/user-menu/employee-profile/employee-education-info/employee-education-form/employee-education-form.ts); EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts)</para>
        /// </remarks>
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
