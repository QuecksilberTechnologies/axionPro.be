// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Reporting Type operations.
// ================================================================

using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Features.ReportTypeCmd.Handlers;
 
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
        private readonly ILogger<ReportingTypeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingTypeController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance for sending commands/queries.</param>
        /// <param name="logger">Logger instance for tracking actions.</param>

        public ReportingTypeController(
            IMediator mediator,
            ILogger<ReportingTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        // =============================================================================================
        #region 🔹 CREATE reporting type
        // =============================================================================================

        /// <summary>
        /// Create Reporting Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to create reporting type.
        /// </remarks>
        /// <param name="dto">The request body used to create reporting type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("create")]       
        
        public async Task<IActionResult> CreateReportingType([FromBody] CreateReportingTypeRequestDTO dto)
         {
           
                _logger.LogInformation("🎯 Received request to create ReportingType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new CreateReportingTypeCommand(dto));

                return Ok(result);
           
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET ALL reporting typeS
        // =============================================================================================

        /// <summary>
        /// Get All Reporting Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all reporting types.
        /// </remarks>
        /// <param name="dto">The query parameters used to get all reporting types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReportingTypes([FromQuery] GetReportingTypeRequestDTO dto)
        {
           
                _logger.LogInformation("📦 Fetching all reporting types...");
                var result = await _mediator.Send(new GetAllReportingTypeQuery(dto));
                return Ok(result);
          
        }

        #endregion

        // =============================================================================================
        #region 🔹 GET reporting type BY ID
        // =============================================================================================

        /// <summary>
        /// Get Reporting Type By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get reporting type by id.
        /// </remarks>
        /// <param name="dto">The query parameters used to get reporting type by id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-by-id")]   
        public async Task<IActionResult> GetReportingTypeById([FromQuery] GetReportingTypeByIdRequestDTO dto)
        {         

                var result = await _mediator.Send(new GetReportingTypeByIdQuery(dto));      

                return Ok(result);
           
        }

        #endregion

        // =============================================================================================
        #region 🔹 UPDATE reporting type
        // =============================================================================================

        /// <summary>
        /// Update Reporting Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to update reporting type.
        /// </remarks>
        /// <param name="dto">The request body used to update reporting type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")]  
        public async Task<IActionResult> UpdateReportingType([FromBody] UpdateReportingTypeRequestDTO dto)
        {
            
                _logger.LogInformation("🛠️ Updating ReportingType: {Data}", JsonConvert.SerializeObject(dto));

                var result = await _mediator.Send(new UpdateReportingTypeCommand(dto));
              return Ok(result);
          
        }

        #endregion

        // =============================================================================================
        #region 🔹 DELETE reporting type
        // =============================================================================================

        /// <summary>
        /// Delete Reporting Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete reporting type.
        /// </remarks>
        /// <param name="dto">The query parameters used to delete reporting type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]       
        public async Task<IActionResult> DeleteReportingType([FromQuery] DeleteReportingTypeRequestDTO dto)
        {

                var result = await _mediator.Send(new DeleteReportingTypeCommand(dto));
                return Ok(result);
           
            
        }

        #endregion
    }


}

