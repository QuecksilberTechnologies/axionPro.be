// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Bank operations.
// ================================================================



using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public BankController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Create Bank Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to create bank info.
        /// </remarks>
        /// <param name="Dto">The form data used to create bank info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]        
        public async Task<IActionResult> CreateBankInfo([FromForm] CreateBankRequestDTO Dto)
        {
            
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }
                _logger.LogInfo("Creating new Bank process started.");

                var command = new CreateBankInfoCommand(Dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee-Bankinfo created successfully.");
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
        
        
        public async Task<IActionResult> GetBankinfo([FromQuery] GetBankReqestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all bank.");

                var command = new GetBankInfoQuery(requestDto);
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
                public async Task<IActionResult> Delete([FromQuery] DeleteBankRequestDTO dto)
       
            {
                _logger.LogInfo($"Deleting employee bank info with Id: {dto.Id}");

                var command = new DeleteBankInfoQuery(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee bank info deleted successfully.");
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
        public async Task<IActionResult> Update([FromForm] UpdateBankReqestDTO dto)
        {
                _logger.LogInfo($"Updating employee-bank record. By UserEmployeeId: {dto.UserEmployeeId}");

                var command = new UpdateBankCommand(dto);
                var result = await

                    _mediator.Send(command);


                _logger.LogInfo("Employee updated successfully.");
                return Ok(result);
            }
           
        }


    
}
