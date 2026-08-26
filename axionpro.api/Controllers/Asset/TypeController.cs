// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Type operations.
// ================================================================

using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.AssetFeatures.Type.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Asset
{
    /// <summary>
    /// Controller to manage all Asset Type related operations 
    /// for Tenant Admins (Add, Update, Delete, GetAll, GetById, GetByCategoryId).
    /// </summary>
    [ApiController]
    [Route("api/Asset/Type")]
    public class TypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public TypeController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Tenant Admin - Asset Type CRUD

        /// <summary>
        /// Get All Asset Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all asset type.
        /// </remarks>
        /// <param name="request">The query parameters used to get all asset type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAssetType([FromQuery] GetTypeRequestDTO request)
        {
           
                _logger.LogInfo("Fetching all asset types for tenant...");
                var query = new GetAllTypeCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
          
        }

    
        /// <summary>
        /// Add Asset Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to add asset type.
        /// </remarks>
        /// <param name="request">The request body used to add asset type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("add")]        
        public async Task<IActionResult> AddAssetType([FromBody] AddTypeRequestDTO request)
        {
            
                _logger.LogInfo("Add Asset Type request received.");
                var command = new AddTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            
        }

        /// <summary>
        /// Updates an existing asset type record.
        /// </summary>
        // [HttpPost("update")]
        /// <summary>
        /// Update Asset Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to update asset type.
        /// </remarks>
        /// <param name="request">The request body used to update asset type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")] 
        public async Task<IActionResult> UpdateAssetType([FromBody] UpdateTypeRequestDTO request)
        {
            
                _logger.LogInfo($"Update Asset Type request received for ID: {request.Id}");
                var command = new UpdateAssetTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Delete Asset Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete asset type.
        /// </remarks>
        /// <param name="request">The query parameters used to delete asset type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")] 
        public async Task<IActionResult> DeleteAssetType([FromQuery] DeleteTypeRequestDTO request)
        {

                _logger.LogInfo($"Delete Asset Type request received for ID: {request.Id}");
                var command = new DeletetTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        #endregion
    }
}
