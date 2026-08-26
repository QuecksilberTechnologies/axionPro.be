// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Dependent operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Dependent related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class DependentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DependentController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Create Dependent Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to create dependent info.
        /// </remarks>
        /// <param name="Dto">The form data used to create dependent info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDependentInfo([FromForm] CreateDependentRequestDTO Dto)
        {
           
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                }

                _logger.LogInfo("Creating new empolyee Dependent process started.");

                var command = new CreateDependentCommand(Dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-Dependent created successfully.");
                return Ok(result);
           
          
        }


        /// <summary>
        /// Getinfo.
        /// </summary>
        /// <remarks>
        /// Handles the request to getinfo.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to getinfo.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]    
        public async Task<IActionResult> Getinfo([FromQuery] GetDependentRequestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all  .");

                var command = new GetDependentInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
        }
        /// <summary>
        /// Get In Detail.
        /// </summary>
        /// <remarks>
        /// Handles the request to get in detail.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get in detail.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-in-detail")]    
        public async Task<IActionResult> GetInDetail([FromQuery] GetDependentRequestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all .");

                var command = new GetDependentCountsQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
        }
        

      /// <summary>
      /// Delete.
      /// </summary>
      /// <remarks>
      /// Handles the request to delete.
      /// </remarks>
      /// <param name="dto">The query parameters used to delete.</param>
      /// <returns>An HTTP response containing the result of the operation.</returns>
      [HttpDelete("delete")]               
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            
                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteContactQuery(dto);
                var result = await _mediator.Send(command);

                

                _logger.LogInfo("Dependent deleted successfully.");
                return Ok(result);
           
           
        }


        /// <summary>
        /// Update.
        /// </summary>
        /// <remarks>
        /// Handles the request to update.
        /// </remarks>
        /// <param name="dto">The form data used to update.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateDependentRequestDTO dto)
        {
            
                _logger.LogInfo($"Updating employee-Dependent record. Id: {dto.Id}");

                var command = new UpdateDependentCommand(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee-Dependent updated successfully.");
                return Ok(result);
            
        }



    }
}
