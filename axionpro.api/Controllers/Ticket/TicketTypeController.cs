// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Ticket Type operations.
// ================================================================

using axionpro.application.DTOS.TicketDTO.TicketType;
 
using axionpro.application.Features.TickeAllCmd.TicketType.Handlers;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Ticket
{
    /// <summary>
    /// Controller responsible for managing Ticket Type operations.
    /// Handles all Create, Read, Update, and Delete (CRUD) APIs for Ticket Types.
    /// </summary>
    [ApiController]
    [Route("api/Ticket/[controller]")]
    public class TicketTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketTypeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketTypeController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance for handling CQRS commands/queries.</param>
        /// <param name="logger">Logger instance for logging controller actions.</param>
        public TicketTypeController(IMediator mediator, ILogger<TicketTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ----------------------------------------------------------------------------------------------------
        // 1️⃣ CREATE - Add new Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Create Ticket Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to create ticket type.
        /// </remarks>
        /// <param name="dto">The request body used to create ticket type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicketType([FromBody] AddTicketTypeRequestDTO dto)
        {

            _logger.LogInformation("🎯 Received request to create TicketType: {Data}", JsonConvert.SerializeObject(dto));
            var result = await _mediator.Send(new CreateTicketTypeCommand(dto));
            return Ok(result);
        }



        // ----------------------------------------------------------------------------------------------------
        // 2️⃣ READ - Get all Ticket Types
        // ----------------------------------------------------------------------------------------------------

        // <summary>
        /// <summary>
        /// Get All Ticket Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all ticket types.
        /// </remarks>
        /// <param name="dto">The query parameters used to get all ticket types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTicketTypes([FromQuery] GetTicketTypeRequestDTO dto)
        {

            _logger.LogInformation("📦 Fetching all Ticket Types...");
            var result = await _mediator.Send(new GetAllTicketTypeQuery(dto));
            return Ok(result);
        }
        /// <summary>
        /// Get DDL Ticket Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get ddl ticket types.
        /// </remarks>
        /// <param name="dto">The query parameters used to get ddl ticket types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpGet("ddl-list")]
        public async Task<IActionResult> GetDDLTicketTypes([FromQuery] GetDDLTicketTypeRequestDTO dto)
        {

            _logger.LogInformation("📦 Fetching all Ticket Types for DDL...");
            var result = await _mediator.Send(new DDLTicketTypeQuery(dto));
            return Ok(result);
        }

        // ----------------------------------------------------------------------------------------------------
        // 3️⃣ READ (BY ID) - Get specific Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get Ticket Type By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get ticket type by id.
        /// </remarks>
        /// <param name="dto">The query parameters used to get ticket type by id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetTicketTypeById([FromQuery] GetTicketTypeByIdRequestDTO dto)
        {

            _logger.LogInformation("🔍 Fetching TicketType details for Id = {Id}", dto);
            var result = await _mediator.Send(new GetTicketTypeByIdQuery(dto));
            return Ok(result);

        }

        // ----------------------------------------------------------------------------------------------------
        // 4️⃣ UPDATE - Modify existing Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Update Ticket Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to update ticket type.
        /// </remarks>
        /// <param name="dto">The request body used to update ticket type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")]      
        public async Task<IActionResult> UpdateTicketType([FromBody] UpdateTicketTypeRequestDTO dto)
        {
            
                var result = await _mediator.Send(new UpdateTicketTypeCommand(dto));     

                return Ok(result);
            
        }

        // ----------------------------------------------------------------------------------------------------
        // 5️⃣ DELETE - Soft delete Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Delete Ticket Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete ticket type.
        /// </remarks>
        /// <param name="dto">The request body used to delete ticket type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]        
        public async Task<IActionResult> DeleteTicketType([FromBody]DeleteTicketTypeRequestDTO dto)
        {
            
                _logger.LogInformation("🗑️ Request received to delete TicketType with Id = {Id}", dto);
                var result = await _mediator.Send(new DeleteTicketTypeCommand(dto));
                return Ok(result);
           
        }

        // ----------------------------------------------------------------------------------------------------
        // 6️⃣ GET BY MODULE ID - Filter Ticket Types by Module
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves Ticket Types associated with a specific Module ID.
        /// </summary>

        /// <summary>
        /// Get Ticket Types By Header ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get ticket types by header id.
        /// </remarks>
        /// <param name="dto">The query parameters used to get ticket types by header id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-by-header-id")]
        public async Task<IActionResult> GetTicketTypesByHeaderId( [FromQuery] GetTicketTypeByHeaderIdRequestDTO dto)
        {

                _logger.LogInformation("📂 Fetching Ticket Types for ModuleId = {ModuleId}", dto.TicketHeaderId);
                var result = await _mediator.Send(new GetAllTicketTypeByHeaderIdQuery(dto));
            return Ok(result);
           
        }
        /// <summary>
        /// Retrieves Ticket Types associated with a specific Module ID.
        /// </summary>

        /// <param name="dto">Role ID to filter Ticket Types.</param>
        /// <returns>Returns a list of Ticket Types linked to the provided Role ID.</returns>
        //[HttpGet("get-by-role-id")]
        //public async Task<IActionResult> GetTicketTypesByRoleId([FromQuery] GetTicketTypeByRoleIdRequestDTO dto)
        //{
        //    var result = await _mediator.Send(new GetAllTicketTypeByRoleIdQuery(dto));                         
        //        return Ok(result);
           
        //}

    }
}
