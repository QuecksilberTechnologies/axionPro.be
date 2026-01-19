using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.Verify;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Features.RegistrationCmd.Commands;
using axionpro.application.Features.RegistrationCmd.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Features.VerifyEmailCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Tenant
{

    /// <summary>
    /// handled-Tenant-related-operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public TenantController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;  // Logger service ko inject karna
        }
    

        [HttpPost("create-tenant")]
        // [Authorize]
        public async Task<IActionResult> TenantCreation([FromBody] application.DTOs.Registration.TenantCreateRequestDTO tenantCreateRequestDTO)
        {
            _logger.LogInfo("Received request for register a new Tenant" + tenantCreateRequestDTO.ToString());
            var command = new CreateTenantCommand(tenantCreateRequestDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }

        [HttpGet("get-all-tenant-by-subscription-plan-Id")]
        public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] application.DTOs.Tenant.TenantRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetAllTenantBySubscriptionPlanIdQuery(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }
        [HttpGet("get-employee-code-pattern")]
        public async Task<IActionResult> GetEmployeeCodePatternAsync(
        [FromQuery] EmployeeCodePatternRequestDTO code)
        {
            _logger.LogInfo($"Fetching employee code pattern for tenant.");

            var query = new GetEmployeeCodePatternQuery(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo("Employee code pattern not found or inactive.");
                return NotFound(result);
            }

            return Ok(result);
        }


        /// <summary>
        /// Get all tenant enabled modules
        /// </summary>
        [HttpPost("get")]
        public async Task<IActionResult> GetAllTenantEnabledModuleOperationsByTenantIdAsync([FromBody] TenantEnabledModuleRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetTenantEnabledModuleCommand(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }

       

        [HttpPost("get-enabled-operations")]
        public async Task<IActionResult> GetAllNodeLeafeWithOperationsAsync([FromBody] TenantEnabledModuleRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetAllTenantTrueEnabledModuleOperationByTenantIdCommand(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }

        [HttpPost("update-modules-and-operations")]
        public async Task<IActionResult> TenantModuleOperationsUpdate([FromBody] TenantModuleOperationsUpdateRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new TenantEnabledModuleOperationsUpdateCommand(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
        {
            try
            {
                var command = new VerifyEmailCommand(request);
                var result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while verifying email.");
                return StatusCode(500, new ApiResponse<VerifyEmailResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Internal server error occurred while verifying email.",
                    Data = null
                });
            }
        }


    }
}
