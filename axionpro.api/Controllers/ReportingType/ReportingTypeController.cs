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
        /// Creates a new reporting type record.
        /// </summary>
        /// <param name="dto">reporting type creation data.</param>
        /// <returns>Returns a response containing success status and message.</returns>

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
        /// Retrieves all reporting types.
        /// </summary>
        /// <param name="dto">Filter criteria (optional).</param>
        /// <returns>Returns list of reporting types.</returns>
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
        /// Retrieves details of a specific reporting type by ID.
        /// </summary>
        /// <param name="dto">DTO containing reporting type ID.</param>
        /// <returns>Returns reporting type details.</returns>
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
        /// Updates an existing reporting type record.
        /// </summary>
        /// <param name="dto">reporting type update details.</param>
        /// <returns>Returns a response indicating success or failure.</returns>
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
        /// Soft deletes a reporting type by ID.
        /// </summary>
        /// <param name="dto">reporting type ID to be deleted.</param>
        /// <returns>Returns success or failure message.</returns>
        [HttpDelete("delete")]       
        public async Task<IActionResult> DeleteReportingType([FromQuery] DeleteReportingTypeRequestDTO dto)
        {
           
                var result = await _mediator.Send(new DeleteReportingTypeCommand(dto));
                return Ok(result);
           
            
        }

        #endregion
    }


}

