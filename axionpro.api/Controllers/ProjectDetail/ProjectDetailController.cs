// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes public read-only project feature-page metadata.
// ================================================================

using axionpro.application.Features.FeaturePages.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.ProjectDetail;

/// <summary>
/// Exposes active master feature headers, operational pages, and their operation metadata.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ProjectDetailController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectDetailController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches feature-page queries.</param>
    public ProjectDetailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves active feature headers, their child headers, operational leaf pages, and leaf-page operation metadata.
    /// </summary>
    /// <remarks>
    /// This public read-only endpoint intentionally has no request validation or permission check.
    /// </remarks>
    /// <param name="scope">Optional scope: 1 for Tenant, 2 for Host, 3 for Common; omit for all scopes.</param>
    /// <param name="cancellationToken">A token used to cancel request processing.</param>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(
        [FromQuery(Name = "Scope")] short? scope,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFeaturePagesQuery(scope), cancellationToken);
        return Ok(result);
    }
}
