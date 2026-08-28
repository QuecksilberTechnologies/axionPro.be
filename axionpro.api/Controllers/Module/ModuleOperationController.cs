// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes HostUser-controlled CRUD endpoints for ModuleOperation mappings.
// ================================================================

using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.ModuleOperation;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Module;

/// <summary>
/// Coordinates HostUser HTTP requests for module-operation mappings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModuleOperationController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleOperationController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches module-operation commands and queries.</param>
    public ModuleOperationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region ModuleOperation CRUD

    /// <summary>
    /// Supports the Angular UI flow for create module operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: creates module operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations.</para>
    /// <para>Angular API service call(s): ModuleOperationApi.addModuleOperation (app/core/services/module-operation-api.ts:37).</para>
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> CreateModuleOperation(
        [FromBody] CreateModuleOperationRequestDTO? dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateModuleOperationCommand(dto),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for update module operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates module operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations.</para>
    /// <para>Angular API service call(s): ModuleOperationApi.updateModuleOperation (app/core/services/module-operation-api.ts:44).</para>
    /// </remarks>
    [HttpPost("update")]
    public async Task<IActionResult> UpdateModuleOperation(
        [FromBody] UpdateModuleOperationMappingByProductOwnerRequestDTO? dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateModuleOperationMappingByProductOwnerCommand(dto),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for delete module operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes module operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations.</para>
    /// <para>Angular API service call(s): ModuleOperationApi.deleteModuleOperation (app/core/services/module-operation-api.ts:50).</para>
    /// </remarks>
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteModuleOperation(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteModuleOperationCommand(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for get module operation by id.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves module operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations.</para>
    /// <para>Angular API service call(s): ModuleOperationApi.getModuleOperation (app/core/services/module-operation-api.ts:31).</para>
    /// </remarks>
    [HttpGet("get-by-id/{id:int}")]
    public async Task<IActionResult> GetModuleOperationById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetModuleOperationByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Supports the Angular UI flow for get all module operations.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves module operations.</para>
    /// <para>Angular page(s): /app/modules/module-operations.</para>
    /// <para>Angular API service call(s): ModuleOperationApi.getModuleOperations (app/core/services/module-operation-api.ts:25).</para>
    /// </remarks>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllModuleOperations(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllModuleOperationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
