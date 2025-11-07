using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Features.AssetFeatures.Type.Commands;
using axionpro.application.Interfaces.ILogger;
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
        /// Retrieves all asset types for a specific tenant.
        /// </summary>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAssetType([FromQuery] GetTypeRequestDTO request)
        {
            try
            {
                _logger.LogInfo("Fetching all asset types for tenant...");
                var query = new GetAllTypeCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching asset types: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while retrieving asset types." });
            }
        }

    
        /// <summary>
        /// Adds a new asset type record for a tenant.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetType([FromBody] AddTypeRequestDTO request)
        {
            try
            {
                _logger.LogInfo("Add Asset Type request received.");
                var command = new AddTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding asset type: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to add asset type." });
            }
        }

        /// <summary>
        /// Updates an existing asset type record.
        /// </summary>
        // [HttpPost("update")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssetType([FromBody] UpdateTypeRequestDTO request)
        {
            try
            {
                _logger.LogInfo($"Update Asset Type request received for ID: {request.Id}");
                var command = new UpdateAssetTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating asset type (ID: {request.Id}): {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to update asset type." });
            }
        }

        /// <summary>
        /// Deletes an existing asset type record (soft delete).
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAssetType([FromQuery] DeleteTypeRequestDTO request)
        {
            try
            {
                _logger.LogInfo($"Delete Asset Type request received for ID: {request.Id}");
                var command = new DeletetTypeCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting asset type (ID: {request.Id}): {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "Failed to delete asset type." });
            }
        }

        #endregion
    }
}
