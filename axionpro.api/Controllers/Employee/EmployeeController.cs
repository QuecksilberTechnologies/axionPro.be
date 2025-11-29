 
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.EmployeeCmd.UpdateProfile.Handlers;
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
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public EmployeeController(IMediator mediator, ILoggerService logger)
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
        /// <param name="employeeCreateDto">Employee details</param>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateBaseEmployeeRequestDTO employeeCreateDto)
        {
            try
            {
                // ✅ IMEI validation
                if (employeeCreateDto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {employeeCreateDto}");
                    var invalidResponse = ApiResponse<bool>.Fail("Invalid IMEI number. It must be 15 digits numeric value.");
                    return BadRequest(invalidResponse);
                }

                _logger.LogInfo("Creating new employee process started.");

                var command = new CreateBaseEmployeeInfoCommand(employeeCreateDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Failed to create employee record.");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee created successfully.");
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



        [HttpPost("profile/pic/update")]
       
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfieImage([FromForm] UpdateEmployeeImageRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Update image.");

                var command = new UpdateProfileImageCommand(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employees found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Get all employees based on TenantId or filters.
        /// </summary>

        [HttpGet("Image/get")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
       // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEmployeeImage([FromQuery] GetEmployeeImageRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all employees.");

                var command = new GetEmployeeImageQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employees found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Bulk update section verification + edit permission status for an employee.
        /// </summary>
        [HttpPost("update-bulk")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSectionStatusBulk( [FromBody] UpdateEmployeeSectionStatusRequestDTO dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<bool>.Fail("Invalid or empty request."));

            var command = new UpdateSectionBulkCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("get-all-percentage")]
        [ProducesResponseType(typeof(ApiResponse<List<CompletionSectionDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEmployeePercentageAsync([FromQuery] string employeeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    _logger.LogInfo("Invalid EmployeeId received.");
                    return BadRequest(ApiResponse<bool>.Fail("Invalid EmployeeId."));
                }

                _logger.LogInfo("Fetching employee completion percentage...");

                var query = new GetEmployeeProfileStatusQuery(employeeId);
                var result = await _mediator.Send(query);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Failed to fetch completion data.");
                    return BadRequest(result);
                }

                // ✔ Correct response mapping
                var response = ApiResponse<List<CompletionSectionDTO>>
                    .Response(result.Sections, "Fetched successfully");

                _logger.LogInfo("Employee percentage fetched successfully.");

                return Ok(response);   // ✔ Correct return
            }
            catch (Exception ex)
            {
                _logger.LogError( "Error getting employee completion percentage.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.Fail("Internal Server Error")
                );
            }
        }


  

        //[HttpPost("Image/add")]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> AddEmployeeImage([FromForm] CreateEmployeeImageRequestDTO requestDto)
        //{
        //    try
        //    {
        //        _logger.LogInfo("add new image.");

        //        var command = new CreateEmployeeImageCommand(requestDto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo("No employees found or request failed.");
        //            return BadRequest(result);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error while fetching employees: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        //}

        /// <summary>
        /// Get all employees based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEmployee([FromQuery] GetBaseEmployeeRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all employees.");

                var command = new GetBaseEmployeeInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employees found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }
        /// <summary>
        /// Get all employees based on TenantId or filters.
        /// </summary>
        [HttpGet("get-all")]

        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEmployee([FromQuery] GetAllEmployeeInfoRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all employees.");

                var command = new GetAllEmployeeInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employees found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes employee record by Id.
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            try
            {
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteEmployeeQuery(dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo($"Failed to delete employee with Id: {dto.UserEmployeeId}");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee deleted successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting employee: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while deleting employee.",
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
        //        _logger.LogInfo($"Updating employee record. EmployeeId: {dto.UserEmployeeId}");

        //        var command = new UpdateEmployeeCommand(dto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo($"Failed to update employee with Id: {dto.UserEmployeeId}");
        //            return BadRequest(result);
        //        }

        //        _logger.LogInfo("Employee updated successfully.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating employee: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        //}
    }
}
