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
        /// Create Host User.
        /// </summary>
        /// <remarks>
        /// Handles the request to create host user.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="tenantCreateRequestDTO">The request body used to create host user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create-host-user")]
        public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
        {
            _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
            var command = new CreateHostUserCommand(tenantCreateRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Create Host Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to create host role.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="hostRoleRequestDTO">The request body used to create host role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get Host User By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get host user by id.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get All Host Users.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all host users.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="isActive">The query parameters used to get all host users.</param>
        /// <param name="pageNumber">The query parameters used to get all host users.</param>
        /// <param name="pageSize">The query parameters used to get all host users.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Update Host User.
        /// </summary>
        /// <remarks>
        /// Handles the request to update host user.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to update host user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Delete Host User.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete host user.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to delete host user.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Change Host User Password.
        /// </summary>
        /// <remarks>
        /// Handles the request to change host user password.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to change host user password.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Reset Host User Password.
        /// </summary>
        /// <remarks>
        /// Handles the request to reset host user password.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to reset host user password.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get Host Role By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get host role by id.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get All Host Roles.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all host roles.
        /// Requires an authenticated user.
        /// </remarks>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all-host-roles")]
        public async Task<IActionResult> GetAllHostRoles()
        {
            _logger.LogInfo("Received request to get all host roles.");

            var query = new GetAllHostRolesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Update Host Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to update host role.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to update host role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Delete Host Role.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete host role.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="requestDTO">The request body used to delete host role.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get Host Modules.
        /// </summary>
        /// <remarks>
        /// Handles the request to get host modules.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="isActive">The query parameters used to get host modules.</param>
        /// <param name="pageNumber">The query parameters used to get host modules.</param>
        /// <param name="pageSize">The query parameters used to get host modules.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Get Host Module By ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get host module by id.
        /// Requires an authenticated user.
        /// </remarks>
        /// <param name="id">The identifier supplied in the route.</param>
        /// <param name="isActive">The query parameters used to get host module by id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
