// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Dependent operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Dependent related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class DependentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DependentController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Supports the Angular UI flow for create dependent info.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates employee dependent.</para>
        /// <para>Angular page(s): /app/profile/dependent-info.</para>
        /// <para>Angular API service call(s): EmployeeDependentApi.createEmployeeDependent (app/core/services/employee-dependent-api.ts:68).</para>
        /// </remarks>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDependentInfo([FromForm] CreateDependentRequestDTO Dto)
        {
           
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                }

                _logger.LogInfo("Creating new empolyee Dependent process started.");

                var command = new CreateDependentCommand(Dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-Dependent created successfully.");
                return Ok(result);
           
          
        }


        /// <summary>
        /// Supports the Angular UI flow for getinfo.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee dependents.</para>
        /// <para>Angular page(s): /app/profile/dependent-info.</para>
        /// <para>Angular API service call(s): EmployeeDependentApi.getEmployeeDependents (app/core/services/employee-dependent-api.ts:75).</para>
        /// </remarks>
        [HttpGet("get")]    
        public async Task<IActionResult> Getinfo([FromQuery] GetDependentRequestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all  .");

                var command = new GetDependentInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
        }
        /// <summary>
        /// Supports the Angular UI flow for get in detail.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee dependents.</para>
        /// <para>Angular page(s): /app/profile/dependent-info.</para>
        /// <para>Angular API service call(s): EmployeeDependentApi.getEmployeeDependents (app/core/services/employee-dependent-api.ts:84).</para>
        /// </remarks>
        [HttpGet("get-in-detail")]    
        public async Task<IActionResult> GetInDetail([FromQuery] GetDependentRequestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all .");

                var command = new GetDependentCountsQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
        }
        

      /// <summary>
      /// Supports the Angular UI flow for delete.
      /// </summary>
      /// <remarks>
      /// <para>Angular purpose: deletes employee dependent.</para>
      /// <para>Angular page(s): /app/profile/dependent-info.</para>
      /// <para>Angular API service call(s): EmployeeDependentApi.deleteEmployeeDependent (app/core/services/employee-dependent-api.ts:99).</para>
      /// </remarks>
      [HttpDelete("delete")]               
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteContactQuery(dto);
                var result = await _mediator.Send(command);

                

                _logger.LogInfo("Dependent deleted successfully.");
                return Ok(result);
           
           
        }


        /// <summary>
        /// Supports the Angular UI flow for update.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee dependent.</para>
        /// <para>Angular page(s): /app/profile/dependent-info.</para>
        /// <para>Angular API service call(s): EmployeeDependentApi.updateEmployeeDependent (app/core/services/employee-dependent-api.ts:92).</para>
        /// </remarks>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateDependentRequestDTO dto)
        {
            
                _logger.LogInfo($"Updating employee-Dependent record. Id: {dto.Id}");

                var command = new UpdateDependentCommand(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee-Dependent updated successfully.");
                return Ok(result);
            
        }



    }
}
