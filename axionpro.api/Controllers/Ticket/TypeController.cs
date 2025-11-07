using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.TicketFeatures.TicketType.Commands;
using axionpro.application.Features.TicketFeatures.TicketType.Queries;
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
    public class TypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TypeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance for handling CQRS commands/queries.</param>
        /// <param name="logger">Logger instance for logging controller actions.</param>
        public TypeController(IMediator mediator, ILogger<TypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ----------------------------------------------------------------------------------------------------
        // 1️⃣ CREATE - Add new Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a new Ticket Type record.
        /// </summary>
        /// <param name="dto">Ticket type data to be created.</param>
        /// <returns>Returns the created Ticket Type list with success message.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicketType([FromBody] AddTicketTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🎯 Received request to create TicketType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new CreateTicketTypeCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating TicketType.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 2️⃣ READ - Get all Ticket Types
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves all Ticket Types available in the system.
        /// </summary>
        /// <returns>Returns a list of all Ticket Types.</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTicketTypes([FromQuery] GetTicketTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📦 Fetching all Ticket Types...");

                var result = await _mediator.Send(new GetAllTicketTypeQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching all Ticket Types.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 3️⃣ READ (BY ID) - Get specific Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves Ticket Type details by its unique ID.
        /// </summary>
        /// <param name="dto">Unique identifier of the Ticket Type.</param>
        /// <returns>Returns Ticket Type details.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetTicketTypeById([FromQuery] GetTicketTypeByIdRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🔍 Fetching TicketType details for Id = {Id}", dto);

                var result = await _mediator.Send(new GetTicketTypeByIdQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching TicketType by Id = {Id}", dto);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 4️⃣ UPDATE - Modify existing Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates the details of an existing Ticket Type.
        /// </summary>
        /// <param name="dto">Ticket type update data.</param>
        /// <returns>Returns success message after update.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTicketType([FromBody] UpdateTicketTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🛠️ Updating TicketType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new UpdateTicketTypeCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating TicketType.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 5️⃣ DELETE - Soft delete Ticket Type
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Soft deletes a Ticket Type based on its unique ID.
        /// </summary>
        /// <param name="dto">Ticket Type ID to be deleted.</param>
        /// <returns>Returns confirmation message.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteTicketType([FromBody]DeleteTicketTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🗑️ Request received to delete TicketType with Id = {Id}", dto);

                var result = await _mediator.Send(new DeleteTicketTypeCommand(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting TicketType Id = {Id}", dto);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 6️⃣ GET BY MODULE ID - Filter Ticket Types by Module
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves Ticket Types associated with a specific Module ID.
        /// </summary>
       
        /// <param name="dto">Module ID to filter Ticket Types.</param>
        /// <returns>Returns a list of Ticket Types linked to the provided Module ID.</returns>
        [HttpGet("get-by-header")]
        public async Task<IActionResult> GetTicketTypesByHeaderId( [FromQuery] GetTicketTypeByHeaderIdRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📂 Fetching Ticket Types for ModuleId = {ModuleId}", dto.TicketHeaderId);


                var result = await _mediator.Send(new GetAllTicketTypeByHeaderIdQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching Ticket Types for ModuleId = {ModuleId}", dto.TicketHeaderId);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
        /// <summary>
        /// Retrieves Ticket Types associated with a specific Module ID.
        /// </summary>

        /// <param name="dto">Role ID to filter Ticket Types.</param>
        /// <returns>Returns a list of Ticket Types linked to the provided Role ID.</returns>
        [HttpGet("get-by-role-id")]
        public async Task<IActionResult> GetTicketTypesByRoleId([FromQuery] GetTicketTypeByRoleIdRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📂 Fetching Ticket Types for RoleId = {Role Id}", dto.RoleId);


                var result = await _mediator.Send(new GetAllTicketTypeByRoleIdQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching Ticket Types for RoleId = {RoleId}", dto.RoleId);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

    }
}
