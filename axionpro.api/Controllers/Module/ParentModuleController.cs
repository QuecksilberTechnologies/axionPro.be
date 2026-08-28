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
        /// Supports the Angular UI flow for add module.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates parent module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): ParentModuleApi.addParentModule (app/core/services/parent-module-api.ts:41).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update module.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates parent module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): ParentModuleApi.updateParentModule (app/core/services/parent-module-api.ts:47).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update module status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates parent module status.</para>
        /// <para>Angular page(s): /app/modules.</para>
        /// <para>Angular API service call(s): ParentModuleApi.setParentModuleStatus (app/core/services/parent-module-api.ts:53).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get module by id.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves parent module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules; /app/modules/module-operations.</para>
        /// <para>Angular API service call(s): ParentModuleApi.getParentModule (app/core/services/parent-module-api.ts:35).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get module headers.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves module headers.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules; /app/modules/module-operations.</para>
        /// <para>Angular API service call(s): ParentModuleApi.getModuleHeaders (app/core/services/parent-module-api.ts:28).</para>
        /// </remarks>
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
