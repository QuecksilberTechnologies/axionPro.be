
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
 
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Subscription
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public SubscriptionController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #region  ss

        [HttpPost("get-all-subscription-plan")]
        public async Task<IActionResult> GetAllSubscriptionPlan([FromBody] SubscriptionPlanRequestDTO? subscriptionPlanRequestDTO)
        {
            
            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new GetSubscriptionPlanCommand(subscriptionPlanRequestDTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);


            return Ok(result);
        }

        #endregion
        [HttpPost("get-tenant-subscription-plan-info")]
        public async Task<IActionResult> GetTenantSubscriptionPlanInfo([FromBody] TenantSubscriptionPlanRequestDTO subscriptionPlanRequestDTO)
        {
           
            var query = new GetValidateTenantIdCommand(subscriptionPlanRequestDTO);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("get-all-tenant-accessible-modules")]
        public async Task<IActionResult> GetAllPlanModulePapping([FromBody] PlanModuleMappingRequestDTO? planModuleMappingRequest)
        {
            // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

            var query = new GetPlanModuleMappingCommand(planModuleMappingRequest);  //  Fix: No parameter needed in GetAllAssetQuery
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }

}
