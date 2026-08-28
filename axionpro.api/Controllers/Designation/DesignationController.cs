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
        /// Supports the Angular UI flow for get all designation asyc.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves designations.</para>
        /// <para>Angular page(s): /app/designations; /app/departments; /app/payroll/overview; /app/employees; /app/payroll/payslips; /app/payroll; /app/performance/history; /app/profile/basic-info; and 1 more.</para>
        /// <para>Angular API service call(s): DesignationsApi.getDesignations (app/core/services/designations-api.ts:62).</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllDesignationAsyc([FromQuery] GetDesignationRequestDTO designationRequestDTO)
        {
         //   _logger.LogInfo($"Received request to get designation from userId: {designationRequestDTO.Id}");

            var command = new GetDesignationQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpPost("Department/Group/get")]
        
        
        
        public async Task<IActionResult> GetAllDepartmentAsyc([FromBody] GetDepartmentRequestDTO designationRequestDTO)
        {
            _logger.LogInfo($"Received request to get tenant from tenantId: {designationRequestDTO.Id}");

            var command = new GetDepartmentQuery(designationRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for get designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves designation options.</para>
        /// <para>Angular page(s): /app/employees; /app/departments; /app/designations; /app/payroll/overview; /app/payroll/payslips; /app/payroll; /app/performance/history; /app/profile/basic-info; and 1 more.</para>
        /// <para>Angular API service call(s): DesignationsApi.getDesignationOptions (app/core/services/designations-api.ts:69).</para>
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
        /// Supports the Angular UI flow for create designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates designation.</para>
        /// <para>Angular page(s): /app/designations.</para>
        /// <para>Angular API service call(s): DesignationsApi.addDesignation (app/core/services/designations-api.ts:55).</para>
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
        /// Supports the Angular UI flow for delete.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes designation.</para>
        /// <para>Angular page(s): /app/designations.</para>
        /// <para>Angular API service call(s): DesignationsApi.deleteDesignation (app/core/services/designations-api.ts:82).</para>
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
        /// Supports the Angular UI flow for update designation.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates designation.</para>
        /// <para>Angular page(s): /app/designations.</para>
        /// <para>Angular API service call(s): DesignationsApi.updateDesignation (app/core/services/designations-api.ts:75).</para>
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
