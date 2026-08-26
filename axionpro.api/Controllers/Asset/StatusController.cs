// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Status operations.
// ================================================================

using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.AssetFeatures.Status.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Asset
{
    /// <summary>
    /// Controller to manage all Asset Status related operations 
    /// for Tenant Admins (Add, Update, Delete, GetAll).
    /// </summary>
    [ApiController]
    [Route("api/Asset/Status")]
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public StatusController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Tenant Admin - Asset Status CRUD

 
        /// <summary>
        /// Get By ID Asset Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to get by id asset status.
        /// </remarks>
        /// <param name="request">The query parameters used to get by id asset status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetByIdAssetStatus([FromQuery] GetStatusRequestDTO request)
        {
                _logger.LogInfo("Fetching all asset statuses for tenant...");
                var query = new GetAllAssetStatusCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
        }

        /// <summary>
        /// Add Asset Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to add asset status.
        /// </remarks>
        /// <param name="request">The request body used to add asset status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetStatus([FromBody] CreateStatusRequestDTO request)
        {

            _logger.LogInfo("Add Asset Status request received.");
            var command = new AddStatusCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Update Asset Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to update asset status.
        /// </remarks>
        /// <param name="request">The request body used to update asset status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssetStatus( [FromBody] UpdateStatusRequestDTO request)
        {
             
                _logger.LogInfo($"Update request received for Asset Status ID: {request.Id}");
                var command = new UpdateStatusCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Delete Asset Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete asset status.
        /// </remarks>
        /// <param name="request">The query parameters used to delete asset status.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]    
        
        public async Task<IActionResult> DeleteAssetStatus([FromQuery] DeleteStatusReqestDTO request)
        {
           
                _logger.LogInfo($"Delete Asset Status request received for ID: {request.Id}");
                var command = new DeleteStatusCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }
    
        #endregion
    }
}
