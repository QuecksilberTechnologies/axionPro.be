using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Features.TicketFeatures.TicketHeader.Commands;
using axionpro.application.Features.TicketFeatures.TicketHeader.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Ticket
{
    /// <summary>
    /// Controller responsible for managing Ticket Header operations.
    /// Handles all Create, Read, Update, and Delete (CRUD) APIs for Ticket Headers.
    /// </summary>
    [ApiController]
    [Route("api/Ticket/[controller]")]
    public class HeaderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HeaderController> _logger;

        public HeaderController(IMediator mediator, ILogger<HeaderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ----------------------------------------------------------------------------------------------------
        // 1️⃣ CREATE - Add new Ticket Header
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new Ticket Header record.
        /// </summary>
        /// <param name="dto">Ticket header data to be created.</param>
        /// <returns>Returns the created Ticket Header with a success message.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateHeader([FromBody] AddHeaderRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🎯 Received request to create Ticket Header: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new AddHeaderCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating Ticket Header.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 2️⃣ READ - Get all Ticket Headers with filters
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// 🔍 Retrieves Ticket Headers based on provided filters.
        /// </summary>
        /// <remarks>
        /// This API supports multiple optional filters for flexible querying.  
        /// Provide only the parameters you want to filter by.
        /// <br/><br/>
        /// <b>Available Filters:</b><br/>
        /// - <b>TenantId</b> (required): Fetch headers under a specific tenant. <br/>
        /// - <b>Id</b>: Fetch a specific header by its ID. <br/>
        /// - <b>HeaderName</b>: Filter by partial or full header name (case-insensitive). <br/>
        /// - <b>Description</b>: Filter by description text. <br/>
        /// - <b>IsActive</b>: Filter by active/inactive headers. <br/>
        /// - <b>TicketClassificationId</b>: Filter by classification ID. <br/>
        /// - <b>IsAssetRelated</b>: Filter headers related to assets. <br/>
        /// <br/>
        /// Example Request:  
        /// <code>
        /// GET /api/Ticket/Header/get-filter?TenantId=1&HeaderName=Network&IsActive=true
        /// </code>
        /// </remarks>
        [HttpGet("get-filter")]
        public async Task<IActionResult> GetAllHeaderFilterAsync([FromQuery] GetHeaderRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📦 Fetching Ticket Headers with applied filters: {Filters}",
                    JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new GetHeaderFilterCommand(dto));

                if (result == null || !result.IsSucceeded)
                {
                    _logger.LogWarning("⚠️ No Ticket Headers found for filters: {Filters}", JsonConvert.SerializeObject(dto));
                    return NotFound(result);
                }

                _logger.LogInformation("✅ {Count} Ticket Headers fetched successfully.", result.Data?.Count ?? 0);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching Ticket Headers with filters.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 3️⃣ UPDATE - Modify existing Ticket Header
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the details of an existing Ticket Header.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateHeader([FromBody] UpdateHeaderRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🛠️ Updating Ticket Header: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new UpdateHeaderCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating Ticket Header.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 4️⃣ DELETE - Soft delete Ticket Header
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Soft deletes a Ticket Header based on its unique ID.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteHeader([FromBody] DeleteHeaderRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🗑️ Request received to delete Ticket Header with Id = {Id}", dto.Id);

                var result = await _mediator.Send(new DeleteHeaderCommand(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting Ticket Header Id = {Id}", dto.Id);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
    }
}
