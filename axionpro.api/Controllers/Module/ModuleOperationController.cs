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
    /// Creates a module-operation mapping for the authenticated Host user.
    /// </summary>
    /// <param name="dto">The client-editable mapping values.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The created module-operation mapping.</returns>
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
    /// Updates a module-operation mapping for the authenticated Host user.
    /// </summary>
    /// <param name="dto">The client-editable mapping values.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The updated module-operation mapping.</returns>
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
    /// Deactivates a module-operation mapping for the authenticated Host user.
    /// </summary>
    /// <param name="id">The mapping identifier to deactivate.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The deletion response.</returns>
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
    /// Retrieves one module-operation mapping for the authenticated Host user.
    /// </summary>
    /// <param name="id">The mapping identifier to retrieve.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The requested module-operation mapping.</returns>
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
    /// Retrieves all module-operation mappings for the authenticated Host user.
    /// </summary>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>All module-operation mappings.</returns>
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
