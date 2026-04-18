using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Features.TickeAllCmd.Classification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Ticket;

/// <summary>
/// Controller responsible for managing Ticket Classification operations.
/// Handles all Create, Read, Update, and Delete (CRUD) APIs for Ticket Classifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketClassificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketClassificationController> _logger;

    public TicketClassificationController(IMediator mediator, ILogger<TicketClassificationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // ----------------------------------------------------------------------------------------------------
    // 1️⃣ CREATE - Add new Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpPost("create")]   
    public async Task<IActionResult> CreateTicketClassification([FromBody] AddClassificationRequestDTO dto)
    {
       
            _logger.LogInformation("🎯 Received request to create TicketClassification: {Data}", JsonConvert.SerializeObject(dto));
            var command = new AddClassificationCommand(dto);
            var result = await _mediator.Send(command);          
            return Ok(result);
       
      
    }

    // ----------------------------------------------------------------------------------------------------
    // 2️⃣ READ - Get all Ticket Classifications
    // ----------------------------------------------------------------------------------------------------
    //[HttpGet("all")]   
    //public async Task<IActionResult> GetAllTicketClassifications([FromQuery] GetClassificationRequestDTO dto)
    //{
        
    //        _logger.LogInformation("📦 Fetching all Ticket Classifications...");
    //        var command = new GetAllClassificationCommand(dto);
    //        var result = await _mediator.Send(command);           
    //        return Ok(result);
        
        
    //}

    // ----------------------------------------------------------------------------------------------------
    // 3️⃣ READ (BY ID) - Get specific Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpGet("get")]
    public async Task<IActionResult> GetTicketClassificationById([FromQuery] GetClassificationRequestDTO dto)
    {
       
            _logger.LogInformation("🔍 Fetching TicketClassification details for Id = {Id}", dto.Id);
            var command = new GetClassificationByIdQuery(dto);
            var result = await _mediator.Send(command);            
            return Ok(result);
       
    }

    // ----------------------------------------------------------------------------------------------------
    // 4️⃣ UPDATE - Modify existing Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpPut("update")]
    public async Task<IActionResult> UpdateTicketClassification([FromBody] UpdateClassificationRequestDTO dto)
    {
       
            _logger.LogInformation("🛠️ Updating TicketClassification: {Data}", JsonConvert.SerializeObject(dto));
            var command = new UpdateClassificationCommand(dto);
            var result = await _mediator.Send(command);          
            return Ok(result);
       
    }

    // ----------------------------------------------------------------------------------------------------
    // 5️⃣ DELETE - Soft delete Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpDelete("delete")] 
    public async Task<IActionResult> DeleteTicketClassification([FromBody] DeleteClassificationRequestDTO dto)
    {
       
            _logger.LogInformation("🗑️ Request received to delete TicketClassification with Id = {Id}", dto.Id);
            var command = new DeleteClassificationCommand(dto);
            var result = await _mediator.Send(new DeleteClassificationCommand(dto));
           
            return Ok(result);
        
      
    }
}
