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
        /// Get All Departments.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all departments.
        /// </remarks>
        /// <param name="departmentRequestDTO">The query parameters used to get all departments.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Create Department.
        /// </summary>
        /// <remarks>
        /// Handles the request to create department.
        /// </remarks>
        /// <param name="createDto">The request body used to create department.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Update Department.
        /// </summary>
        /// <remarks>
        /// Handles the request to update department.
        /// </remarks>
        /// <param name="updateDto">The request body used to update department.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// get Department.
        /// </summary>
        /// <remarks>
        /// Handles the request to get department.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get department.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Delete Department.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete department.
        /// </remarks>
        /// <param name="deleteDto">The query parameters used to delete department.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
