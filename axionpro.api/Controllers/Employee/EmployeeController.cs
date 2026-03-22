
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.EmployeeCmd.Handlers;
using axionpro.application.Features.EmployeeCmd.UpdateStatus.Handler;
using axionpro.application.Features.EmployeeCmd.UpdateVerification.Handler;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
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
        /// Creates a new employee record.
        /// </summary>
        /// <param name="employeeCreateDto">Employee details</param>
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateBaseEmployeeRequestDTO employeeCreateDto)
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

                _logger.LogInfo("Employee created successfully.");
                return Ok(result);
           
           
        }


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
        /// Get all employees based on TenantId or filters.
        /// </summary>
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
        ///  update  edit permission status for an employee.
        /// </summary>
        [Authorize]
        [HttpPost("update-edit-status")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEditStatusRequestDTO_ dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<bool>.Fail("Invalid or empty request."));

            var command = new UpdateEditableStatusCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        ///  update  verification permission status for an employee.
        /// </summary>
        [HttpPost("update-verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus([FromBody] UpdateVerificationStatusRequestDTO_ dto)
        {
            

            var command = new UpdateVerificationStatusCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Bulk update section verification + edit permission status for an employee.
        /// </summary>
        [Authorize]
        [HttpPost("update-bulk")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEmployeeSectionStatusRequestDTO dto)
        {

            var command = new UpdateSectionBulkCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("get-all-percentage")]
        public async Task<IActionResult> GetAllEmployeePercentageAsync([FromQuery] string employeeId)
        {
           
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    _logger.LogInfo("Invalid EmployeeId received.");
                    return BadRequest(ApiResponse<bool>.Fail("Invalid EmployeeId."));
                }

                _logger.LogInfo("Fetching employee completion percentage...");

                var query = new GetEmployeeProfileStatusQuery(employeeId);
                var result = await _mediator.Send(query);

                var response = ApiResponse<List<CompletionSectionDTO>>
                    .Response(result.Sections, "Fetched successfully");
                _logger.LogInfo("Employee percentage fetched successfully.");

                return Ok(response);   // ✔ Correct return
           
           
        }


        /// <summary>
        /// Get all employees based on TenantId or filters.
        /// </summary>
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
        /// Get  summary based on TenantId or filters.
        /// </summary>
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
        /// Get all employees based on TenantId or filters.
        /// </summary>
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
        /// Deletes employee record by Id.
        /// </summary>
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
        /// Activate or deactivate employee and all related records by Employee Id.
        /// </summary>
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
