// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes employee endpoints and delegates application errors to middleware.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.EmployeeCmd.UpdateStatus.Handler;
using axionpro.application.Features.EmployeeCmd.UpdateVerification.Handler;
using axionpro.application.Interfaces.ILogger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        /// Create Employee.
        /// </summary>
        /// <remarks>
        /// Handles the request to create employee.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="employeeCreateDto">The request body used to create employee.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateBaseEmployeeRequestDTO employeeCreateDto)
        {
              // ✅ IMEI validation
                if (employeeCreateDto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {employeeCreateDto}");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                _logger.LogInfo("Creating new employee process started.");

                var command = new CreateBaseEmployeeInfoCommand(employeeCreateDto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee created successfully.");
                return Ok(result);
           
           
        }
        /// <summary>
        /// Update Profie Image.
        /// </summary>
        /// <remarks>
        /// Handles the request to update profie image.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The form data used to update profie image.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


        [Authorize]
        [HttpPost("profile/pic/update")]
        public async Task<IActionResult> UpdateProfieImage([FromForm] UpdateEmployeeImageRequestDTO requestDto)
        {
            
                _logger.LogInfo("Update image.");

                var command = new UpdateProfileImageCommand(requestDto);
                var result = await _mediator.Send(command);                 
                return Ok(result);
           
            
        }

        /// <summary>
        /// Get All Employee Image.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee image.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get all employee image.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("Image/get")]     
        public async Task<IActionResult> GetAllEmployeeImage([FromQuery] GetEmployeeImageRequestDTO requestDto)
        {
            
                _logger.LogInfo("Fetching all employees.");

                var command = new GetEmployeeImageQuery(requestDto);
                var result = await _mediator.Send(command);
                return Ok(result);
           
           
        }

        /// <summary>
        /// Update Section Status Bulk.
        /// </summary>
        /// <remarks>
        /// Handles the request to update section status bulk.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The request body used to update section status bulk.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPost("update-edit-status")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEditStatusRequestDTO_ dto)
        {
            if (dto == null)
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);

            var command = new UpdateEditableStatusCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Update Verification Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to update verification status.
        /// </remarks>
        /// <param name="dto">The request body used to update verification status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update-verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus([FromBody] UpdateVerificationStatusRequestDTO_ dto)
        {
            

            var command = new UpdateVerificationStatusCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Update Section Status Bulk.
        /// </summary>
        /// <remarks>
        /// Handles the request to update section status bulk.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The request body used to update section status bulk.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPost("update-bulk")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEmployeeSectionStatusRequestDTO dto)
        {

            var command = new UpdateSectionBulkCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Get All Employee Percentage.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee percentage.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="employeeId">The query parameters used to get all employee percentage.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("get-all-percentage")]
        public async Task<IActionResult> GetAllEmployeePercentageAsync([FromQuery] string employeeId)
        {
           
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    _logger.LogInfo("Invalid EmployeeId received.");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidIdentifier);
                }

                _logger.LogInfo("Fetching employee completion percentage...");

                var query = new GetEmployeeProfileStatusQuery(employeeId);
                var result = await _mediator.Send(query);

                _logger.LogInfo("Employee percentage fetched successfully.");

                return Ok(result);
           
           
        }


        /// <summary>
        /// Get Employee.
        /// </summary>
        /// <remarks>
        /// Handles the request to get employee.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get employee.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetEmployee([FromQuery] GetBaseEmployeeRequestDTO requestDto)
        {
            
                _logger.LogInfo("Fetching all employees.");

                var command = new GetBaseEmployeeInfoQuery(requestDto);
                var result = await _mediator.Send(command);


                return Ok(result);
          
           
        }
        /// <summary>
        /// Get Employee Summary.
        /// </summary>
        /// <remarks>
        /// Handles the request to get employee summary.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get employee summary.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("get-summary")]
        public async Task<IActionResult> GetEmployeeSummary([FromQuery] GetEmployeeSummaryRequestDTO requestDto)
        {

            _logger.LogInfo("Fetching all employees.");

            var command = new GetEmployeeSummaryQuery(requestDto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo("No employees summary found or request failed.");
                return BadRequest(result);
            }

            return Ok(result);


        }
        /// <summary>
        /// Get Employee Profile Summary.
        /// </summary>
        /// <remarks>
        /// Handles the request to get employee profile summary.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get employee profile summary.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("get-profile-summary")]
        public async Task<IActionResult> GetEmployeeProfileSummary([FromQuery] GetEmployeeSummaryRequestDTO requestDto)
        {
           
                _logger.LogInfo("Fetching all employees.");
                var command = new GetEmployeeProfileSummaryQuery(requestDto);
                var result = await _mediator.Send(command);

             

                return Ok(result);
           
           
        }
        /// <summary>
        /// Get All Employee.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get all employee.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllEmployee([FromQuery] GetAllEmployeeInfoRequestDTO requestDto)
        {
            
                _logger.LogInfo("Fetching all employees.");

                var command = new GetAllEmployeeInfoQuery(requestDto);
                var result = await _mediator.Send(command);
                return Ok(result);
           
            
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The query parameters used to delete.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpDelete("delete-all")]
        public async Task<IActionResult> Delete([FromQuery] DeleteBaseEmployeeRequestDTO dto)
        {
            
                _logger.LogInfo($"Deleting employee with Id: {dto.EmployeeId}");
                var command = new DeleteEmployeeQuery(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee deleted successfully.");
                return Ok(result);
            
        }
        /// <summary>
        /// Update Employee Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to update employee status.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The query parameters used to update employee status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateEmployeeStatus(
            [FromQuery] ActivateAllEmployeeRequestDTO dto)
       
            {
                _logger.LogInfo(
                    $"Updating employee active status. EmployeeId: {dto.EmployeeId}, IsActive: {dto.IsActive}");

                var command = new ActivateAllEmployeeQuery(dto);
                var result = await _mediator.Send(command);
            _logger.LogInfo(
                    $"Employee {(dto.IsActive ? "activated" : "deactivated")} successfully.");

                return Ok(result);
          
           
        }

        /// <summary>
        // Updates employee details.
        // </summary>
        /// <summary>
        /// Update.
        /// </summary>
        /// <remarks>
        /// Handles the request to update.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The request body used to update.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeRequestDTO dto)
        {
           
                _logger.LogInfo($"Updating employee record. EmployeeId: {dto.EmployeeId}");

                var command = new UpdateEmployeeCommand(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee updated successfully.");
                return Ok(result);
            
            
        }
        /// <summary>
        // Updates employee details.
        // </summary>
        /// <summary>
        /// Official Update.
        /// </summary>
        /// <remarks>
        /// Handles the request to official update.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="dto">The request body used to official update.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [Authorize]
        [HttpPost("official/update")]
        
        public async Task<IActionResult> OfficialUpdate([FromBody] UpdateEmployeeRequestOfficialDTO dto)
        {
            
                _logger.LogInfo($"Updating employee record. EmployeeId: {dto.EmployeeId}");

                var command = new UpdateBaseEmployeeByAdminCommand(dto);
                var result = await _mediator.Send(command);
            
            return Ok(result);
          
        }
    }
}
