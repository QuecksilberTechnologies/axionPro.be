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
        /// Creates a new Employee Dependent record.
        /// </summary>
        /// <param name="DTO"></param>
        /// <param name="Dto">Employee-Dependent details</param>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDependentInfo([FromForm] CreateDependentRequestDTO Dto)
        {
           
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    var invalidResponse = ApiResponse<bool>.Fail("Invalid IMEI number. It must be 15 digits numeric value.");
                    return BadRequest(invalidResponse);
                }

                _logger.LogInfo("Creating new empolyee Dependent process started.");

                var command = new CreateDependentCommand(Dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-Dependent created successfully.");
                return Ok(result);
           
          
        }


        /// <summary>
        /// Get all employee-Dependent based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]    
        public async Task<IActionResult> GetBankinfo([FromQuery] GetDependentRequestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all bank.");

                var command = new GetDependentInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
        }
        

      /// <summary>
      /// Deletes employee education record by Id. 
      /// </summary>
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
        /// Updates employee details.
        /// </summary>
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
