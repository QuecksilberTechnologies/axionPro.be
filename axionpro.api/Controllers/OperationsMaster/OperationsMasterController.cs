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
    /// Create Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to create operation.
    /// </remarks>
    /// <param name="requestDTO">The request body used to create operation.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Update Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to update operation.
    /// </remarks>
    /// <param name="requestDTO">The request body used to update operation.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Delete Operation.
    /// </summary>
    /// <remarks>
    /// Handles the request to delete operation.
    /// </remarks>
    /// <param name="operationId">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Get Operation By ID.
    /// </summary>
    /// <remarks>
    /// Handles the request to get operation by id.
    /// </remarks>
    /// <param name="operationId">The identifier supplied in the route.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
    /// Get All Operations.
    /// </summary>
    /// <remarks>
    /// Handles the request to get all operations.
    /// </remarks>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
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
