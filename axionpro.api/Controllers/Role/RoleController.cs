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
        /// Supports the Angular UI flow for update role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates role.</para>
        /// <para>Angular page(s): /app/roles.</para>
        /// <para>Angular API service call(s): RolesApi.updateRole (app/core/services/roles-api.ts:121).</para>
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
        /// Supports the Angular UI flow for get role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves role options.</para>
        /// <para>Angular page(s): /app/tickets/types; /app/employees; /app/profile/basic-info.</para>
        /// <para>Angular API service call(s): RolesApi.getRoleOptions (app/core/services/roles-api.ts:115).</para>
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
        /// Supports the Angular UI flow for create role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates role.</para>
        /// <para>Angular page(s): /app/roles.</para>
        /// <para>Angular API service call(s): RolesApi.addRole (app/core/services/roles-api.ts:101).</para>
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
        /// Supports the Angular UI flow for get all roles.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves roles.</para>
        /// <para>Angular page(s): /app/roles; /app/host-roles/permissions/:hostRoleId; /app/roles/permissions/:roleId; /app/employees; /app/profile/basic-info.</para>
        /// <para>Angular API service call(s): RolesApi.getRoles (app/core/services/roles-api.ts:108).</para>
        /// </remarks>
        [HttpGet("get")]      
        
        public async Task<IActionResult> GetAllRoles([FromQuery] GetRoleRequestDTO? roleRequestDTO)
        {            

            var query = new GetRoleQuery(roleRequestDTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for delete role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes role.</para>
        /// <para>Angular page(s): /app/roles.</para>
        /// <para>Angular API service call(s): RolesApi.deleteRole (app/core/services/roles-api.ts:128).</para>
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
