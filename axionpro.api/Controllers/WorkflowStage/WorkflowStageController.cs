
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Features.WorkflowStage.Commands;
using axionpro.application.Features.WorkflowStage.Queries;
 
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.WorkflowStage
{
    /// <summary>
    /// Controller responsible for managing Workflow Stage operations.
    /// Handles all Create, Read, Update, and Delete (CRUD) APIs for Workflow Stages.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowStageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WorkflowStageController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowStageController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance for sending commands/queries.</param>
        /// <param name="logger">Logger instance for tracking actions.</param>
        public WorkflowStageController(IMediator mediator, ILogger<WorkflowStageController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // =============================================================================================
        #region 🔹 CREATE WORKFLOW STAGE
        // =============================================================================================

        /// <summary>
        /// Creates a new workflow stage record.
        /// </summary>
        /// <param name="dto">Workflow stage creation data.</param>
        /// <returns>Returns a response containing success status and message.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkflowStage([FromBody] CreateWorkflowStageRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🎯 Received request to create WorkflowStage: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new CreateWorkflowStageCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating WorkflowStage.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET ALL WORKFLOW STAGES
        // =============================================================================================

        /// <summary>
        /// Retrieves all workflow stages.
        /// </summary>
        /// <param name="dto">Filter criteria (optional).</param>
        /// <returns>Returns list of workflow stages.</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllWorkflowStages([FromQuery] GetWorkflowStageRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("📦 Fetching all Workflow Stages...");

                var result = await _mediator.Send(new GetAllWorkflowStageQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching all Workflow Stages.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET WORKFLOW STAGE BY ID
        // =============================================================================================

        /// <summary>
        /// Retrieves details of a specific workflow stage by ID.
        /// </summary>
        /// <param name="dto">DTO containing workflow stage ID.</param>
        /// <returns>Returns workflow stage details.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetWorkflowStageById([FromQuery] GetWorkflowStageByIdRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🔍 Fetching WorkflowStage details for Id = {Id}", dto.Id);

                var result = await _mediator.Send(new GetWorkflowStageByIdQuery(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching WorkflowStage by Id = {Id}", dto.Id);
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 UPDATE WORKFLOW STAGE
        // =============================================================================================

        /// <summary>
        /// Updates an existing workflow stage record.
        /// </summary>
        /// <param name="dto">Workflow stage update details.</param>
        /// <returns>Returns a response indicating success or failure.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWorkflowStage([FromBody] UpdateWorkflowStageRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🛠️ Updating WorkflowStage: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new UpdateWorkflowStageCommand(dto));

                if (!result.IsSucceeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating WorkflowStage.");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        #endregion

        // =============================================================================================
        #region 🔹 DELETE WORKFLOW STAGE
        // =============================================================================================

        /// <summary>
        /// Soft deletes a workflow stage by ID.
        /// </summary>
        /// <param name="dto">Workflow stage ID to be deleted.</param>
        /// <returns>Returns success or failure message.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteWorkflowStage([FromBody] DeleteWorkflowStageRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("🗑️ Request received to delete WorkflowStage with Id = {Id}", dto.Id);

                var result = await _mediator.Send(new DeleteWorkflowStageCommand(dto));

                if (!result.IsSucceeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting WorkflowStage Id = {Id}", dto.Id);
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
