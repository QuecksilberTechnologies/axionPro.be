// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Contact operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Contact;

using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Contact related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ContactController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Create Contact Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to create contact info.
        /// </remarks>
        /// <param name="Dto">The request body used to create contact info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateContactInfo([FromBody] CreateContactRequestDTO Dto)
        {
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                _logger.LogInfo("Creating new empolyee contact process started.");

                var command = new CreateContactInfoCommand(Dto);
                var result = await _mediator.Send(command);

          
                _logger.LogInfo("Employee-contact created successfully.");
                return Ok(result);
            }
            
       


        /// <summary>
        /// Get Bankinfo.
        /// </summary>
        /// <remarks>
        /// Handles the request to get bankinfo.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get bankinfo.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]     
                public async Task<IActionResult> GetBankinfo([FromQuery] GetContactRequestDTO requestDto)
       
            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetContactInfoQuery(requestDto);
                var result = await _mediator.Send(command);
                return Ok(result);
                        
        }



        /// <summary>
        /// Update Contact.
        /// </summary>
        /// <remarks>
        /// Handles the request to update contact.
        /// </remarks>
        /// <param name="dto">The request body used to update contact.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]        
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequestDTO dto)
        {
                _logger.LogInfo($"Updating employee-contact record. EmployeeId: {dto.Id}");

                var command = new UpdateEmployeeContactCommand(dto);
                var result = await _mediator.Send(command);

                

                _logger.LogInfo("Employee-contact updated successfully.");
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


                _logger.LogInfo("Employee deleted successfully.");
                return Ok(result);
           
        }
    }
}
