using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.TenantCmd.Queries;
using axionpro.application.Features.TenantIndustryCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;
 

namespace axionpro.api.Controllers.TenantIndustry
{
    /// <summary>
    /// handled-Tenant-Related-Industry-operations and Its Subscription-Plans.
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class TenantIndustryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public TenantIndustryController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;  // Logger service ko inject karna
        }


        /// <summary>
        /// get all industry.
        /// </summary>
        [HttpGet("get-industries")]
        public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] int planId)
        {
            _logger.LogInfo($"Getting email templates for code: {planId}");

            var query = new GetAllTenantIndustryQuery(planId);
            var result = await _mediator.Send(query);
            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {planId}");
                return NotFound(result); // NotFound better than Unauthorized here
            }

            return Ok(result);
        }

        /// <summary>
        /// get tenant subscription plan detail.
        /// </summary>
        [HttpGet("get-tenant-subscription-plan")]
        public async Task<IActionResult> GetTenantSubscriptionPlanInfoAsync([FromQuery] TenantSubscriptionPlanRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetTenantSubscriptionQuery(code);
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
