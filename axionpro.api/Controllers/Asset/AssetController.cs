// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Asset operations.
// ================================================================

using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.AssetFeatures.Assets.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Asset
{
    /// <summary>
    /// Controller to handle all Asset-related operations.
    /// Supports CRUD operations, filtering, and retrieval by tenant.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetController"/> class.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands/queries.</param>
        /// <param name="logger">Logger service for tracking requests and errors.</param>
        public AssetController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region CRUD - Asset by Tenant User


        /// <summary>
        /// Get All Assets.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all assets.
        /// </remarks>
        /// <param name="assetRequestDTO">The query parameters used to get all assets.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllAssets([FromQuery] GetAssetRequestDTO assetRequestDTO)
        {
            var command = new GetAllAssetCommand(assetRequestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Add Asset.
        /// </summary>
        /// <remarks>
        /// Handles the request to add asset.
        /// </remarks>
        /// <param name="addAssetDTO">The form data used to add asset.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddAsset([FromForm] AddAssetRequestDTO  addAssetDTO)
        {
            _logger.LogInfo("Request: Add asset - " + addAssetDTO);
            var command = new AddAssetCommand(addAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Update Asset.
        /// </summary>
        /// <remarks>
        /// Handles the request to update asset.
        /// </remarks>
        /// <param name="updateAssetDTO">The form data used to update asset.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")]    
        
        public async Task<IActionResult> UpdateAsset([FromForm] UpdateAssetRequestDTO updateAssetDTO)
        {
            _logger.LogInfo("Request: Update asset - " + updateAssetDTO);
            var command = new UpdateAssetCommand(updateAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Delete Asset.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete asset.
        /// </remarks>
        /// <param name="deleteAssetDTO">The query parameters used to delete asset.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]        
        public async Task<IActionResult> DeleteAsset([FromQuery] DeleteAssetReqestDTO deleteAssetDTO)
        {
            _logger.LogInfo("Request: Delete asset - " + deleteAssetDTO);
            var command = new DeleteAssetCommand(deleteAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #endregion
    }
}
