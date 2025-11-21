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
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDependentInfo([FromForm] CreateDependentRequestDTO Dto)
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

                _logger.LogInfo("Creating new empolyee Dependent process started.");

                var command = new CreateDependentCommand(Dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Failed to create employee record.");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee-Dependent created successfully.");
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
                _logger.LogError($"Exception occurred in employee-Dependent: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while creating employee Dependent.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }


        /// <summary>
        /// Get all employee-Dependent based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBankinfo([FromQuery] GetDependentRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetDependentInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employee-Dependent found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees-Dependent: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee Dependent info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }


        /// <summary>
        /// Updates employee details.
        /// </summary>
        //[HttpPost("update")]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> Update([FromBody] GenricUpdateRequestDTO dto)
        //{
        //    try
        //    {
        //        _logger.LogInfo($"Updating employee-Dependent record. EmployeeId: {dto._EmployeeId}");

        //        var command = new UpdateDependentCommand(dto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo($"Failed to update employee-Dependent with Id: {dto._EmployeeId}");
        //            return BadRequest(result);
        //        }

        //        _logger.LogInfo("Employee-Dependent updated successfully.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating employee-Dependent: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-Dependent info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        //}
    
    
    }
}
