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
        /// Get All Designation Asyc.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all designation asyc.
        /// </remarks>
        /// <param name="designationRequestDTO">The query parameters used to get all designation asyc.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllDesignationAsyc([FromQuery] GetDesignationRequestDTO designationRequestDTO)
        {
         //   _logger.LogInfo($"Received request to get designation from userId: {designationRequestDTO.Id}");

            var command = new GetDesignationQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Get All Department Asyc.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all department asyc.
        /// </remarks>
        /// <param name="designationRequestDTO">The request body used to get all department asyc.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("Department/Group/get")]
        
        
        
        public async Task<IActionResult> GetAllDepartmentAsyc([FromBody] GetDepartmentRequestDTO designationRequestDTO)
        {
            _logger.LogInfo($"Received request to get tenant from tenantId: {designationRequestDTO.Id}");

            var command = new GetDepartmentQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// get Designation.
        /// </summary>
        /// <remarks>
        /// Handles the request to get designation.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get designation.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("option")]      
        
        public async Task<IActionResult> getDesignation([FromQuery] GetDesignationOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get designation options for DepartmentId: {requestDTO.DepartmentId}");

            var command = new GetDesignationOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Create Designation.
        /// </summary>
        /// <remarks>
        /// Handles the request to create designation.
        /// </remarks>
        /// <param name="dTO">The request body used to create designation.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Delete.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete.
        /// </remarks>
        /// <param name="dTO">The query parameters used to delete.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Update Designation.
        /// </summary>
        /// <remarks>
        /// Handles the request to update designation.
        /// </remarks>
        /// <param name="updateDesignationDTO">The request body used to update designation.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
