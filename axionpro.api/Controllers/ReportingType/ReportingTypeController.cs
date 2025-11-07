using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Features.ReportTypeCmd;
using axionpro.application.Features.ReportTypeCmd.Commands;
using axionpro.application.Features.ReportTypeCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.ReportingType
{
    /// <summary>
    /// Controller responsible for managing reporting type operations.
    /// Handles all Create, Read, Update, and Delete (CRUD) APIs for reporting types.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class ReportingTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repo1rtingTypeController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance for sending commands/queries.</param>
        /// <param name="logger">Logger instance for tracking actions.</param>
        public ReportingTypeController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // =============================================================================================
        #region 🔹 CREATE reporting type
        // =============================================================================================

        /// <summary>
        /// Creates a new reporting type record.
        /// </summary>
        /// <param name="dto">reporting type creation data.</param>
        /// <returns>Returns a response containing success status and message.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateReportingType([FromBody] CreateReportingTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🎯 Received request to create ReportingType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new CreateReportingTypeCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating ReportingType.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET ALL reporting typeS
        // =============================================================================================

        /// <summary>
        /// Retrieves all reporting types.
        /// </summary>
        /// <param name="dto">Filter criteria (optional).</param>
        /// <returns>Returns list of reporting types.</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReportingTypes([FromQuery] GetReportingTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📦 Fetching all reporting types...");

                var result = await _mediator.Send(new GetAllReportingTypeQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching all reporting types.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET reporting type BY ID
        // =============================================================================================

        /// <summary>
        /// Retrieves details of a specific reporting type by ID.
        /// </summary>
        /// <param name="dto">DTO containing reporting type ID.</param>
        /// <returns>Returns reporting type details.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetReportingTypeById([FromQuery] GetReportingTypeByIdRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🔍 Fetching ReportingType details for Id = {Id}", dto.Id);

                var result = await _mediator.Send(new GetReportingTypeByIdQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching ReportingType by Id = {Id}", dto.Id);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 UPDATE reporting type
        // =============================================================================================

        /// <summary>
        /// Updates an existing reporting type record.
        /// </summary>
        /// <param name="dto">reporting type update details.</param>
        /// <returns>Returns a response indicating success or failure.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReportingType([FromBody] UpdateReportingTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🛠️ Updating ReportingType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new UpdateReportingTypeCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating ReportingType.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 DELETE reporting type
        // =============================================================================================

        /// <summary>
        /// Soft deletes a reporting type by ID.
        /// </summary>
        /// <param name="dto">reporting type ID to be deleted.</param>
        /// <returns>Returns success or failure message.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReportingType([FromBody] DeleteReportingTypeRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🗑️ Request received to delete ReportingType with Id = {Id}", dto.Id);

                var result = await _mediator.Send(new DeleteReportingTypeCommand(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting ReportingType Id = {Id}", dto.Id);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion
    }


}

