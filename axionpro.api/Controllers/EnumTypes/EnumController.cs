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

        [HttpGet("get-all-currencies")]


        public async Task<IActionResult> GetCurrencies([FromQuery] GetCurrencyRequestDTO dto)
        {

            _logger.LogInfo("Fetching all currencies.");

            var data = CurrencyProvider.GetAll(dto.IsActive);
          


            return Ok(data);
        }
     
        

    }
}

 

  