// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes department endpoints and delegates application errors to middleware.
// ================================================================

using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Department
{
    [ApiController]
    [Route("api/[controller]")]

    public class DepartmentController : ControllerBase    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DepartmentController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // -------------------------------------------------------
        // ✅ GET : Filtered list of departments
        // -------------------------------------------------------

        /// <summary>
        /// Used-In-Angular: retrieves departments.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves department.</para>
        /// <para>Handler flow: GetDepartmentQuery is processed by GetDepartmentQueryHandler; operation(s): GetAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDepartmentResponseDTO: Id (int), DepartmentName (string), IsActive (bool), Description (string?), Remark (string?)</para>
        /// <para>Angular function(s): DepartmentsApi.getDepartments (app/core/services/departments-api.ts:34).</para>
        /// <para>Angular purpose: retrieves departments.</para>
        /// <para>Integrated UI page(s): /app/departments</para>
        /// <para>Angular UI component(s): DepartmentsStore (app/features/departments/departments.store.ts); Departments (app/features/departments/departments.ts)</para>
        /// </remarks>
        [HttpGet("get")]



        public async Task<IActionResult> GetAllDepartmentsAsync([FromQuery] GetDepartmentRequestDTO departmentRequestDTO)
        {
            _logger.LogInfo($"Request received to get departments for TenantId: {departmentRequestDTO.UserEmployeeId}");

            var query = new GetDepartmentQuery(departmentRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // -------------------------------------------------------
        // ✅ POST : Create new department
        // -------------------------------------------------------

        /// <summary>
        /// Used-In-Angular: creates department.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: creates department.</para>
        /// <para>Handler flow: CreateDepartmentCommand is processed by CreateDepartmentCommandHandler; operation(s): CreateAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDepartmentResponseDTO: Id (int), DepartmentName (string), IsActive (bool), Description (string?), Remark (string?)</para>
        /// <para>Angular function(s): DepartmentsApi.addDepartment (app/core/services/departments-api.ts:27).</para>
        /// <para>Angular purpose: creates department.</para>
        /// <para>Integrated UI page(s): /app/departments</para>
        /// <para>Angular UI component(s): DepartmentManageDialog (app/shared/components/department/department-manage-dialog/department-manage-dialog.ts); Departments (app/features/departments/departments.ts)</para>
        /// </remarks>
        [HttpPost("add")]
        public async Task<IActionResult> CreateDepartmentAsync([FromBody] CreateDepartmentRequestDTO createDto)
        {
            if (createDto == null)
            {
                _logger.LogInfo("CreateDepartmentAsync received a null DTO.");
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo($"Creating new department: {createDto.DepartmentName}");
            var command = new  CreateDepartmentCommand(createDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        // -------------------------------------------------------
        // ✅ PUT : Update existing department
        // -------------------------------------------------------

        /// <summary>
        /// Used-In-Angular: updates department.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates department async.</para>
        /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
        /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
        /// <para>Angular function(s): DepartmentsApi.updateDepartment (app/core/services/departments-api.ts:47).</para>
        /// <para>Angular purpose: updates department.</para>
        /// <para>Integrated UI page(s): /app/departments</para>
        /// <para>Angular UI component(s): DepartmentsStore (app/features/departments/departments.store.ts); DepartmentManageDialog (app/shared/components/department/department-manage-dialog/department-manage-dialog.ts); Departments (app/features/departments/departments.ts)</para>
        /// </remarks>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDepartmentAsync([FromBody] UpdateDepartmentRequestDTO updateDto)
        {
            if (updateDto == null)
            {
                _logger.LogInfo("UpdateDepartmentAsync received a null DTO.");
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo($"Updating department Id: {updateDto.Id}");
            var command = new UpdateDepartmentCommad(updateDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Used-In-Angular: retrieves departments options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves department option.</para>
        /// <para>Handler flow: GetDepartmentOptionQuery is processed by GetDepartmentOptionQueryHandler; operation(s): GetOptionAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDepartmentOptionResponse: Id (int), DepartmentName (string)</para>
        /// <para>Angular function(s): DepartmentsApi.getDepartmentsOptions (app/core/services/departments-api.ts:41).</para>
        /// <para>Angular purpose: retrieves departments options.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
        /// </remarks>
        [HttpGet("option")]
        public async Task<IActionResult> getDepartment([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Department : {requestDTO.UserEmployeeId}");

            var command = new GetDepartmentOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        // -------------------------------------------------------
        // ✅ DELETE : Soft delete department
        // -------------------------------------------------------

        /// <summary>
        /// Used-In-Angular: deletes department.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes department.</para>
        /// <para>Handler flow: DeleteDepartmentQuery is processed by DeleteDepartmentQueryHandler; operation(s): DeleteAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): DepartmentsApi.deleteDepartment (app/core/services/departments-api.ts:54).</para>
        /// <para>Angular purpose: deletes department.</para>
        /// <para>Integrated UI page(s): /app/departments</para>
        /// <para>Angular UI component(s): DepartmentsStore (app/features/departments/departments.store.ts); Departments (app/features/departments/departments.ts)</para>
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDepartmentAsync([FromQuery] DeleteDepartmentRequestDTO deleteDto)
        {
            if (deleteDto == null)
            {
                _logger.LogInfo("DeleteDepartmentAsync received a null DTO.");
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo($"Deleting department Id: {deleteDto.Id}");
            var command = new DeleteDepartmentQuery(deleteDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
