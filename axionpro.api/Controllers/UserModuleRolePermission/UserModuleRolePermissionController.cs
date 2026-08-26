// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for User Module Role Permission operations.
// ================================================================

using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.UserModuleRolePermission;

[ApiController]
[Route("api/[controller]")]
public class UserModuleRolePermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    public UserModuleRolePermissionController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    /// <summary>
    /// Create Permission.
    /// </summary>
    /// <remarks>
    /// Handles the request to create permission.
    /// </remarks>
    /// <param name="insertRoleModulePermissionsRequestDTO">The request body used to create permission.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>

    [HttpPost("assign-role-permissions")]    
    
    // [Authorize]
    public async Task<IActionResult> CreatePermission([FromBody] CreateModuleOperationRolePermissionsRequestDTO insertRoleModulePermissionsRequestDTO)
    {
        _logger.LogInfo("Received request for update a new role" + insertRoleModulePermissionsRequestDTO.ToString());
        var command = new CreateRolePermissionCommand(insertRoleModulePermissionsRequestDTO);
        var result = await _mediator.Send(command);
       
        return Ok(result);
    }
    /// <summary>
    /// Get Tenant Enabled Operations.
    /// </summary>
    /// <remarks>
    /// Handles the request to get tenant enabled operations.
    /// </remarks>
    /// <param name="code">The query parameters used to get tenant enabled operations.</param>
    /// <returns>An HTTP response containing the result of the operation.</returns>
    [HttpGet("get-role-based-permissions")]
    public async Task<IActionResult> GetTenantEnabledOperations([FromQuery] GetAllActiveRoleModuleOperationsRequestByRoleIdDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetRolePermissionCommand(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
