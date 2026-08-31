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
        /// Used-In-Angular: creates employee.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.createEmployee (app/core/services/employee-api.ts:95).</para>
        /// <para>Angular purpose: creates employee.</para>
        /// <para>Integrated UI page(s): /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
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
        /// Used-In-Angular: updates profile images.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeBasicAPI.updateProfileImages (app/core/services/employee-basic-api.ts:180).</para>
        /// <para>Angular purpose: updates profile images.</para>
        /// <para>Integrated UI page(s): /app/profile</para>
        /// <para>Angular UI component(s): ProfileSummary (app/shared/components/profile/profile-summary/profile-summary.ts); EmployeeProfile (app/features/user-menu/employee-profile/employee-profile.ts)</para>
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
        /// Used-In-Angular: retrieves profile images.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeBasicAPI.getProfileImages (app/core/services/employee-basic-api.ts:174).</para>
        /// <para>Angular purpose: retrieves profile images.</para>
        /// <para>Integrated UI page(s): /app/profile</para>
        /// <para>Angular UI component(s): ProfileSummary (app/shared/components/profile/profile-summary/profile-summary.ts); EmployeeProfile (app/features/user-menu/employee-profile/employee-profile.ts)</para>
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
        /// Used-In-Angular: updates edit status.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): ProfileAccessApi.updateEditStatus (app/core/services/profile-access-api.ts:50).</para>
        /// <para>Angular purpose: updates edit status.</para>
        /// <para>Integrated UI page(s): /app/profile/bank-info; /app/profile/basic-info; /app/profile/contact-info; /app/profile/dependent-info; /app/profile/education-info; /app/profile/experience-info; /app/profile/identity-info; /app/profile/insurance-info</para>
        /// <para>Angular UI component(s): ProfileHeader (app/shared/components/profile/profile-header/profile-header.ts); EmployeeBankInfo (app/features/user-menu/employee-profile/employee-bank-info/employee-bank-info.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts); EmployeeContactInfo (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-info.ts); EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts); EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts); EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts); EmployeeIdentityInfo (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-info.ts)</para>
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
        /// Used-In-Angular: updates verification status.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): ProfileAccessApi.updateVerificationStatus (app/core/services/profile-access-api.ts:44).</para>
        /// <para>Angular purpose: updates verification status.</para>
        /// <para>Integrated UI page(s): /app/profile/bank-info; /app/profile/basic-info; /app/profile/contact-info; /app/profile/dependent-info; /app/profile/education-info; /app/profile/experience-info; /app/profile/identity-info; /app/profile/insurance-info</para>
        /// <para>Angular UI component(s): ProfileHeader (app/shared/components/profile/profile-header/profile-header.ts); EmployeeBankInfo (app/features/user-menu/employee-profile/employee-bank-info/employee-bank-info.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts); EmployeeContactInfo (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-info.ts); EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts); EmployeeEducationInfo (app/features/user-menu/employee-profile/employee-education-info/employee-education-info.ts); EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts); EmployeeIdentityInfo (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-info.ts)</para>
        /// </remarks>
        [HttpPost("update-verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus([FromBody] UpdateVerificationStatusRequestDTO_ dto)
        {
            

            var command = new UpdateVerificationStatusCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Used-In-Angular: updates bulk.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.updateBulk (app/core/services/employee-api.ts:128).</para>
        /// <para>Angular purpose: updates bulk.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): VerificationSettings (app/shared/components/employee/verification-settings/verification-settings.ts); Employees (app/features/employees/employees.ts)</para>
        /// </remarks>
        [Authorize]
        [HttpPost("update-bulk")]
        public async Task<IActionResult> UpdateSectionStatusBulk([FromBody] UpdateEmployeeSectionStatusRequestDTO dto)
        {

            var command = new UpdateSectionBulkCommand(dto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/employee/reset-password.</para>
        //         /// </remarks>
        //         [Authorize]
        //         [HttpPost("reset-password")]
        //         public async Task<IActionResult> ResetEmployeePassword(
        //             [FromBody] ResetEmployeePasswordRequestDTO requestDto)
        //         {
        //             if (requestDto is null)
        //             {
        //                 throw new axionpro.application.Exceptions.ValidationErrorException(
        //                     axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
        //             }
        //
        //             _logger.LogInfo("Received authorized Tenant Employee password-reset request.");
        //
        //             var command = new ResetPasswordCommand(requestDto);
        //             var result = await _mediator.Send(command);
        //
        //             return Ok(result);
        //         }
        #endregion

        /// <summary>
        /// Used-In-Angular: retrieves all percentage.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.getAllPercentage (app/core/services/employee-api.ts:122).</para>
        /// <para>Angular purpose: retrieves all percentage.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): VerificationSettings (app/shared/components/employee/verification-settings/verification-settings.ts); Employees (app/features/employees/employees.ts)</para>
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
        /// Used-In-Angular: retrieves employee basics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeBasicAPI.getEmployeeBasics (app/core/services/employee-basic-api.ts:136).</para>
        /// <para>Angular purpose: retrieves employee basics.</para>
        /// <para>Integrated UI page(s): /app/profile/basic-info; /app/profile/identity-info; /app/employees/permissions/:employeeId</para>
        /// <para>Angular UI component(s): EmployeesPermissionsStore (app/features/employees/employees-permissions/employees-permissions.store.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts); EmployeeIdentityInfo (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-info.ts); EmployeesPermissions (app/features/employees/employees-permissions/employees-permissions.ts)</para>
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
        /// Used-In-Angular: retrieves employee summary.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.getEmployeeSummary (app/core/services/employee-api.ts:142).</para>
        /// <para>Angular purpose: retrieves employee summary.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): AvatarPopup (app/features/employees/avatar-popup/avatar-popup.ts); Employees (app/features/employees/employees.ts)</para>
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
        /// Used-In-Angular: retrieves profile summary.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeBasicAPI.fetchProfileSummary (app/core/services/employee-basic-api.ts:158); EmployeeBasicAPI.getProfileSummary (app/core/services/employee-basic-api.ts:187).</para>
        /// <para>Angular purpose: retrieves profile summary.</para>
        /// <para>Integrated UI page(s): /app/profile/basic-info; /app/profile</para>
        /// <para>Angular UI component(s): EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts); ProfileSummary (app/shared/components/profile/profile-summary/profile-summary.ts); EmployeeProfile (app/features/user-menu/employee-profile/employee-profile.ts)</para>
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
        /// Used-In-Angular: retrieves all employees.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.getAllEmployees (app/core/services/employee-api.ts:102).</para>
        /// <para>Angular purpose: retrieves all employees.</para>
        /// <para>Integrated UI page(s): /app/assets/list; /app/employees</para>
        /// <para>Angular UI component(s): AssignAssetPopup (app/features/assets-management/assign-asset-popup/assign-asset-popup.ts); EmployeesStore (app/features/employees/employees.store.ts); LeaveBalanceManageDialog (app/features/leaves/leave-requests/leave-balances/leave-balance-manage-dialog/leave-balance-manage-dialog.ts); TicketManageDialog (app/features/tickets/ticket-lists/ticket-manage-dialog/ticket-manage-dialog.ts); AssetsManagement (app/features/assets-management/assets-management.ts); Employees (app/features/employees/employees.ts)</para>
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
        /// Used-In-Angular: deletes employee.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.deleteEmployee (app/core/services/employee-api.ts:115).</para>
        /// <para>Angular purpose: deletes employee.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): EmployeesStore (app/features/employees/employees.store.ts); Employees (app/features/employees/employees.ts)</para>
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
        /// Used-In-Angular: updates employee update status.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.updateEmployeeUpdateStatus (app/core/services/employee-api.ts:135).</para>
        /// <para>Angular purpose: updates employee update status.</para>
        /// <para>Integrated UI page(s): /app/employees</para>
        /// <para>Angular UI component(s): EmployeesStore (app/features/employees/employees.store.ts); Employees (app/features/employees/employees.ts)</para>
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
        /// Used-In-Angular: updates employee; updates employee basics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeesApi.updateEmployee (app/core/services/employee-api.ts:108); EmployeeBasicAPI.updateEmployeeBasics (app/core/services/employee-basic-api.ts:142).</para>
        /// <para>Angular purpose: updates employee; updates employee basics.</para>
        /// <para>Integrated UI page(s): /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
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
        /// Used-In-Angular: updates employee official basics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EmployeeBasicAPI.updateEmployeeOfficialBasics (app/core/services/employee-basic-api.ts:150).</para>
        /// <para>Angular purpose: updates employee official basics.</para>
        /// <para>Integrated UI page(s): /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
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
