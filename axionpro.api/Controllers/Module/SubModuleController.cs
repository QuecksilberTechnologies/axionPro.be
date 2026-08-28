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
        /// Supports the Angular UI flow for add module.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates sub module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): SubModuleApi.addSubModule (app/core/services/sub-module-api.ts:40).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update module.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates sub module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): SubModuleApi.updateSubModule (app/core/services/sub-module-api.ts:46).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get module by id.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves sub module.</para>
        /// <para>Angular page(s): /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): SubModuleApi.getSubModule (app/core/services/sub-module-api.ts:34).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get modules.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves sub modules.</para>
        /// <para>Angular page(s): /app/modules/module-operations; /app/modules; /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): SubModuleApi.getSubModules (app/core/services/sub-module-api.ts:27).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update module status.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates sub module status.</para>
        /// <para>Angular page(s): /app/modules/sub-modules.</para>
        /// <para>Angular API service call(s): SubModuleApi.setSubModuleStatus (app/core/services/sub-module-api.ts:52).</para>
        /// </remarks>
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
