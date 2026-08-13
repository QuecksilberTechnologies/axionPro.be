// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes CRUD endpoints for OperationsMaster records.
// ================================================================

using axionpro.application.DTOs.Operation;
using axionpro.application.Features.OperationsMasterCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.OperationsMaster;

/// <summary>
/// Provides CRUD endpoints for operation master records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OperationsMasterController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationsMasterController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to dispatch commands and queries.</param>
    public OperationsMasterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates a new operation.
    /// </summary>
    /// <param name="requestDTO">The operation details to create.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The created operation response.</returns>
    [HttpPost("create-operation")]
    public async Task<IActionResult> CreateOperation(
        [FromBody] CreateOperationRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateOperationCommand(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Update

    /// <summary>
    /// Updates an existing operation.
    /// </summary>
    /// <param name="requestDTO">The operation details to update.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The updated operation response.</returns>
    [HttpPost("update-operation")]
    public async Task<IActionResult> UpdateOperation(
        [FromBody] UpdateOperationRequestDTO requestDTO,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateOperationCommand(requestDTO),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Deactivates an operation by ID.
    /// </summary>
    /// <param name="operationId">The ID of the operation to delete.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The deletion response.</returns>
    [HttpDelete("delete-operation/{operationId:int}")]
    public async Task<IActionResult> DeleteOperation(
        int operationId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteOperationCommand(operationId),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region GetById

    /// <summary>
    /// Retrieves an operation by ID.
    /// </summary>
    /// <param name="operationId">The ID of the operation to retrieve.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The requested operation.</returns>
    [HttpGet("get-operation/{operationId:int}")]
    public async Task<IActionResult> GetOperationById(
        int operationId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOperationByIdQuery(operationId),
            cancellationToken);

        return Ok(result);
    }

    #endregion

    #region GetAll

    /// <summary>
    /// Retrieves all operations.
    /// </summary>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>All operation records.</returns>
    [HttpGet("get-all-operations")]
    public async Task<IActionResult> GetAllOperations(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllOperationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    #endregion
}
