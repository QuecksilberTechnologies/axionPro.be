// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates Host-admin HTTP requests for Parent/Header Modules.
// ================================================================

using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module
{
    /// <summary>
    /// Provides Parent/Header Module management endpoints for authenticated Host Super Admins.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParentModuleController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentModuleController"/> class.
        /// </summary>
        /// <param name="mediator">Dispatches Parent Module commands and queries.</param>
        public ParentModuleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        #region Parent Module Commands

        /// <summary>
        /// Create Parent/Header Module.
        /// </summary>
        /// <remarks>
        /// Creates a new root Parent/Header Module from the supplied editable values and supported module scope.
        /// Requires an authenticated Host user whose current Host role is the verified Super Admin role.
        /// </remarks>
        /// <param name="createModuleRequestDTO">The editable Parent/Header Module values, including its requested module scope.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The created Parent/Header Module response.</returns>
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddModule(
            [FromBody] CreateParentModuleRequestDTO? createModuleRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreateParentModuleCommand(createModuleRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update Parent/Header Module.
        /// </summary>
        /// <remarks>
        /// Updates the editable values of an existing Parent/Header Module without changing its ownership or hierarchy.
        /// Requires an authenticated Host user whose current Host role is the verified Super Admin role.
        /// </remarks>
        /// <param name="id">The Parent/Header Module identifier.</param>
        /// <param name="updateModuleRequestDTO">The editable Parent/Header Module values and module scope used to locate the existing header.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The updated Parent/Header Module response.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateModule(
            int id,
            [FromBody] UpdateParentModuleRequestDTO? updateModuleRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateParentModuleCommand(id, updateModuleRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update Parent/Header Module status.
        /// </summary>
        /// <remarks>
        /// Updates the active and visible state of the selected Parent/Header Module and cascades the same state downward to all descendants and directly linked module-operation mappings.
        /// Ancestor modules and sibling branches are not affected.
        /// Requires an authenticated Host user whose current Host role is the verified Super Admin role.
        /// </remarks>
        /// <param name="id">The Parent/Header Module identifier.</param>
        /// <param name="statusRequestDTO">The target active state and module scope used to locate the Parent/Header Module.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The Parent/Header Module after the status cascade completes.</returns>
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
                    public async Task<IActionResult> UpdateModuleStatus(
            int id,
            [FromBody] UpdateParentModuleStatusRequestDTO? statusRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateParentModuleStatusCommand(id, statusRequestDTO),
                cancellationToken);

            return Ok(result);
        }

        #endregion

        #region Parent Module Queries

        /// <summary>
        /// Get Parent/Header Module by ID.
        /// </summary>
        /// <remarks>
        /// Retrieves one Parent/Header Module in the requested module scope.
        /// Requires an authenticated Host user whose current Host role is the verified Super Admin role.
        /// </remarks>
        /// <param name="id">The Parent/Header Module identifier.</param>
        /// <param name="moduleScope">The module scope required to locate the Parent/Header Module.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The matching Parent/Header Module response.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
                    public async Task<IActionResult> GetModuleById(
            int id,
            [FromQuery] short moduleScope,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetParentModuleByIdQuery(id, moduleScope),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get Module Headers.
        /// </summary>
        /// <remarks>
        /// Retrieves visible top-level Parent/Header Modules for the requested module scope and only their qualifying direct non-leaf Sub-Parent Headers.
        /// A Parent/Header Module remains in the response with an empty <c>Children</c> collection when it has no qualifying child headers. Leaf modules are not returned.
        /// The optional <c>IsActive</c> filter returns active headers when <c>true</c>, inactive headers when <c>false</c>, and does not filter by active status when omitted.
        /// Requires an authenticated Host user whose current Host role is the verified Super Admin role.
        /// </remarks>
        /// <param name="requestDTO">Query filters: <c>ModuleScope</c> selects the requested scope; optional <c>IsActive</c> filters active or inactive headers.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The module-header tree response.</returns>
        [HttpGet("get-module-headers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
                public async Task<IActionResult> GetModuleHeaders(
            [FromQuery] GetParentModuleFilterRequestDTO requestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetModuleHeadersCommand(requestDTO),
                cancellationToken);

            return Ok(result);
        }

        #endregion
    }
}
