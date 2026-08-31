// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Workflow Stage operations.
// ================================================================


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
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/workflowstage/create.</para>
        //         /// </remarks>
        //
        //         [HttpPost("create")]
        //         public async Task<IActionResult> CreateWorkflowStage([FromBody] CreateWorkflowStageRequestDTO dto)
        //         {
        //
        //                 _logger.LogInformation("🎯 Received request to create WorkflowStage: {Data}", JsonConvert.SerializeObject(dto));
        //                 var result = await _mediator.Send(new CreateWorkflowStageCommand(dto));
        //                 return Ok(result);
        //
        //         }
        #endregion

        #endregion

        // =============================================================================================
        #region 🔹 GET ALL WORKFLOW STAGES
        // =============================================================================================
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: GET /api/workflowstage/get-all.</para>
        //         /// </remarks>
        //
        //         [HttpGet("get-all")]
        //         public async Task<IActionResult> GetAllWorkflowStages([FromQuery] GetWorkflowStageRequestDTO dto)
        //         {
        //
        //                 _logger.LogInformation("📦 Fetching all Workflow Stages...");
        //
        //                 var result = await _mediator.Send(new GetAllWorkflowStageQuery(dto));
        //
        //                 return Ok(result);
        //
        //         }
        #endregion

        #endregion

        // =============================================================================================
        #region 🔹 GET WORKFLOW STAGE BY ID
        // =============================================================================================
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: GET /api/workflowstage/get.</para>
        //         /// </remarks>
        //
        //         [HttpGet("get")]
        //         public async Task<IActionResult> GetWorkflowStageById([FromQuery] GetWorkflowStageByIdRequestDTO dto)
        //         {
        //
        //                 _logger.LogInformation("🔍 Fetching WorkflowStage details for Id = {Id}", dto.Id);
        //                 var result = await _mediator.Send(new GetWorkflowStageByIdQuery(dto));
        //
        //                 return Ok(result);
        //
        //         }
        #endregion

        #endregion

        // =============================================================================================
        #region 🔹 UPDATE WORKFLOW STAGE
        // =============================================================================================
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: PUT /api/workflowstage/update.</para>
        //         /// </remarks>
        //
        //         [HttpPut("update")]
        //         public async Task<IActionResult> UpdateWorkflowStage([FromBody] UpdateWorkflowStageRequestDTO dto)        {
        //
        //                 _logger.LogInformation("🛠️ Updating WorkflowStage: {Data}", JsonConvert.SerializeObject(dto));
        //                 var result = await _mediator.Send(new UpdateWorkflowStageCommand(dto));
        //                 return Ok(result);
        //
        //         }
        #endregion

        #endregion

        // =============================================================================================
        #region 🔹 DELETE WORKFLOW STAGE
        // =============================================================================================
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: DELETE /api/workflowstage/delete.</para>
        //         /// </remarks>
        //
        //         [HttpDelete("delete")]
        //         public async Task<IActionResult> DeleteWorkflowStage([FromBody] DeleteWorkflowStageRequestDTO dto)
        //         {
        //
        //                 _logger.LogInformation("🗑️ Request received to delete WorkflowStage with Id = {Id}", dto.Id);
        //                 var result = await _mediator.Send(new DeleteWorkflowStageCommand(dto));
        //                 return Ok(result);
        //
        //
        //         }
        #endregion

        #endregion
    }
}
