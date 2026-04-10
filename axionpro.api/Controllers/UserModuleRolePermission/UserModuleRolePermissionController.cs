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
    /// Get all tenant enabled modules with operations
    /// </summary>
    [HttpPost("get-role-based-permissions")]
    public async Task<IActionResult> GetTenantEnabledOperations([FromBody] GetAllActiveRoleModuleOperationsRequestByRoleIdDTO code)
    {
        _logger.LogInfo($"Getting email templates for code: {code}");

        var query = new GetRolePermissionCommand(code);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
 