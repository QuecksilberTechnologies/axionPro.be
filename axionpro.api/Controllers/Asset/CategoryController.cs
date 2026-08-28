// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Category operations.
// ================================================================

using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.AssetFeatures.Category.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Asset
{
    /// <summary>
    /// Controller to manage all Asset Category related operations 
    /// for Tenant Admins (Add, Update, Delete, GetAll).
    /// </summary>
    [ApiController]
    [Route("api/Asset/Category")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public CategoryController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region Tenant Admin - Asset Category CRUD

        /// <summary>
        /// Supports the Angular UI flow for get all asset category.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves all asset category.</para>
        /// <para>Angular page(s): /app/assets/asset-categories; /app/assets/asset-types; /app/assets/list.</para>
        /// <para>Angular API service call(s): AssetCategoriesApi.getAllAssetCategory (app/core/services/asset-categories-api.ts:36).</para>
        /// </remarks>
        [HttpGet("get")]        
        public async Task<IActionResult> GetAllAssetCategory([FromQuery] GetCategoryReqestDTO request)
        {
           
                _logger.LogInfo("Fetching all asset categories for tenant...");
                var query = new GetAllCategoryCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
                 
        }

        
        /// <summary>
        /// Supports the Angular UI flow for add asset category.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates asset category.</para>
        /// <para>Angular page(s): /app/assets/asset-categories.</para>
        /// <para>Angular API service call(s): AssetCategoriesApi.createAssetCategory (app/core/services/asset-categories-api.ts:29).</para>
        /// </remarks>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetCategory([FromBody] AddCategoryReqestDTO request)
        {
                _logger.LogInfo("Add Asset Category request received.");
                var command = new AddCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
        
        }

        /// <summary>
        /// Supports the Angular UI flow for update asset category.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates asset category.</para>
        /// <para>Angular page(s): /app/assets/asset-categories.</para>
        /// <para>Angular API service call(s): AssetCategoriesApi.updateAssetCategory (app/core/services/asset-categories-api.ts:43).</para>
        /// </remarks>
        [HttpPut("update")] 
        public async Task<IActionResult> UpdateAssetCategory([FromBody] UpdateCategoryReqestDTO request)
        {
           
                _logger.LogInfo($"Update Asset Category request received for ID: {request.Id}");
                var command = new UpdateCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Supports the Angular UI flow for delete asset category.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes asset category.</para>
        /// <para>Angular page(s): /app/assets/asset-categories.</para>
        /// <para>Angular API service call(s): AssetCategoriesApi.deleteAssetCategory (app/core/services/asset-categories-api.ts:50).</para>
        /// </remarks>
        [HttpDelete("delete")]     
        public async Task<IActionResult> DeleteAssetCategory([FromQuery] DeleteCategoryReqestDTO request)
        {

                _logger.LogInfo($"Delete Asset Category request received for ID: {request.Id}");
                var command = new DeleteCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
                
        }

        #endregion
    }
}
