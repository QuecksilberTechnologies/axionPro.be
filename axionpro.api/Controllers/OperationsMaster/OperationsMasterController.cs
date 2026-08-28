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
    /// Supports the Angular UI flow for create operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: creates operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations; /app/modules/operations.</para>
    /// <para>Angular API service call(s): OperationsMasterApi.addOperation (app/core/services/operations-master-api.ts:37).</para>
    /// </remarks>
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
    /// Supports the Angular UI flow for update operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: updates operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations; /app/modules/operations.</para>
    /// <para>Angular API service call(s): OperationsMasterApi.updateOperation (app/core/services/operations-master-api.ts:44).</para>
    /// </remarks>
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
    /// Supports the Angular UI flow for delete operation.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations; /app/modules/operations.</para>
    /// <para>Angular API service call(s): OperationsMasterApi.deleteOperation (app/core/services/operations-master-api.ts:50).</para>
    /// </remarks>
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
    /// Supports the Angular UI flow for get operation by id.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves operation.</para>
    /// <para>Angular page(s): /app/modules/module-operations; /app/modules/operations.</para>
    /// <para>Angular API service call(s): OperationsMasterApi.getOperation (app/core/services/operations-master-api.ts:31).</para>
    /// </remarks>
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
    /// Supports the Angular UI flow for get all operations.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves operations.</para>
    /// <para>Angular page(s): /app/modules/module-operations; /app/modules/operations.</para>
    /// <para>Angular API service call(s): OperationsMasterApi.getOperations (app/core/services/operations-master-api.ts:25).</para>
    /// </remarks>
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
