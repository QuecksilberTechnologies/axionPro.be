using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Commands;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.ModulePermission
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionModuleOperationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public RolePermissionModuleOperationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create")]
        // [Authorize]
        public async Task<IActionResult> CreatePermission([FromBody] CreateModuleOperationRolePermissionsRequestDTO insertRoleModulePermissionsRequestDTO)
        {
            _logger.LogInfo("Received request for update a new role" + insertRoleModulePermissionsRequestDTO.ToString());
            var command = new  CreateModuleOperationRolePermissionCommand(insertRoleModulePermissionsRequestDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Get all tenant enabled modules with operations
        /// </summary>
        [HttpPost("get-tenant-mapped-operations")]
        public async Task<IActionResult> GetTenantEnabledOperations([FromBody] GetTenantModuleOperationRolePermissionsRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetModuleOperationMappingRepositoryCommand(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }
    }
}
