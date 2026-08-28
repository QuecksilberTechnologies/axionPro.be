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
        /// Supports the Angular UI flow for get all departments async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves departments.</para>
        /// <para>Angular page(s): /app/departments.</para>
        /// <para>Angular API service call(s): DepartmentsApi.getDepartments (app/core/services/departments-api.ts:33).</para>
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
        /// Supports the Angular UI flow for create department async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates department.</para>
        /// <para>Angular page(s): /app/departments.</para>
        /// <para>Angular API service call(s): DepartmentsApi.addDepartment (app/core/services/departments-api.ts:26).</para>
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
        /// Supports the Angular UI flow for update department async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates department.</para>
        /// <para>Angular page(s): /app/departments.</para>
        /// <para>Angular API service call(s): DepartmentsApi.updateDepartment (app/core/services/departments-api.ts:46).</para>
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
        /// Supports the Angular UI flow for get department.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves departments options.</para>
        /// <para>Angular page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations; and 12 more.</para>
        /// <para>Angular API service call(s): DepartmentsApi.getDepartmentsOptions (app/core/services/departments-api.ts:40).</para>
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
        /// Supports the Angular UI flow for delete department async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes department.</para>
        /// <para>Angular page(s): /app/departments.</para>
        /// <para>Angular API service call(s): DepartmentsApi.deleteDepartment (app/core/services/departments-api.ts:53).</para>
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
