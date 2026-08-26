// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Enum operations.
// ================================================================

using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Entity;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.EnumDTO;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EnumTypes
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnumController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration
        public EnumController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Get Currencies.
        /// </summary>
        /// <remarks>
        /// Handles the request to get currencies.
        /// </remarks>
        /// <param name="dto">The query parameters used to get currencies.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpGet("get-all-currencies")]


        public async Task<IActionResult> GetCurrencies([FromQuery] GetCurrencyRequestDTO dto)
        {

            _logger.LogInfo("Fetching all currencies.");

            var data = CurrencyProvider.GetAll(dto.IsActive);
          


            return Ok(data);
        }
     
        

    }
}

