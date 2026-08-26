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
        /// Update Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to update role.
        /// </remarks>
        /// <param name="updateRoleDTO">The request body used to update role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// get Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to get role.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("option")]      
        public async Task<IActionResult> getRole([FromQuery] GetRoleOptionRequestDTO requestDTO)
        {
            _logger.LogInfo("Received request to get role options.");

            var command = new GetRoleOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Create Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to create role.
        /// </remarks>
        /// <param name="createRoleDTO">The request body used to create role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get All Roles.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all roles.
        /// </remarks>
        /// <param name="roleRequestDTO">The query parameters used to get all roles.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]      
        
        public async Task<IActionResult> GetAllRoles([FromQuery] GetRoleRequestDTO? roleRequestDTO)
        {            

            var query = new GetRoleQuery(roleRequestDTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Delete Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete role.
        /// </remarks>
        /// <param name="deleteRole">The query parameters used to delete role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")] 
        public async Task<IActionResult> DeleteRole([FromQuery] DeleteRoleRequestDTO deleteRole)
        {         

            var command = new DeleteRoleQuery(deleteRole);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
