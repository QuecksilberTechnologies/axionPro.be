using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.DesignationCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
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
        /// Fetches all designations for a given tenant with optional filters.
        /// </summary>
        /// <param name="designationRequestDTO">The filter criteria for fetching designations.</param>
        /// <returns>List of matching designations wrapped in ApiResponse.</returns>
        /// <response code="200">Successfully fetched designation list.</response>
        /// <response code="400">Invalid request parameters.</response>
        /// <response code="401">Unauthorized request or invalid tenant.</response>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllDesignationAsyc([FromQuery] GetDesignationRequestDTO designationRequestDTO)
        {
         //   _logger.LogInfo($"Received request to get designation from userId: {designationRequestDTO.Id}");

            var command = new GetDesignationQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Fetches all department for a given tenant with optional filters.
        /// </summary>
        /// <param name="designationRequestDTO">The filter criteria for fetching departments.</param>
        /// <returns>List of matching department wrapped in ApiResponse.</returns>
        /// <response code="200">Successfully fetched department list.</response>
        /// <response code="400">Invalid request parameters.</response>
        /// <response code="401">Unauthorized request or invalid tenant.</response>
        [HttpPost("Department/Group/get")]
        public async Task<IActionResult> GetAllDepartmentAsyc([FromBody] GetDepartmentRequestDTO designationRequestDTO)
        {
            _logger.LogInfo($"Received request to get tenant from tenantId: {designationRequestDTO.Id}");

            var command = new GetDepartmentQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all designation.
        /// </summary>
        [HttpGet("option")]

        public async Task<IActionResult> getDesignation([FromQuery] GetDesignationOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Designation : {requestDTO.UserEmployeeId}");

            var command = new GetDesignationOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Creates a new designation for the specified tenant.
        /// </summary>
        /// <param name="dTO">Designation details to be created.</param>
        /// <returns>Operation result wrapped in ApiResponse.</returns>
        /// <response code="200">Designation created successfully.</response>
        /// <response code="400">Invalid request payload.</response>
        [HttpPost("add")]
        public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationRequestDTO dTO)
        {
            if (dTO == null)
            {
                _logger.LogInfo("Received null request for creating designation.");
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInfo($"Received request to create a new designation: {dTO.DesignationName}");

            var command = new CreateDesignationCommand(dTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes (soft delete) an existing designation.
        /// </summary>
        /// <param name="dTO">The designation delete request details.</param>
        /// <returns>Operation result wrapped in ApiResponse.</returns>
        /// <response code="200">Designation deleted successfully.</response>
        /// <response code="400">Invalid request payload.</response>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteDesignationRequestDTO dTO)
        {
            if (dTO == null)
            {
                _logger.LogInfo("Received null request.");
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInfo($"Received request to delete designation");

            var command = new DeleteDesignationQuery(dTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing designation’s information.
        /// </summary>
        /// <param name="updateDesignationDTO">The updated designation details.</param>
        /// <returns>Operation result wrapped in ApiResponse.</returns>
        /// <response code="200">Designation updated successfully.</response>
        /// <response code="400">Invalid update request.</response>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDesignation([FromBody] UpdateDesignationRequestDTO updateDesignationDTO)
        {
            _logger.LogInfo("Received request for update designation: " + updateDesignationDTO.ToString());
            var command = new UpdateDesignationCommand(updateDesignationDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
    }
}
