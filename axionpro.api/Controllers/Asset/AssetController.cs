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
        /// Supports the Angular UI flow for get all assets.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves assets.</para>
        /// <para>Angular page(s): /app/assets/list.</para>
        /// <para>Angular API service call(s): AssetsApi.getAssets (app/core/services/assets-api.ts:35).</para>
        /// </remarks>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllAssets([FromQuery] GetAssetRequestDTO assetRequestDTO)
        {
            var command = new GetAllAssetCommand(assetRequestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for add asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates asset.</para>
        /// <para>Angular page(s): /app/assets/list.</para>
        /// <para>Angular API service call(s): AssetsApi.createAsset (app/core/services/assets-api.ts:28).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates asset.</para>
        /// <para>Angular page(s): /app/assets/list.</para>
        /// <para>Angular API service call(s): AssetsApi.updateAsset (app/core/services/assets-api.ts:41).</para>
        /// </remarks>
        [HttpPut("update")]    
        
        public async Task<IActionResult> UpdateAsset([FromForm] UpdateAssetRequestDTO updateAssetDTO)
        {
            _logger.LogInfo("Request: Update asset - " + updateAssetDTO);
            var command = new UpdateAssetCommand(updateAssetDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for delete asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes asset.</para>
        /// <para>Angular page(s): /app/assets/list.</para>
        /// <para>Angular API service call(s): AssetsApi.deleteAsset (app/core/services/assets-api.ts:48).</para>
        /// </remarks>
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
