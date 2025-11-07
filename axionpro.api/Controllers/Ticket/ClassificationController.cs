using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Features.TicketFeatures.Classification.Commands;
using axionpro.application.Wrappers;
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
        try
        {
            _logger.LogInformation("🎯 Received request to create TicketClassification: {Data}", JsonConvert.SerializeObject(dto));
            var command = new AddClassificationCommand(dto);
            var result = await _mediator.Send(command);
          
            if (!result.IsSucceeded)
                return BadRequest(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while creating TicketClassification.");
            return StatusCode(500, new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = $"Internal Server Error: {ex.Message}"
            });
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 2️⃣ READ - Get all Ticket Classifications
    // ----------------------------------------------------------------------------------------------------
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTicketClassifications([FromQuery] GetClassificationRequestDTO dto)
    {
        try
        {
            _logger.LogInformation("📦 Fetching all Ticket Classifications...");
            var command = new GetAllClassificationCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while fetching all Ticket Classifications.");
            return StatusCode(500, new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = $"Internal Server Error: {ex.Message}"
            });
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 3️⃣ READ (BY ID) - Get specific Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpGet("get")]
    public async Task<IActionResult> GetTicketClassificationById([FromQuery] GetClassificationRequestDTO dto)
    {
        try
        {
            _logger.LogInformation("🔍 Fetching TicketClassification details for Id = {Id}", dto.Id);
            var command = new GetClassificationByIdQuery(dto);
            var result = await _mediator.Send(command);              
            if (!result.IsSucceeded)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while fetching TicketClassification by Id = {Id}", dto.Id);
            return StatusCode(500, new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = $"Internal Server Error: {ex.Message}"
            });
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 4️⃣ UPDATE - Modify existing Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpPut("update")]
    public async Task<IActionResult> UpdateTicketClassification([FromBody] UpdateClassificationRequestDTO dto)
    {
        try
        {
            _logger.LogInformation("🛠️ Updating TicketClassification: {Data}", JsonConvert.SerializeObject(dto));
            var command = new UpdateClassificationCommand(dto);
            var result = await _mediator.Send(command); 
            if (!result.IsSucceeded)
                return BadRequest(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while updating TicketClassification.");
            return StatusCode(500, new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = $"Internal Server Error: {ex.Message}"
            });
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 5️⃣ DELETE - Soft delete Ticket Classification
    // ----------------------------------------------------------------------------------------------------
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteTicketClassification([FromBody] DeleteClassificationRequestDTO dto)
    {
        try
        {
            _logger.LogInformation("🗑️ Request received to delete TicketClassification with Id = {Id}", dto.Id);
            var command = new DeleteClassificationCommand(dto);
            var result = await _mediator.Send(new DeleteClassificationCommand(dto));
            if (!result.IsSucceeded)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while deleting TicketClassification Id = {Id}", dto.Id);
            return StatusCode(500, new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = $"Internal Server Error: {ex.Message}"
            });
        }
    }
}
