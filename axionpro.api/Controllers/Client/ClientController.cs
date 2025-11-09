 
using axionpro.application.DTOs.Client;
using axionpro.application.DTOS.Common;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Features.ClientCmd.Commands;
using axionpro.application.Features.ClientCmd.Handlers;
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


        
        [HttpGet("get")]
        public async Task<IActionResult> GetAllClientType([FromQuery] GetOptionRequestDTO clientRequestType)
        {
            _logger.LogInfo($"📩 Received request to get client list for userId: {clientRequestType.UserEmployeeId}");

            // 🧩 Dummy Data (Temporary Static List)
            var clientList = new List<GetClientOptionResponseDTO>
    {
        new GetClientOptionResponseDTO { Id = "1", ClientName = "TechNova Solutions Pvt. Ltd." },
        new GetClientOptionResponseDTO { Id = "2", ClientName = "InfyCore Technologies LLP" },
        new GetClientOptionResponseDTO { Id = "3", ClientName = "BluePeak Consulting Services" },
        new GetClientOptionResponseDTO { Id = "4", ClientName = "SkyBridge Digital Systems" },
        new GetClientOptionResponseDTO { Id = "5", ClientName = "NextEra IT Innovations" },
        new GetClientOptionResponseDTO { Id = "6", ClientName = "VirtuWorks Global Pvt. Ltd." },
        new GetClientOptionResponseDTO { Id = "7", ClientName = "DataMinds Analytics" },
        new GetClientOptionResponseDTO { Id = "8", ClientName = "ProEdge Business Solutions" },
        new GetClientOptionResponseDTO { Id = "9", ClientName = "CloudNest Technologies" },
        new GetClientOptionResponseDTO { Id = "10", ClientName = "AxionPro Workforce Systems" }
    };

            // 🧾 Wrap Response (Optional Standard Format)
            var response = new
            {
                IsSucceeded = true,
                Message = "Client list fetched successfully.",
                Data = clientList
            };

            _logger.LogInfo($"✅ Returning {clientList.Count} clients successfully.");
            return Ok(response);
        }

        //[HttpGet("get")]
        //public async Task<IActionResult> GetAllClientType([FromQuery] ClientRequestTypeDTO clientRequestType)
        //{
        //    _logger.LogInfo($"Received request to get clientRequestType from userId: {clientRequestType.Id}");

        //    var command = new GetClientTypeQuery(clientRequestType);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}
        [HttpPost("add")]
        public async Task<IActionResult> CreateClientType([FromBody] CreateClientTypeDTO createClientTypeDTO)
        {
            if (createClientTypeDTO == null)
            {
                _logger.LogInfo("Received null request for creating leave type.");  // ✅ अब सही है
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInfo($"Received request to create a new leave type: {createClientTypeDTO.TypeName}");

            var command = new  CreateClientTypeCommand(createClientTypeDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("update")]
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
