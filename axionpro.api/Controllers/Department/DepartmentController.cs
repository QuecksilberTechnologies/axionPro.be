using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Department
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class DepartmentController : ControllerBase
    {
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
        /// Retrieves all departments based on filters such as TenantId, IsActive, and search criteria.
        /// </summary>
        /// <param name="departmentRequestDTO">Filter criteria for departments.</param>
        /// <returns>List of departments matching the filter.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllDepartmentsAsync([FromQuery] GetDepartmentRequestDTO departmentRequestDTO)
        {
            _logger.LogInfo($"Request received to get departments for TenantId: {departmentRequestDTO.UserEmployeeId}");

            var query = new GetDepartmentQuery(departmentRequestDTO);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        // -------------------------------------------------------
        // ✅ POST : Create new department
        // -------------------------------------------------------

        /// <summary>
        /// Creates a new department entry for the given tenant.
        /// </summary>
        /// <param name="createDto">Department details to create.</param>
        /// <returns>Success or failure response with created data.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> CreateDepartmentAsync([FromBody] CreateDependentRequestDTO createDto)
        {
            if (createDto == null)
            {
                _logger.LogInfo("CreateDepartmentAsync received a null DTO.");
                return BadRequest("Department data cannot be null.");
            }

            _logger.LogInfo($"Creating new department: {createDto.DependentName}");

            var command = new  CreateDependentCommand(createDto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        // -------------------------------------------------------
        // ✅ PUT : Update existing department
        // -------------------------------------------------------

        /// <summary>
        /// Updates an existing department's information.
        /// </summary>
        /// <param name="updateDto">Updated department details.</param>
        /// <returns>Boolean status indicating success or failure.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDepartmentAsync([FromBody] UpdateDepartmentRequestDTO updateDto)
        {
            if (updateDto == null)
            {
                _logger.LogInfo("UpdateDepartmentAsync received a null DTO.");
                return BadRequest("Invalid request. Department data is required.");
            }

            _logger.LogInfo($"Updating department Id: {updateDto.Id}");

            var command = new UpdateDepartmentCommad(updateDto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Get all department.
        /// </summary>
        [HttpGet("option")]

        public async Task<IActionResult> getDepartment([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Department : {requestDTO.UserEmployeeId}");

            var command = new GetDepartmentOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        // -------------------------------------------------------
        // ✅ DELETE : Soft delete department
        // -------------------------------------------------------

        /// <summary>
        /// Soft deletes (deactivates) a department based on Id and Tenant.
        /// </summary>
        /// <param name="deleteDto">Department delete request DTO.</param>
        /// <returns>Boolean status indicating success or failure.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDepartmentAsync([FromQuery] DeleteDepartmentRequestDTO deleteDto)
        {
            if (deleteDto == null)
            {
                _logger.LogInfo("DeleteDepartmentAsync received a null DTO.");
                return BadRequest("Delete request cannot be null.");
            }

            _logger.LogInfo($"Deleting department Id: {deleteDto.Id}");

            var command = new DeleteDepartmentQuery(deleteDto);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
