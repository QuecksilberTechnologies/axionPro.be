// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes HTTP endpoints for Role operations.
// ================================================================

using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Role;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Role
{
    /// <summary>
    /// Controller to manage Roles in the system.
    /// Provides endpoints for creating, updating, retrieving, and deleting roles.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="RoleController"/>.
        /// </summary>
        /// <param name="mediator">Mediator service for handling commands and queries.</param>
        /// <param name="logger">Logger service for logging information and errors.</param>
        public RoleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Used-In-Angular: updates role.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): RolesApi.updateRole (app/core/services/roles-api.ts:125).</para>
        /// <para>Angular purpose: updates role.</para>
        /// <para>Integrated UI page(s): /app/roles</para>
        /// <para>Angular UI component(s): RoleDialog (app/features/roles/role-dialog/role-dialog.ts); RolesList (app/features/roles/roles-list/roles-list.ts)</para>
        /// </remarks>
        [HttpPut("update")]        
        // [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequestDTO updateRoleDTO)
        {
            _logger.LogInfo("Received request to update a role: " + updateRoleDTO.ToString());
            var command = new UpdateRoleCommand(updateRoleDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Used-In-Angular: retrieves role options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): RolesApi.getRoleOptions (app/core/services/roles-api.ts:119).</para>
        /// <para>Angular purpose: retrieves role options.</para>
        /// <para>Integrated UI page(s): /app/tickets/types; /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): RolePopup (app/features/employees/role-popup/role-popup.ts); TicketTypeManageDialog (app/features/tickets/ticket-type/ticket-type-manage-dialog/ticket-type-manage-dialog.ts); EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); EmployeeRoleCell (app/features/employees/employee-role-cell/employee-role-cell.ts); TicketTypeComponent (app/features/tickets/ticket-type/ticket-type.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
        /// </remarks>
        [HttpGet("option")]      
        public async Task<IActionResult> getRole([FromQuery] GetRoleOptionRequestDTO requestDTO)
        {
            _logger.LogInfo("Received request to get role options.");

            var command = new GetRoleOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: creates role.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): RolesApi.addRole (app/core/services/roles-api.ts:105).</para>
        /// <para>Angular purpose: creates role.</para>
        /// <para>Integrated UI page(s): /app/roles</para>
        /// <para>Angular UI component(s): RoleDialog (app/features/roles/role-dialog/role-dialog.ts); RolesList (app/features/roles/roles-list/roles-list.ts)</para>
        /// </remarks>
        [HttpPost("add")]        
        // [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDTO createRoleDTO)
        {
            _logger.LogInfo("Received request to create a new role: " + createRoleDTO.ToString());
            var command = new CreateRoleCommand(createRoleDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: retrieves roles.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): RolesApi.getRoles (app/core/services/roles-api.ts:112).</para>
        /// <para>Angular purpose: retrieves roles.</para>
        /// <para>Integrated UI page(s): /app/roles; /app/roles/permissions/:roleId; /app/employees; /app/profile/basic-info</para>
        /// <para>Angular UI component(s): RolePermissionsStore (app/features/roles/role-permissions/role-permissions.store.ts); RolesList (app/features/roles/roles-list/roles-list.ts); EmployeeManageDialog (app/shared/components/employee/employee-manage-dialog/employee-manage-dialog.ts); RolePermissions (app/features/roles/role-permissions/role-permissions.ts); Employees (app/features/employees/employees.ts); EmployeeBasicInfo (app/features/user-menu/employee-profile/employee-basic-info/employee-basic-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]      
        
        public async Task<IActionResult> GetAllRoles([FromQuery] GetRoleRequestDTO? roleRequestDTO)
        {            

            var query = new GetRoleQuery(roleRequestDTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: deletes role.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): RolesApi.deleteRole (app/core/services/roles-api.ts:132).</para>
        /// <para>Angular purpose: deletes role.</para>
        /// <para>Integrated UI page(s): /app/roles</para>
        /// <para>Angular UI component(s): RolesList (app/features/roles/roles-list/roles-list.ts)</para>
        /// </remarks>
        [HttpDelete("delete")] 
        public async Task<IActionResult> DeleteRole([FromQuery] DeleteRoleRequestDTO deleteRole)
        {         

            var command = new DeleteRoleQuery(deleteRole);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
