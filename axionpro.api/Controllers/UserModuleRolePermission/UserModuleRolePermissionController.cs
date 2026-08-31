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
    /// Used-In-Angular: updates role permissions.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): RolesApi.saveRolePermissions (app/core/services/roles-api.ts:155).</para>
    /// <para>Angular purpose: updates role permissions.</para>
    /// <para>Integrated UI page(s): /app/roles/permissions/:roleId</para>
    /// <para>Angular UI component(s): RolePermissionsStore (app/features/roles/role-permissions/role-permissions.store.ts); RolePermissions (app/features/roles/role-permissions/role-permissions.ts)</para>
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
    /// Used-In-Angular: retrieves role based permissions.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): RolesApi.getRoleBasedPermissions (app/core/services/roles-api.ts:146).</para>
    /// <para>Angular purpose: retrieves role based permissions.</para>
    /// <para>Integrated UI page(s): /app/policies/attendance-policies; /auth/login; /app/admin-dashboard; /app/departments; /app/designations; /app/device-masters; /app/modules/module-operations; /app/modules/operations</para>
    /// <para>Angular UI component(s): CurrentUserPermissionsStore (app/core/stores/current-user-permissions.store.ts); RolePermissionsStore (app/features/roles/role-permissions/role-permissions.store.ts); hasModuleOperationGuard (app/core/guards/has-module-operation-guard.ts); hasModulePermissionGuard (app/core/guards/has-module-permission-guard.ts); superAdminGuard (app/core/guards/super-admin-guard.ts); AttendancePolicies (app/features/attendance-policies/attendance-policies.ts); Login (app/features/authentication/login/login.ts); DashboardAdmin (app/features/dashboard/dashboard-admin/dashboard-admin.ts)</para>
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
