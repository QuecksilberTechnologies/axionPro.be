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
        /// Supports the Angular UI flow for create host user.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates host user.</para>
        /// <para>Angular page(s): /app/host-users.</para>
        /// <para>Angular API service call(s): HostApi.createHostUser (app/core/services/host-api.ts:71).</para>
        /// </remarks>
        [HttpPost("create-host-user")]
        public async Task<IActionResult> CreateHostUser([FromBody] CreateHostUserRequestDTO tenantCreateRequestDTO)
        {
            _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
            var command = new CreateHostUserCommand(tenantCreateRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        /// <summary>
        /// Supports the Angular UI flow for create host role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: creates host role.</para>
        /// <para>Angular page(s): /app/host-roles.</para>
        /// <para>Angular API service call(s): HostApi.createHostRole (app/core/services/host-api.ts:101).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get host user by id.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves host user by id.</para>
        /// <para>Angular page(s): /auth/login; /app/admin-dashboard; /app/dashboard; /app/okr/dashboard; /app/okr/:id; /app/okr/my; /app/okr/team; /app/okr/company; and 42 more.</para>
        /// <para>Angular API service call(s): HostApi.getHostUserById (app/core/services/host-api.ts:45).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get all host users.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves host users.</para>
        /// <para>Angular page(s): /app/host-dashboard; /app/host-users.</para>
        /// <para>Angular API service call(s): HostApi.getHostUsers (app/core/services/host-api.ts:37).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for update host user.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates host user.</para>
        /// <para>Angular page(s): /app/host-users; /app/profile; /auth/login; /app/admin-dashboard; /app/dashboard; /app/okr/dashboard; /app/okr/:id; /app/okr/my; and 10 more.</para>
        /// <para>Angular API service call(s): HostApi.updateHostUser (app/core/services/host-api.ts:77).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete host user.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes host user.</para>
        /// <para>Angular page(s): /app/host-users.</para>
        /// <para>Angular API service call(s): HostApi.deleteHostUser (app/core/services/host-api.ts:83).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for change host user password.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates host user password.</para>
        /// <para>Angular page(s): /app/update-password.</para>
        /// <para>Angular API service call(s): HostApi.changeHostUserPassword (app/core/services/host-api.ts:95).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for reset host user password.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: resets host user password.</para>
        /// <para>Angular page(s): /app/host-users.</para>
        /// <para>Angular API service call(s): HostApi.resetHostUserPassword (app/core/services/host-api.ts:89).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get host role by id.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves host role by id.</para>
        /// <para>Angular page(s): /app/host-roles/permissions/:hostRoleId.</para>
        /// <para>Angular API service call(s): HostApi.getHostRoleById (app/core/services/host-api.ts:57).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get all host roles.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves host roles.</para>
        /// <para>Angular page(s): /app/host-dashboard; /app/host-roles; /app/host-users.</para>
        /// <para>Angular API service call(s): HostApi.getHostRoles (app/core/services/host-api.ts:51).</para>
        /// </remarks>
        [HttpGet("get-all-host-roles")]
        public async Task<IActionResult> GetAllHostRoles()
        {
            _logger.LogInfo("Received request to get all host roles.");

            var query = new GetAllHostRolesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Supports the Angular UI flow for update host role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: updates host role.</para>
        /// <para>Angular page(s): /app/host-roles.</para>
        /// <para>Angular API service call(s): HostApi.updateHostRole (app/core/services/host-api.ts:107).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for delete host role.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: deletes host role.</para>
        /// <para>Angular page(s): /app/host-roles.</para>
        /// <para>Angular API service call(s): HostApi.deleteHostRole (app/core/services/host-api.ts:113).</para>
        /// </remarks>
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
        /// Supports the Angular UI flow for get host modules.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves host modules.</para>
        /// <para>Angular page(s): No Angular component caller was statically resolved; the Angular API-service wrapper is documented below..</para>
        /// <para>Angular API service call(s): HostApi.getHostModules (app/core/services/host-api.ts:65).</para>
        /// </remarks>
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
