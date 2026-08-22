// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Host administration endpoints for Host-user, Host-role, and Host-module management.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostCmd.Handler;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Host
{
    /// <summary>
    /// Provides authenticated HTTP endpoints for Host administration requests.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    /// <summary>
    /// Initializes a new instance of the <see cref="HostController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to dispatch Host requests.</param>
    /// <param name="logger">The logger used to record controller activity.</param>
    public HostController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;  // Logger service ko inject karna
    }

        /// <summary>
        /// Creates a Host user.
        /// </summary>
        /// <param name="tenantCreateRequestDTO">The Host-user creation request.</param>
        /// <returns>The created Host-user response.</returns>
        [HttpPost("create-host-user")]
        public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
        {
            _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
            var command = new CreateHostUserCommand(tenantCreateRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Creates a Host role.
        /// </summary>
        /// <param name="hostRoleRequestDTO">The Host-role creation request.</param>
        /// <returns>The created Host-role response.</returns>
        [HttpPost("create-host-role")]
        public async Task<IActionResult> CreateHostRole( [FromBody] CreateHostRoleRequestDTO hostRoleRequestDTO)
        {
            _logger.LogInfo(
                "Received request to create a new Host Role.");

            var command = new CreateHostRoleCommand(hostRoleRequestDTO);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #region Host User CRUD

        /// <summary>
        /// Retrieves a host user by identifier.
        /// </summary>
        /// <param name="id">The host-user identifier.</param>
        /// <returns>An HTTP response containing the requested host user.</returns>
        [HttpGet("get-host-user-by-id/{id}")]
        public async Task<IActionResult> GetHostUserById(long id)
        {
            _logger.LogInfo(
                $"Received request to get host user by Id: {id}");

            var query = new GetHostUserByIdQuery(id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves filtered and database-paged host users.
        /// </summary>
        /// <param name="isActive">When supplied, filters users by active status.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The requested number of rows per page.</param>
        /// <returns>An HTTP response containing the requested Host-user page.</returns>
        [HttpGet("get-all-host-users")]
        public async Task<IActionResult> GetAllHostUsers(
            [FromQuery] bool? isActive = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInfo($"Received Host-user list request. IsActive: {isActive}; PageNumber: {pageNumber}; PageSize: {pageSize}.");

            var query = new GetAllHostUsersQuery(isActive, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Updates editable details for a host user.
        /// </summary>
        /// <param name="requestDTO">The host-user details to update.</param>
        /// <returns>An HTTP response containing the updated host user.</returns>
        [HttpPost("update-host-user")]
        public async Task<IActionResult> UpdateHostUser(
            [FromBody] UpdateHostUserRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to update host user. Id: {requestDTO?.Id}");

            var command = new UpdateHostUserCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Soft deletes a host user.
        /// </summary>
        /// <param name="requestDTO">The host-user identifier to delete.</param>
        /// <returns>An HTTP response indicating whether the host user was soft deleted.</returns>
        [HttpPost("delete-host-user")]
        public async Task<IActionResult> DeleteHostUser(
            [FromBody] DeleteHostUserRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to delete host user. Id: {requestDTO?.Id}");

            var command = new DeleteHostUserCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #endregion

        #region Host User Password Management

        /// <summary>
        /// Changes a host user's password after verifying the old password.
        /// </summary>
        /// <param name="requestDTO">The password-change details.</param>
        /// <returns>An HTTP response indicating whether the password was changed.</returns>
        [HttpPost("change-host-user-password")]
        public async Task<IActionResult> ChangeHostUserPassword(
            [FromBody] ChangeHostUserPasswordRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to change HostUser password. HostUserId: {requestDTO?.HostUserId}");

            var command = new ChangeHostUserPasswordCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Resets a host user's password without requiring the old password.
        /// </summary>
        /// <param name="requestDTO">The password-reset details.</param>
        /// <returns>An HTTP response indicating whether the password was reset.</returns>
        [HttpPost("reset-host-user-password")]
        public async Task<IActionResult> ResetHostUserPassword(
            [FromBody] ResetHostUserPasswordRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to reset HostUser password. HostUserId: {requestDTO?.HostUserId}");

            var command = new ResetHostUserPasswordCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #endregion

        #region Host Role CRUD

        /// <summary>
        /// Retrieves a host role by identifier.
        /// </summary>
        /// <param name="id">The host-role identifier.</param>
        /// <returns>An HTTP response containing the requested host role.</returns>
        [HttpGet("get-host-role-by-id/{id}")]
        public async Task<IActionResult> GetHostRoleById(long id)
        {
            _logger.LogInfo(
                $"Received request to get host role by Id: {id}");

            var query = new GetHostRoleByIdQuery(id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all host roles.
        /// </summary>
        /// <returns>An HTTP response containing all host roles.</returns>
        [HttpGet("get-all-host-roles")]
        public async Task<IActionResult> GetAllHostRoles()
        {
            _logger.LogInfo("Received request to get all host roles.");

            var query = new GetAllHostRolesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Updates editable details for a host role.
        /// </summary>
        /// <param name="requestDTO">The host-role details to update.</param>
        /// <returns>An HTTP response containing the updated host role.</returns>
        [HttpPost("update-host-role")]
        public async Task<IActionResult> UpdateHostRole(
            [FromBody] UpdateHostRoleRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to update host role. Id: {requestDTO?.Id}");

            var command = new UpdateHostRoleCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Soft deletes a host role.
        /// </summary>
        /// <param name="requestDTO">The host-role identifier to delete.</param>
        /// <returns>An HTTP response indicating whether the host role was soft deleted.</returns>
        [HttpPost("delete-host-role")]
        public async Task<IActionResult> DeleteHostRole(
            [FromBody] DeleteHostRoleRequestDTO requestDTO)
        {
            _logger.LogInfo(
                $"Received request to delete host role. Id: {requestDTO?.Id}");

            var command = new DeleteHostRoleCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        #endregion
         
        #region Host Module Queries

        /// <summary>
        /// Retrieves modules that belong to the Host application scope.
        /// </summary>
        /// <param name="isActive">When supplied, filters Host modules by their active state.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The requested number of rows per page.</param>
        /// <returns>An HTTP response containing the requested Host-module page.</returns>
        [HttpGet("get-host-modules")]
        public async Task<IActionResult> GetHostModules(
            [FromQuery] bool? isActive = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInfo($"Received Host-module list request. IsActive: {isActive}; PageNumber: {pageNumber}; PageSize: {pageSize}.");

            var query = new GetHostModulesQuery(isActive, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves one module from the Host application scope by identifier.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        /// <param name="isActive">When supplied, filters the Host module by its active state.</param>
        /// <returns>An HTTP response containing the requested Host module.</returns>
        [HttpGet("get-host-module-by-id/{id:int}")]
        public async Task<IActionResult> GetHostModuleById(
            int id,
            [FromQuery] bool? isActive = null)
        {
            _logger.LogInfo($"Received request to get Host module. Id: {id}, IsActive: {isActive}");

            var query = new GetHostModuleByIdQuery(id, isActive);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        #endregion

    }
}
