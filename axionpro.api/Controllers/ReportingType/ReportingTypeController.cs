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
        /// Supports the Angular UI flow for create reporting type.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates report type.</para>
        /// <para>Angular page(s): /app/report-types.</para>
        /// <para>Angular API service call(s): ReportTypeApi.addReportType (app/core/services/report-type-api.ts:31).</para>
        /// </remarks>

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
        /// Supports the Angular UI flow for get all reporting types.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves report types.</para>
        /// <para>Angular page(s): /app/report-types.</para>
        /// <para>Angular API service call(s): ReportTypeApi.getReportTypes (app/core/services/report-type-api.ts:25).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update reporting type.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates report type.</para>
        /// <para>Angular page(s): /app/report-types.</para>
        /// <para>Angular API service call(s): ReportTypeApi.updateReportType (app/core/services/report-type-api.ts:37).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete reporting type.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes report type.</para>
        /// <para>Angular page(s): /app/report-types.</para>
        /// <para>Angular API service call(s): ReportTypeApi.deleteReportType (app/core/services/report-type-api.ts:44).</para>
        /// </remarks>
        [HttpDelete("delete")]       
        public async Task<IActionResult> DeleteReportingType([FromQuery] DeleteReportingTypeRequestDTO dto)
        {

                var result = await _mediator.Send(new DeleteReportingTypeCommand(dto));
                return Ok(result);
           
            
        }

        #endregion
    }


}

