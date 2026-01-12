using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Features.AssetFeatures.Category.Handlers;
using axionpro.application.Interfaces.ILogger;
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
        /// Retrieves all asset categories for a specific tenant.
        /// </summary>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAssetCategory([FromQuery] GetCategoryReqestDTO request)
        {
            try
            {
                _logger.LogInfo("Fetching all asset categories for tenant...");
                var query = new GetAllCategoryCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching asset categories: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while retrieving asset categories." });
            }
        }

        
        /// <summary>
        /// Adds a new asset category record for a tenant.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetCategory([FromBody] AddCategoryReqestDTO request)
        {
            try
            {
                _logger.LogInfo("Add Asset Category request received.");
                var command = new AddCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding asset category: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to add asset category." });
            }
        }

        /// <summary>
        /// Updates an existing asset category record.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssetCategory([FromBody] UpdateCategoryReqestDTO request)
        {
            try
            {
                _logger.LogInfo($"Update Asset Category request received for ID: {request.Id}");
                var command = new UpdateCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating asset category (ID: {request.Id}): {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to update asset category." });
            }
        }

        /// <summary>
        /// Deletes an existing asset category record (soft delete).
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAssetCategory([FromQuery] DeleteCategoryReqestDTO request)
        {
            try
            {
                _logger.LogInfo($"Delete Asset Category request received for ID: {request.Id}");
                var command = new DeleteCategoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting asset category (ID: {request.Id}): {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to delete asset category." });
            }
        }

        #endregion
    }
}
