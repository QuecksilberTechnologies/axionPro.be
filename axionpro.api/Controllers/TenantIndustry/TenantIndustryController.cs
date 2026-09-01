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
        /// Used-In-Angular: retrieves industries.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves all tenant industry.</para>
        /// <para>Handler flow: GetAllTenantIndustryQuery is processed by GetAllTenantIndustryQueryHandler; operation(s): GetAllActiveIndustriesAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantIndustryResponseDTO: Id (int), IndustryName (string), Description (string?), Remark (string?), IsActive (bool)</para>
        /// <para>Angular function(s): IndustriesApi.getIndustries (app/core/services/industries-api.ts:45).</para>
        /// <para>Angular purpose: retrieves industries.</para>
        /// <para>Integrated UI page(s): /auth/register-tenant; /app/tenants/new; /app/tenants/:tenantId/edit</para>
        /// <para>Angular UI component(s): Registration (app/features/authentication/registration/registration.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
        /// </remarks>
        [HttpGet("get-industries")]
        public async Task<IActionResult> GetAllTenantBySubscriptionIdAsync([FromQuery] int planId)
        {
            _logger.LogInfo($"Getting email templates for code: {planId}");
            var query = new GetAllTenantIndustryQuery(planId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: retrieves tenant subscription plans.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves tenant subscription.</para>
        /// <para>Handler flow: GetTenantSubscriptionQuery is processed by GetTenantSubscriptionQueryHandler; operation(s): GetTenantSubscriptionPlanInfoAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantSubscriptionPlanResponseDTO: Id (long), TenantId (long?), SubscriptionPlanId (int), SubscriptionStartDate (DateTime), SubscriptionEndDate (DateTime?), IsActive (bool), PaymentTxnId (string?), PaymentMode (string?), IsTrial (bool)</para>
        /// <para>Angular function(s): IndustriesApi.getTenantSubscriptionPlans (app/core/services/industries-api.ts:59).</para>
        /// <para>Angular purpose: retrieves tenant subscription plans.</para>
        /// <para>Integrated UI page(s): /app/subscriptions</para>
        /// <para>Angular UI component(s): SubscriptionPlanDetail (app/features/host/subscriptions/subscription-plan-detail/subscription-plan-detail.ts); SubscriptionsStore (app/features/host/subscriptions/subscriptions.store.ts); Subscriptions (app/features/host/subscriptions/subscriptions.ts)</para>
        /// </remarks>
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
