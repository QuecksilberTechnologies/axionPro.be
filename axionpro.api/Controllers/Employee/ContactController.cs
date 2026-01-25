


using axionpro.application.DTOs.test.dummy;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Contact;
 
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Contact related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ContactController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Creates a new Employee Contact record.
        /// </summary>
        /// <param name="DTO"></param>
        /// <param name="Dto">Employee-Contact details</param>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateContactInfo([FromBody] CreateContactRequestDTO Dto)
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

                _logger.LogInfo("Creating new empolyee contact process started.");

                var command = new CreateContactInfoCommand(Dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Failed to create employee record.");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee-contact created successfully.");
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
                _logger.LogError($"Exception occurred in employee-contact: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while creating employee contact.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }


        /// <summary>
        /// Get all employee-contact based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]

        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBankinfo([FromQuery] GetContactRequestDTO requestDto)
        {
            try
            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetContactInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("No employee-contact found or request failed.");
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching employees-contact: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee contact info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }



        /// <summary>
        /// Updates employee details.
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequestDTO dto)
        {
            try
            {
                _logger.LogInfo($"Updating employee-contact record. EmployeeId: {dto.Id}");

                var command = new UpdateEmployeeContactCommand(dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo($"Failed to update employee-contact with Id: {dto.Id}");
                    return BadRequest(result);
                }

                _logger.LogInfo("Employee-contact updated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employee-contact: {ex.Message}");
                var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-contact info.",
                    new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes employee record by Id.
        /// </summary>
        [HttpDelete("delete")]     
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            try
            {
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteContactQuery(dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo($"Failed to delete employee with Id: {dto.Id}");
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
    }
}
