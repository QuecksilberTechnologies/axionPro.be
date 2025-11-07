using axionpro.application.DTOs.Role;
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
        /// Updates an existing role.
        /// </summary>
        /// <param name="updateRoleDTO">DTO containing updated role details.</param>
        /// <returns>Returns the result of the update operation.</returns>
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
        /// Creates a new role.
        /// </summary>
        /// <param name="createRoleDTO">DTO containing details of the role to be created.</param>
        /// <returns>Returns the result of the creation operation.</returns>
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
        /// Retrieves all roles based on provided filters.
        /// </summary>
        /// <param name="roleRequestDTO">Query parameters for filtering roles.</param>
        /// <returns>Returns a list of roles that match the filter criteria.</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAllRoles([FromQuery] GetRoleRequestDTO? roleRequestDTO)
        {
            if (roleRequestDTO == null)
            {
                _logger.LogInfo("Received null request for getting roles.");
                return BadRequest();
            }

            var query = new GetRoleQuery(roleRequestDTO);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="deleteRole">DTO containing the ID of the role to delete.</param>
        /// <returns>Returns the result of the deletion operation.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] DeleteRoleRequestDTO deleteRole)
        {
            if (deleteRole == null)
            {
                _logger.LogInfo("Received null request for deleting a role.");
                return BadRequest();
            }

            var command = new DeleteRoleQuery(deleteRole);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
