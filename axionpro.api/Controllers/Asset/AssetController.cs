 
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Features.AssetFeatures.Assets.Commands;
 
using axionpro.application.Features.AssetFeatures.Type.Commands;
using axionpro.application.Interfaces.ILogger;
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
        /// Retrieves all assets based on filter criteria.
        /// </summary>
        /// <param name="assetRequestDTO">Filter parameters like TenantId, TypeId, etc.</param>
        /// <returns>List of assets matching the criteria.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAssets([FromQuery] GetAssetRequestDTO assetRequestDTO)
        {
            var command = new GetAllAssetCommand(assetRequestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Adds a new asset.
        /// </summary>
        /// <param name="addAssetDTO">DTO containing asset details to create.</param>
        /// <returns>Returns the created asset with its Id and other details.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddAsset([FromBody] AddAssetRequestDTO  addAssetDTO)
        {
            _logger.LogInfo("Request: Add asset - " + addAssetDTO);
            var command = new AddAssetCommand(addAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing asset.
        /// </summary>
        /// <param name="updateAssetDTO">DTO containing updated asset details including Id.</param>
        /// <returns>Returns the updated asset information.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsset([FromBody] UpdateAssetRequestDTO updateAssetDTO)
        {
            _logger.LogInfo("Request: Update asset - " + updateAssetDTO);
            var command = new UpdateAssetCommand(updateAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an asset logically (soft delete).
        /// </summary>
        /// <param name="deleteAssetDTO">DTO containing the Id of the asset to delete.</param>
        /// <returns>Returns status of the delete operation.</returns>
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
