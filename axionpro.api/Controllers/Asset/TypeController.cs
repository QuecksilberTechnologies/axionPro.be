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
        /// Used-In-Angular: retrieves assets types.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetTypesApi.fetchAssetsTypes (app/core/services/asset-types-api.ts:39).</para>
        /// <para>Angular purpose: retrieves assets types.</para>
        /// <para>Integrated UI page(s): /app/assets/asset-types; /app/assets/list</para>
        /// <para>Angular UI component(s): AssetTypesStore (app/features/assets-management/asset-types/asset-types.store.ts); AssetsManagementStore (app/features/assets-management/assets-management.store.ts); AssetFilter (app/shared/components/asset/asset-filter/asset-filter.ts); UpsertAssetDialogStore (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.store.ts); AssetTypes (app/features/assets-management/asset-types/asset-types.ts); AssetsManagement (app/features/assets-management/assets-management.ts); UpsertAssetDialog (app/shared/components/asset/upsert-asset-dialog/upsert-asset-dialog.ts)</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAssetType([FromQuery] GetTypeRequestDTO request)
        {
           
                _logger.LogInfo("Fetching all asset types for tenant...");
                var query = new GetAllTypeCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
          
        }

    
        /// <summary>
        /// Used-In-Angular: creates asset type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetTypesApi.createAssetType (app/core/services/asset-types-api.ts:32).</para>
        /// <para>Angular purpose: creates asset type.</para>
        /// <para>Integrated UI page(s): /app/assets/asset-types</para>
        /// <para>Angular UI component(s): AssetTypeManageDialog (app/shared/components/asset/asset-type-manage-dialog/asset-type-manage-dialog.ts); AssetTypes (app/features/assets-management/asset-types/asset-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: updates asset type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetTypesApi.updateAssetType (app/core/services/asset-types-api.ts:45).</para>
        /// <para>Angular purpose: updates asset type.</para>
        /// <para>Integrated UI page(s): /app/assets/asset-types</para>
        /// <para>Angular UI component(s): AssetTypeManageDialog (app/shared/components/asset/asset-type-manage-dialog/asset-type-manage-dialog.ts); AssetTypes (app/features/assets-management/asset-types/asset-types.ts)</para>
        /// </remarks>
        [HttpPut("update")] 
        public async Task<IActionResult> UpdateAssetType([FromBody] UpdateTypeRequestDTO request)
        {
            
                _logger.LogInfo($"Update Asset Type request received for ID: {request.Id}");
                var command = new UpdateAssetTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Used-In-Angular: deletes asset type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): AssetTypesApi.deleteAssetType (app/core/services/asset-types-api.ts:52).</para>
        /// <para>Angular purpose: deletes asset type.</para>
        /// <para>Integrated UI page(s): /app/assets/asset-types</para>
        /// <para>Angular UI component(s): AssetTypesStore (app/features/assets-management/asset-types/asset-types.store.ts); AssetTypes (app/features/assets-management/asset-types/asset-types.ts)</para>
        /// </remarks>
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
