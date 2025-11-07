 
using axionpro.application.DTOs.Client;
 
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Features.ClientCmd.Commands;
using axionpro.application.Features.ClientCmd.Queries;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Client
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
        {
            private readonly IMediator _mediator;
            private readonly ILoggerService _logger;  // Logger service ka declaration

            public ClientController(IMediator mediator, ILoggerService logger)
            {
                _mediator = mediator;
                _logger = logger;
            }


        [HttpGet("getallclienttype")]
        public async Task<IActionResult> GetAllClientType([FromQuery] ClientRequestTypeDTO clientRequestType)
        {
            _logger.LogInfo($"Received request to get clientRequestType from userId: {clientRequestType.Id}");

            var command = new GetAllClientTypeQuery(clientRequestType);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        [HttpPost("addclient")]
        public async Task<IActionResult> CreateClientType([FromBody] CreateClientTypeDTO createClientTypeDTO)
        {
            if (createClientTypeDTO == null)
            {
                _logger.LogInfo("Received null request for creating leave type.");  // ✅ अब सही है
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInfo($"Received request to create a new leave type: {createClientTypeDTO.TypeName}");

            var command = new CreateClientTypeCommand(createClientTypeDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("updateclient")]
        public async Task<IActionResult> UpdateClientType([FromBody] UpdateClientTypeDTO updateClientTypeDTO)
        {
            _logger.LogInfo("Received request for update a leave" + updateClientTypeDTO.ToString());
            var command = new UpdateClientTypeCommand(updateClientTypeDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
        //  [HttpPost("getalltendermaincategory")]
        //public async Task<IActionResult> GetAllTenderMainCategories([FromBody] TenderCategoryRequestDTO? tenderCategoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get categories from userId: {LoginId}" + tenderCategoryRequestDTO.Id.ToString());
        //    var command = new GetTenderMainCategoryRequestCommand(tenderCategoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


        //[HttpPost("getallmainchildcategory")]
        //public async Task<IActionResult> GetAllMainChildCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get sub-categories from userId: {LoginId}" + categoryRequestDTO.Id.ToString());
        //    var command = new GetMainChildCategoryCommand(categoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


    }
}
