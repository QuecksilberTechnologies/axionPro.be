
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.SubscriptionModule;
using axionpro.application.Features.SubscriptionCmd.Commands;
using axionpro.application.Features.SubscriptionCmd.Handlers;
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
        /// <summary>
        /// Creates a new subscription.
        /// </summary>     
        [HttpPost("add")]
      
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequestDTO createSubscriptionDTO)
        {
            _logger.LogInfo("Received request to create a new subscription: " + createSubscriptionDTO.ToString());
            var command = new CreateSubscriptionPlanCommand(createSubscriptionDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateSubscription( long id, [FromBody] UpdateSubscriptionRequestDTO request)
        {
            request.Id = id;

            _logger.LogInfo("Update Subscription request received");

            var result = await _mediator.Send(new UpdateSubscriptionPlanCommand(request));

            return Ok(result);
        }
    }

}
