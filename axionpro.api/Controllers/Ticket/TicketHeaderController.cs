// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Ticket Header operations.
// ================================================================

using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.TickeAllCmd.TicketHeader.Handlers;
using axionpro.application.Features.TickeAllCmd.TicketType.Handlers;
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
    public class TicketHeaderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketHeaderController> _logger;

        public TicketHeaderController(IMediator mediator, ILogger<TicketHeaderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ----------------------------------------------------------------------------------------------------
        // 1️⃣ CREATE - Add new Ticket Header
        // ----------------------------------------------------------------------------------------------------
           /// <summary>
           /// Create Header.
           /// </summary>
           /// <remarks>
           /// Handles the request to create header.
           /// </remarks>
           /// <param name="dto">The request body used to create header.</param>
           /// <returns>An HTTP response containing the result of the operation.</returns>
           [HttpPost("create")] 
         public async Task<IActionResult> CreateHeader([FromBody] AddHeaderRequestDTO dto)
         {
            
               _logger.LogInformation("🎯 Received request to create Ticket Header: {Data}", JsonConvert.SerializeObject(dto));
                var result = await _mediator.Send(new AddHeaderCommand(dto));
                  return Ok(result);
           
         }

        // ----------------------------------------------------------------------------------------------------
        // 2️⃣ READ - Get all Ticket Headers with filters
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get All Header Filter.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all header filter.
        /// </remarks>
        /// <param name="dto">The query parameters used to get all header filter.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-by-classification-id")]    
        public async Task<IActionResult> GetAllHeaderFilterAsync([FromQuery] GetTicketHeaderByClassifyIdRequestDTO dto)
        {
          
                _logger.LogInformation("📦 Fetching Ticket Headers with applied filters: {Filters}",
                    JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new GetHeaderByClassifyIdQuery(dto));
                _logger.LogInformation("✅ {Count} Ticket Headers fetched successfully.", result.Data?.Count ?? 0);
                return Ok(result);
           
           
        }
        //[HttpGet("get-filter")]    
        //public async Task<IActionResult> GetAllHeaderFilterAsync([FromQuery] GetHeaderRequestDTO dto)
        //{
          
        //        _logger.LogInformation("📦 Fetching Ticket Headers with applied filters: {Filters}",
        //            JsonConvert.SerializeObject(dto));

        //        var result = await _mediator.Send(new GetHeaderFilterCommand(dto));
        //        _logger.LogInformation("✅ {Count} Ticket Headers fetched successfully.", result.Data?.Count ?? 0);
        //        return Ok(result);
           
           
        //}

        // ----------------------------------------------------------------------------------------------------
        // 3️⃣ UPDATE - Modify existing Ticket Header
        // ----------------------------------------------------------------------------------------------------
          /// <summary>
          /// Update Header.
          /// </summary>
          /// <remarks>
          /// Handles the request to update header.
          /// </remarks>
          /// <param name="dto">The request body used to update header.</param>
          /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Delete Ticket Header.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete ticket header.
        /// </remarks>
        /// <param name="dto">The request body used to delete ticket header.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]
 
        public async Task<IActionResult> DeleteTicketHeader([FromBody]  DeleteHeaderRequestDTO dto)
        {

            _logger.LogInformation("🗑️ Request received to delete TicketHeader with Id = {Id}", dto.Id);
            var result = await _mediator.Send(new DeleteHeaderCommand(dto));
            return Ok(result);

        }

         
    }
}
