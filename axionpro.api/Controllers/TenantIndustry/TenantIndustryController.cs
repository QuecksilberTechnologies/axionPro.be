// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Tenant Industry operations.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.TenantIndustry;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries;
using axionpro.application.Features.TenantIndustryCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
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
        /// Get All Tenant By Subscription ID.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all tenant by subscription id.
        /// </remarks>
        /// <param name="planId">The query parameters used to get all tenant by subscription id.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-industries")]
        public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] int planId)
        {
            _logger.LogInfo($"Getting email templates for code: {planId}");
            var query = new GetAllTenantIndustryQuery(planId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get Tenant Subscription Plan Info.
        /// </summary>
        /// <remarks>
        /// Handles the request to get tenant subscription plan info.
        /// </remarks>
        /// <param name="code">The query parameters used to get tenant subscription plan info.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-tenant-subscription-plan")]       
        public async Task<IActionResult> GetTenantSubscriptionPlanInfoAsync([FromQuery] TenantSubscriptionPlanRequestDTO code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");
            var query = new GetTenantSubscriptionQuery(code);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
