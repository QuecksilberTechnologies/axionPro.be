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
        /// Supports the Angular UI flow for create bank info.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates employee bank.</para>
        /// <para>Angular page(s): /app/profile/bank-info.</para>
        /// <para>Angular API service call(s): EmployeeBanksAPI.createEmployeeBank (app/core/services/employee-banks-api.ts:85).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get bankinfo.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee banks.</para>
        /// <para>Angular page(s): /app/profile/bank-info.</para>
        /// <para>Angular API service call(s): EmployeeBanksAPI.getEmployeeBanks (app/core/services/employee-banks-api.ts:92).</para>
        /// </remarks>
        [HttpGet("get")]
        
        
        public async Task<IActionResult> GetBankinfo([FromQuery] GetBankReqestDTO requestDto)
        {
          
                _logger.LogInfo("Fetching all bank.");

                var command = new GetBankInfoQuery(requestDto);
                var result = await _mediator.Send(command);


                return Ok(result);
       }
          
     
        /// <summary>
        /// Supports the Angular UI flow for delete.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes employee bank.</para>
        /// <para>Angular page(s): /app/profile/bank-info.</para>
        /// <para>Angular API service call(s): EmployeeBanksAPI.deleteEmployeeBank (app/core/services/employee-banks-api.ts:105).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates employee bank.</para>
        /// <para>Angular page(s): /app/profile/bank-info.</para>
        /// <para>Angular API service call(s): EmployeeBanksAPI.updateEmployeeBank (app/core/services/employee-banks-api.ts:98).</para>
        /// </remarks>
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
