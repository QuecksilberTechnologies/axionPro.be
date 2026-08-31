// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes tenant-scoped department and designation endpoints.
// ================================================================

using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.DesignationCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Designation
{
    /// <summary>
    /// Controller responsible for managing Designation operations such as
    /// create, update, delete, and fetch with filtering options.
    /// Uses MediatR for CQRS and custom ILoggerService for logging.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DesignationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DesignationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Used-In-Angular: retrieves designations.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): DesignationsApi.getDesignations (app/core/services/designations-api.ts:65).</para>
        /// <para>Angular purpose: retrieves designations.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/departments; /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): DesignationsStore (app/features/designations/designations.store.ts); DepartmentFilter (app/shared/components/department/department-filter/department-filter.ts); EmployeeFilter (app/shared/components/employee/employee-filter/employee-filter.ts); EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); Designations (app/features/designations/designations.ts); Departments (app/features/departments/departments.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllDesignationAsyc([FromQuery] GetDesignationRequestDTO designationRequestDTO)
        {
         //   _logger.LogInfo($"Received request to get designation from userId: {designationRequestDTO.Id}");

            var command = new GetDesignationQuery(designationRequestDTO);
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
        //         /// <para>Backend endpoint: POST /api/designation/department/group/get.</para>
        //         /// </remarks>
        //         [HttpPost("Department/Group/get")]
        //
        //
        //
        //         public async Task<IActionResult> GetAllDepartmentAsyc([FromBody] GetDepartmentRequestDTO designationRequestDTO)
        //         {
        //             _logger.LogInfo($"Received request to get tenant from tenantId: {designationRequestDTO.Id}");
        //
        //             var command = new GetDepartmentQuery(designationRequestDTO);
        //             var result = await _mediator.Send(command);
        //
        //             return Ok(result);
        //         }
        #endregion

        /// <summary>
        /// Used-In-Angular: retrieves designation options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): DesignationsApi.getDesignationOptions (app/core/services/designations-api.ts:72).</para>
        /// <para>Angular purpose: retrieves designation options.</para>
        /// <para>Integrated UI page(s): /app/employees; /app/departments; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): EmployeesStore (app/features/employees/employees.store.ts); DepartmentFilter (app/shared/components/department/department-filter/department-filter.ts); EmployeeFilter (app/shared/components/employee/employee-filter/employee-filter.ts); EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); Employees (app/features/employees/employees.ts); Departments (app/features/departments/departments.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
        /// </remarks>
        [HttpGet("option")]      
        
        public async Task<IActionResult> getDesignation([FromQuery] GetDesignationOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get designation options for DepartmentId: {requestDTO.DepartmentId}");

            var command = new GetDesignationOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: creates designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): DesignationsApi.addDesignation (app/core/services/designations-api.ts:58).</para>
        /// <para>Angular purpose: creates designation.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): DesignationManageDialog (app/features/designations/designation-manage-dialog/designation-manage-dialog.ts)</para>
        /// </remarks>
        [HttpPost("add")]       
        
        public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationRequestDTO dTO)
        {
            if (dTO == null)
            {
                _logger.LogInfo("Received null request for creating designation.");
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo($"Received request to create a new designation: {dTO.DesignationName}");

            var command = new CreateDesignationCommand(dTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: deletes designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): DesignationsApi.deleteDesignation (app/core/services/designations-api.ts:85).</para>
        /// <para>Angular purpose: deletes designation.</para>
        /// <para>Integrated UI page(s): /app/designations</para>
        /// <para>Angular UI component(s): DesignationsStore (app/features/designations/designations.store.ts); Designations (app/features/designations/designations.ts)</para>
        /// </remarks>
        [HttpDelete("delete")]       
        public async Task<IActionResult> Delete([FromQuery] DeleteDesignationRequestDTO dTO)
        {
            if (dTO == null)
            {
                _logger.LogInfo("Received null request.");
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
            }

            _logger.LogInfo($"Received request to delete designation");

            var command = new DeleteDesignationQuery(dTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: updates designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): DesignationsApi.updateDesignation (app/core/services/designations-api.ts:78).</para>
        /// <para>Angular purpose: updates designation.</para>
        /// <para>Integrated UI page(s): /app/designations</para>
        /// <para>Angular UI component(s): DesignationManageDialog (app/features/designations/designation-manage-dialog/designation-manage-dialog.ts); DesignationsStore (app/features/designations/designations.store.ts); Designations (app/features/designations/designations.ts)</para>
        /// </remarks>
        [HttpPut("update")]       
        public async Task<IActionResult> UpdateDesignation([FromBody] UpdateDesignationRequestDTO updateDesignationDTO)
        {
            _logger.LogInfo("Received request for update designation: " + updateDesignationDTO.ToString());
            var command = new UpdateDesignationCommand(updateDesignationDTO);
            var result = await _mediator.Send(command);
           
            return Ok(result);
        }
    }
}
