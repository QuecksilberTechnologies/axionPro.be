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
        /// Used-In-Angular: retrieves assets.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetsApi.getAssets (app/core/services/assets-api.ts:36).</para>
        /// <para>Angular purpose: retrieves assets.</para>
        /// <para>Integrated UI page(s): /app/assets/list</para>
        /// <para>Angular UI component(s): AssetsManagementStore (app/features/assets-management/assets-management.store.ts); AssetsManagement (app/features/assets-management/assets-management.ts)</para>
        /// </remarks>
        [HttpGet("get")] 
        public async Task<IActionResult> GetAllAssets([FromQuery] GetAssetRequestDTO assetRequestDTO)
        {
            var command = new GetAllAssetCommand(assetRequestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: creates asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetsApi.createAsset (app/core/services/assets-api.ts:29).</para>
        /// <para>Angular purpose: creates asset.</para>
        /// <para>Integrated UI page(s): /app/assets/list</para>
        /// <para>Angular UI component(s): UpsertAssetDialogStore (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.store.ts); UpsertAssetDialog (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.ts); AssetsManagement (app/features/assets-management/assets-management.ts)</para>
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
        /// Used-In-Angular: updates asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetsApi.updateAsset (app/core/services/assets-api.ts:42).</para>
        /// <para>Angular purpose: updates asset.</para>
        /// <para>Integrated UI page(s): /app/assets/list</para>
        /// <para>Angular UI component(s): UpsertAssetDialogStore (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.store.ts); UpsertAssetDialog (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.ts); AssetsManagement (app/features/assets-management/assets-management.ts)</para>
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
        /// Used-In-Angular: deletes asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetsApi.deleteAsset (app/core/services/assets-api.ts:49).</para>
        /// <para>Angular purpose: deletes asset.</para>
        /// <para>Integrated UI page(s): /app/assets/list</para>
        /// <para>Angular UI component(s): AssetsManagementStore (app/features/assets-management/assets-management.store.ts); AssetsManagement (app/features/assets-management/assets-management.ts)</para>
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
