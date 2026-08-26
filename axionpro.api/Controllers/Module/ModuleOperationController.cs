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
    /// Create Module Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to create module operation.
    /// </remarks>
    /// <param name="dto">The request body used to create module operation.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Update Module Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to update module operation.
    /// </remarks>
    /// <param name="dto">The request body used to update module operation.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Delete Module Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete module operation.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Get Module Operation By ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get module operation by id.
    /// </remarks>
    /// <param name="id">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Get All Module Operations.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all module operations.
    /// </remarks>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
