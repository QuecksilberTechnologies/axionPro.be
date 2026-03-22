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
        /// Retrieves all asset statuses based on tenant context or filters.
        /// </summary>
        /// <param name="request">Request DTO containing filter criteria.</param>
        /// <returns>Returns a list of asset statuses.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetByIdAssetStatus([FromQuery] GetStatusRequestDTO request)
        {
                _logger.LogInfo("Fetching all asset statuses for tenant...");
                var query = new GetAllAssetStatusCommand(request);
                var result = await _mediator.Send(query);
                return Ok(result);
        }

        /// <summary>
        /// Adds a new asset status record for a tenant.
        /// </summary>
        /// <param name="request">Request DTO containing asset status details.</param>
        /// <returns>Returns success message after insertion.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddAssetStatus([FromBody] CreateStatusRequestDTO request)
        {

            _logger.LogInfo("Add Asset Status request received.");
            var command = new AddStatusCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing asset status for the given ID.
        /// </summary>
        /// <param name="id">Asset status ID to be updated.</param>
        /// <param name="request">Request DTO with updated data.</param>
        /// <returns>Returns success message after update.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssetStatus( [FromBody] UpdateStatusRequestDTO request)
        {
             
                _logger.LogInfo($"Update request received for Asset Status ID: {request.Id}");
                var command = new UpdateStatusCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
           
        }

        /// <summary>
        /// Deletes an existing asset status record.
        /// </summary>
        /// <param name="request">Request DTO containing ID of status to delete.</param>
        /// <returns>Returns success message after deletion.</returns>
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
 