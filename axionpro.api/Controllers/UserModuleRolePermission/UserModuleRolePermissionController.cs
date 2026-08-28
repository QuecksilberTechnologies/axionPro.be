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
    /// Supports the Angular UI flow for create permission.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: performs save role permissions.</para>
    /// <para>Angular page(s): /app/host-roles/permissions/:hostRoleId; /app/roles/permissions/:roleId.</para>
    /// <para>Angular API service call(s): RolesApi.saveRolePermissions (app/core/services/roles-api.ts:151).</para>
    /// </remarks>

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
    /// Supports the Angular UI flow for get tenant enabled operations.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves role based permissions.</para>
    /// <para>Angular page(s): /app/policies/attendance-policies; /auth/login; /app/admin-dashboard; /app/departments; /app/designations; /app/device-masters; /app/modules/module-operations; /app/modules/operations; and 24 more.</para>
    /// <para>Angular API service call(s): RolesApi.getRoleBasedPermissions (app/core/services/roles-api.ts:142).</para>
    /// </remarks>
    [HttpGet("get-role-based-permissions")]
    public async Task<IActionResult> GetTenantEnabledOperations([FromQuery] GetAllActiveRoleModuleOperationsRequestByRoleIdDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetRolePermissionCommand(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
