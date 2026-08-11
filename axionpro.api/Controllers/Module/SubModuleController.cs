// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Exposes Host-admin controlled direct SubModule CRUD endpoints.
// ============================================================================

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
        /// Creates a direct child SubModule for the authenticated Host user.
        /// </summary>
        /// <param name="createSubModuleRequestDTO">The client-editable child Module values.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The created SubModule response.</returns>
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
        /// Updates a direct child SubModule and, when requested, moves it to a compatible Header Module.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="updateSubModuleRequestDTO">The editable SubModule values.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The updated SubModule response.</returns>
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
        /// Retrieves one direct child SubModule in the required module scope.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The matching SubModule response.</returns>
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
        /// Retrieves direct child SubModules in the required module scope.
        /// </summary>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="parentModuleId">When supplied, filters by direct Header Module identifier.</param>
        /// <param name="isActive">When supplied, filters by active state.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The ordered SubModule list.</returns>
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
        /// Retrieves direct children for a validated Header Module.
        /// </summary>
        /// <param name="parentModuleId">The Header Module identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="isActive">When supplied, filters by active state.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The ordered direct-child SubModule list.</returns>
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
        /// Changes the active state of a direct SubModule without deleting it.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="statusRequestDTO">The required target active state.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The SubModule after its status changes.</returns>
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
