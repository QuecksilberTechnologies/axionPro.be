// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes employee endpoints and delegates application errors to middleware.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.ResetPassword;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.EmployeeCmd.ResetPassword.Handlers;
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
        /// Supports the Angular UI flow for create employee.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates employee.</para>
        /// <para>Angular page(s): /app/employees; /app/profile/basic-info.</para>
        /// <para>Angular API service call(s): EmployeesApi.createEmployee (app/core/services/employee-api.ts:91).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update profie image.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates profile images.</para>
        /// <para>Angular page(s): /app/profile/basic-info; /app/profile; /app/profile/identity-info.</para>
        /// <para>Angular API service call(s): EmployeeBasicAPI.updateProfileImages (app/core/services/employee-basic-api.ts:175).</para>
        /// </remarks>


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
        /// Supports the Angular UI flow for get all employee image.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves profile images.</para>
        /// <para>Angular page(s): /app/profile/basic-info; /app/profile; /app/profile/identity-info.</para>
        /// <para>Angular API service call(s): EmployeeBasicAPI.getProfileImages (app/core/services/employee-basic-api.ts:169).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update section status bulk.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates edit status.</para>
        /// <para>Angular page(s): /app/profile/bank-info; /app/profile/basic-info; /app/profile/contact-info; /app/profile/dependent-info; /app/profile/education-info; /app/profile/experience-info; /app/profile/identity-info; /app/profile/insurance-info.</para>
        /// <para>Angular API service call(s): ProfileAccessApi.updateEditStatus (app/core/services/profile-access-api.ts:47).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update verification status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates verification status.</para>
        /// <para>Angular page(s): /app/profile/bank-info; /app/profile/basic-info; /app/profile/contact-info; /app/profile/dependent-info; /app/profile/education-info; /app/profile/experience-info; /app/profile/identity-info; /app/profile/insurance-info.</para>
        /// <para>Angular API service call(s): ProfileAccessApi.updateVerificationStatus (app/core/services/profile-access-api.ts:41).</para>
        /// </remarks>
        [HttpPost("update-verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus([FromBody] UpdateVerificationStatusRequestDTO_ dto)
        {
            

            var command = new UpdateVerificationStatusCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Supports the Angular UI flow for update section status bulk.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates bulk.</para>
        /// <para>Angular page(s): /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.updateBulk (app/core/services/employee-api.ts:124).</para>
        /// </remarks>
        [Authorize]
        [HttpPost("update-bulk")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEmployeeSectionStatusRequestDTO dto)
        {

            var command = new UpdateSectionBulkCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Resets the password of a selected Tenant Employee.
        /// </summary>
        /// <remarks>
        /// <para>Authorization: only an authenticated Tenant user with the active
        /// <c>EMP_PASSWORD_MANAGEMENT</c> module and <c>Reset Password</c>
        /// operation permission can use this endpoint.</para>
        /// <para>The supplied ModuleId is bound to the active module code and the
        /// existing database stored procedure validates the supplied ModuleId and
        /// OperationId against the caller's current role permissions.</para>
        /// <para>The selected EmployeeId must be the client-safe encoded ID of an
        /// active Employee belonging to the caller's Tenant.</para>
        /// </remarks>
        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetEmployeePassword(
            [FromBody] ResetEmployeePasswordRequestDTO requestDto)
        {
            if (requestDto is null)
            {
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo("Received authorized Tenant Employee password-reset request.");

            var command = new ResetPasswordCommand(requestDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for get all employee percentage async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves all percentage.</para>
        /// <para>Angular page(s): /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.getAllPercentage (app/core/services/employee-api.ts:118).</para>
        /// </remarks>
        [Authorize]
        [HttpGet("get-all-percentage")]
        public async Task<IActionResult> GetAllEmployeePercentageAsync(
            [FromQuery] string employeeId,
            [FromQuery] axionpro.application.DTOs.BaseDTO.PermissionRequestDTO permissionRequest)
        {
           
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    _logger.LogInfo("Invalid EmployeeId received.");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidIdentifier);
                }

                _logger.LogInfo("Fetching employee completion percentage...");

                var query = new GetEmployeeProfileStatusQuery(employeeId, permissionRequest);
                var result = await _mediator.Send(query);

                _logger.LogInfo("Employee percentage fetched successfully.");

                return Ok(result);
           
           
        }


        /// <summary>
        /// Supports the Angular UI flow for get employee.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee basics.</para>
        /// <para>Angular page(s): /app/profile/basic-info; /app/profile/identity-info; /app/employees/permissions/:employeeId.</para>
        /// <para>Angular API service call(s): EmployeeBasicAPI.getEmployeeBasics (app/core/services/employee-basic-api.ts:132).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get employee summary.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee summary.</para>
        /// <para>Angular page(s): /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.getEmployeeSummary (app/core/services/employee-api.ts:138).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get employee profile summary.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves profile summary.</para>
        /// <para>Angular page(s): /app/profile/basic-info; /app/profile; /app/profile/identity-info.</para>
        /// <para>Angular API service call(s): EmployeeBasicAPI.fetchProfileSummary (app/core/services/employee-basic-api.ts:154); EmployeeBasicAPI.getProfileSummary (app/core/services/employee-basic-api.ts:182).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get all employee.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves all employees.</para>
        /// <para>Angular page(s): /app/assets/list; /app/employees; /app/leave/balances.</para>
        /// <para>Angular API service call(s): EmployeesApi.getAllEmployees (app/core/services/employee-api.ts:98).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes employee.</para>
        /// <para>Angular page(s): /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.deleteEmployee (app/core/services/employee-api.ts:111).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update employee status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee update status.</para>
        /// <para>Angular page(s): /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.updateEmployeeUpdateStatus (app/core/services/employee-api.ts:131).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee; updates employee basics.</para>
        /// <para>Angular page(s): /app/employees; /app/profile/basic-info.</para>
        /// <para>Angular API service call(s): EmployeesApi.updateEmployee (app/core/services/employee-api.ts:104); EmployeeBasicAPI.updateEmployeeBasics (app/core/services/employee-basic-api.ts:138).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for official update.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee basics.</para>
        /// <para>Angular page(s): /app/employees; /app/profile/basic-info.</para>
        /// <para>Angular API service call(s): EmployeeBasicAPI.updateEmployeeBasics (app/core/services/employee-basic-api.ts:146).</para>
        /// </remarks>
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
