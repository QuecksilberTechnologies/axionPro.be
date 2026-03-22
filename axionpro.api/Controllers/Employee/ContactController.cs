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
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>
        /// <summary>
        /// Creates a new Employee Contact record.
        /// </summary>
        /// <param name="DTO"></param>
        /// <param name="Dto">Employee-Contact details</param>
        [HttpPost("create")]
        public async Task<IActionResult> CreateContactInfo([FromBody] CreateContactRequestDTO Dto)
        {
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    var invalidResponse = ApiResponse<bool>.Fail("Invalid IMEI number. It must be 15 digits numeric value.");
                    return BadRequest(invalidResponse);
                }

                _logger.LogInfo("Creating new empolyee contact process started.");

                var command = new CreateContactInfoCommand(Dto);
                var result = await _mediator.Send(command);

          
                _logger.LogInfo("Employee-contact created successfully.");
                return Ok(result);
            }
            
       


        /// <summary>
        /// Get all employee-contact based on TenantId or filters.
        /// </summary>
        [HttpGet("get")]     
                public async Task<IActionResult> GetBankinfo([FromQuery] GetContactRequestDTO requestDto)
       
            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetContactInfoQuery(requestDto);
                var result = await _mediator.Send(command);
                return Ok(result);
                        
        }



        /// <summary>
        /// Updates employee details.
        /// </summary>
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
        /// Deletes employee record by Id.
        /// </summary>
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
