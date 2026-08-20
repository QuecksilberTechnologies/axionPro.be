// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes parent-module management endpoints and delegates requests through MediatR.
// ================================================================

using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module
{
    /// <summary>
    /// Coordinates Host-admin HTTP requests for Parent/Header Modules.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        /// Creates a new Header Module for the authenticated Host user.
        /// </summary>
        /// <param name="createModuleRequestDTO">The client-editable Header Module values.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The created Header Module response.</returns>
        [HttpPost("add")]
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
        /// Updates the editable values of an existing Header Module.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="updateModuleRequestDTO">The editable Header Module values.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The updated Header Module response.</returns>
        [HttpPut("{id:int}")]
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
        /// Updates a Parent Module's active state and cascades the same value to its direct child modules and their operation mappings.
        /// </summary>
        /// <param name="id">The Parent Module identifier.</param>
        /// <param name="statusRequestDTO">The required target active state.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The Parent Module after the status cascade completes.</returns>
        [HttpPatch("{id:int}/status")]
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
        /// Retrieves one Header Module in the requested module scope.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The matching Header Module response.</returns>
        [HttpGet("{id:int}")]
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
        /// Retrieves Header Modules in the requested module scope.
        /// </summary>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="isActive">When supplied, filters modules by active state.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The ordered Header Module list.</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetModules(
            [FromQuery] short moduleScope,
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetParentModulesQuery(moduleScope, isActive),
                cancellationToken);

            return Ok(result);
        }

        #endregion

        #region Existing Compatibility Endpoint

        /// <summary>
        /// Retains the existing module-header tree endpoint for current callers.
        /// </summary>
        /// <param name="getModuleDDLRequestDTO">The legacy tree request values.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The existing non-leaf module tree response.</returns>
        [HttpPost("get-non-leafe")]
        public async Task<IActionResult> GetOperationalModule(
            [FromBody] GetModuleChildInversRequestDTO? getModuleDDLRequestDTO,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetModuleHeadersCommand(getModuleDDLRequestDTO!),
                cancellationToken);

            return Ok(result);
        }

        #endregion
    }
}
