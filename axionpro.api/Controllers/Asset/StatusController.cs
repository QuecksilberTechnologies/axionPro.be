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
        /// Supports the Angular UI flow for get by id asset status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves assets status.</para>
        /// <para>Angular page(s): /app/assets/asset-status; /app/assets/list; /app/roles; /app/assets/asset-types.</para>
        /// <para>Angular API service call(s): AssetStatusApi.fetchAssetsStatus (app/core/services/asset-status-api.ts:34).</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> GetByIdAssetStatus([FromQuery] GetStatusRequestDTO request)
        {
                _logger.LogInfo("Fetching all asset statuses for tenant...");
                var query = new GetAllAssetStatusCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for add asset status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates asset status.</para>
        /// <para>Angular page(s): /app/assets/asset-status.</para>
        /// <para>Angular API service call(s): AssetStatusApi.createAssetStatus (app/core/services/asset-status-api.ts:27).</para>
        /// </remarks>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetStatus([FromBody] CreateStatusRequestDTO request)
        {

            _logger.LogInfo("Add Asset Status request received.");
            var command = new AddStatusCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for update asset status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates asset status.</para>
        /// <para>Angular page(s): /app/assets/asset-status.</para>
        /// <para>Angular API service call(s): AssetStatusApi.updateAssetStatus (app/core/services/asset-status-api.ts:40).</para>
        /// </remarks>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssetStatus( [FromBody] UpdateStatusRequestDTO request)
        {
             
                _logger.LogInfo($"Update request received for Asset Status ID: {request.Id}");
                var command = new UpdateStatusCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Supports the Angular UI flow for delete asset status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes asset status.</para>
        /// <para>Angular page(s): /app/assets/asset-status.</para>
        /// <para>Angular API service call(s): AssetStatusApi.deleteAssetStatus (app/core/services/asset-status-api.ts:47).</para>
        /// </remarks>
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
