
using axionpro.application.DTOs.Employee;
 
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public BankController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Creates a new employee record.
        /// </summary>
        /// <param name="DTO"></param>
        /// <param name="Dto">Employee-Bank details</param>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBankInfo([FromForm] CreateBankRequestDTO Dto)
        {
            try
            {
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    var invalidResponse = ApiResponse<bool>.Fail("Invalid IMEI number. It must be 15 digits numeric value.");
                    return BadRequest(invalidResponse);
                }

                _logger.LogInfo("Creating new Bank process started.");

                var command = new CreateBankInfoCommand(Dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Failed to create employee record.");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee-Bankinfo created successfully.");
                return Ok(result);
            }
            catch (ValidationException vex)
            {
                _logger.LogError($"Validation Error: {vex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("Validation failed.", new List<string> { vex.Message });
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred in CreateEmployee: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while creating employee.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

       
        /// <summary>
        /// Get all employees based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]

        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBankinfo([FromQuery] GetBankReqestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetBankInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employee-bank found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees-bank: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee bank info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }


        /// <summary>
        /// Updates employee details.
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] GenricUpdateRequestDTO dto)
        {
            try
            {
                _logger.LogInfo($"Updating employee-bank record. EmployeeId: {dto.EmployeeId}");

                var command = new UpdateEmployeeCommand(dto);
                var result = await
                    
                    _mediator.Send(command);

                if (
                    !result.IsSucceeded)
                {
                    _logger.LogInfo($"Failed to update employee with Id: {dto.EmployeeId}");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee updated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employee: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }
    }
}
