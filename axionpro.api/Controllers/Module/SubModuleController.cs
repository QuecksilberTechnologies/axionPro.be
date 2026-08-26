// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates Host-admin requests for direct SubModule management.
// ================================================================

using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module
{
    /// <summary>
    /// Coordinates Host-admin HTTP requests for direct SubModules.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubModuleController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleController"/> class.
        /// </summary>
        /// <param name="mediator">Dispatches SubModule commands and queries.</param>
        public SubModuleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        #region SubModule CRUD

        /// <summary>
        /// Add Module.
        /// </summary>
        /// <remarks>
        /// Handles the request to add module.
        /// </remarks>
        /// <param name="createSubModuleRequestDTO">The request body used to add module.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddModule(
            [FromBody] CreateSubModuleRequestDTO? createSubModuleRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreateSubModuleCommand(createSubModuleRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update Module.
        /// </summary>
        /// <remarks>
        /// Handles the request to update module.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <param name="updateSubModuleRequestDTO">The request body used to update module.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateModule(
            int id,
            [FromBody] UpdateSubModuleRequestDTO? updateSubModuleRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateSubModuleCommand(id, updateSubModuleRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get Module By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get module by id.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <param name="moduleScope">The query parameters used to get module by id.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetModuleById(
            int id,
            [FromQuery] short moduleScope,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSubModuleByIdQuery(id, moduleScope),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get Modules.
        /// </summary>
        /// <remarks>
        /// Handles the request to get modules.
        /// </remarks>
        /// <param name="moduleScope">The query parameters used to get modules.</param>
        /// <param name="parentModuleId">The query parameters used to get modules.</param>
        /// <param name="isActive">The query parameters used to get modules.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetModules(
            [FromQuery] short moduleScope,
            [FromQuery] int? parentModuleId,
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSubModulesQuery(moduleScope, parentModuleId, isActive),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get Modules By Parent.
        /// </summary>
        /// <remarks>
        /// Handles the request to get modules by parent.
        /// </remarks>
        /// <param name="parentModuleId">The identifier supplied in the route.</param>
        /// <param name="moduleScope">The query parameters used to get modules by parent.</param>
        /// <param name="isActive">The query parameters used to get modules by parent.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("parent/{parentModuleId:int}")]
        public async Task<IActionResult> GetModulesByParent(
            int parentModuleId,
            [FromQuery] short moduleScope,
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSubModulesByParentQuery(parentModuleId, moduleScope, isActive),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update Module Status.
        /// </summary>
        /// <remarks>
        /// Handles the request to update module status.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <param name="statusRequestDTO">The request body used to update module status.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateModuleStatus(
            int id,
            [FromBody] UpdateSubModuleStatusRequestDTO? statusRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateSubModuleStatusCommand(id, statusRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        #endregion
    }
}
