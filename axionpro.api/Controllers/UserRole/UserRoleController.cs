// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for User Role operations.
// ================================================================

using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.UserRoles;
using axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Handlers;
using axionpro.application.Features.UserRolesCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.UserRole;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    public UserRoleController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    /// <summary>
    /// Used-In-Angular: assigns or maps roles to user.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): RolesApi.assignRolesToUser (app/core/services/roles-api.ts:172).</para>
    /// <para>Angular purpose: assigns or maps roles to user.</para>
    /// <para>Integrated UI page(s): /app/employees</para>
    /// <para>Angular UI component(s): EmployeesStore (app/features/employees/employees.store.ts); Employees (app/features/employees/employees.ts)</para>
    /// </remarks>

    [HttpPost("assign-roles-to-user")]    
    
    // [Authorize]
    public async Task<IActionResult> CreatePermission([FromBody] UserRoleListDTO insertRoleModulePermissionsRequestDTO)
    {
        _logger.LogInfo("Received request for update a new role" + insertRoleModulePermissionsRequestDTO.ToString());
        var command = new AssignUserRolesCommand(insertRoleModulePermissionsRequestDTO);
        var result = await _mediator.Send(command);
       
        return Ok(result);
    }
    /// <summary>
    /// Used-In-Angular: retrieves all user roles.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): RolesApi.getAllUserRoles (app/core/services/roles-api.ts:166).</para>
    /// <para>Angular purpose: retrieves all user roles.</para>
    /// <para>Integrated UI page(s): /app/employees</para>
    /// <para>Angular UI component(s): RolePopup (app/features/employees/role-popup/role-popup.ts); EmployeeRoleCell (app/features/employees/employee-role-cell/employee-role-cell.ts); Employees (app/features/employees/employees.ts)</para>
    /// </remarks>
    [HttpGet("get-all-user-roles")]
    public async Task<IActionResult> GetTenantEnabledOperations([FromQuery] GetUserRoleRequestDTO dTO)
    {
        _logger.LogInfo($"Getting all user roles: {dTO}");

        var query = new GetAllUserRolesCommand(dTO);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
