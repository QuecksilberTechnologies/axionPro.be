using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
 
using axionpro.application.Features.LeaveRuleCmd.Commands;
using axionpro.application.Features.LeaveRuleCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Leave
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaveRuleController> _logger;

        public LeaveRuleController(IMediator mediator, ILogger<LeaveRuleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // ✅ Create PolicyMappingLeaveType
        [HttpPost("create")]
        public async Task<IActionResult> CreateLeaveRuleAsync([FromBody] CreateLeaveRuleDTORequest requestDTO)
        {
            _logger.LogInformation("Received request to create PolicyMappingLeaveType: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new CreateLeaveRuleCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        //  ✅ Get All LeavePolicies
        [HttpGet("get")]
        public async Task<IActionResult> GetAllLeaveRuleAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        //  ✅ Get All leave rule sandwich 
        [HttpGet("LeaveRule/Sandwich/get")]
        public async Task<IActionResult> GetAllLeaveRuleSandwichAsync([FromQuery] GetLeaveRuleRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Leave rule...");
            var query = new GetAllLeaveRuleQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //// ✅ Update LeavePolicy
        [HttpPost("update")]
        public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateLeaveRuleRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to update LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateLeaveRuleCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("delete")]
        // [Authorize]
        public async Task<IActionResult> DeleteLeavePolicy([FromBody] DeleteLeaveRuleDTO request)
        {
            if (request == null)
            {
                _logger.LogWarning("Delete Leave Rule request is null.");
                return BadRequest(new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Invalid request data.",
                    Data = false
                });
            }

            _logger.LogInformation("Received request to delete Leave Rule Type Id: {Id} by UserId: {UserId}", request.Id, request.UserId);

            var command = new DeleteLeaveRuleCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                _logger.LogWarning("Failed to delete Leave Rule Id: {Id}", request.Id);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully deleted Leave Rule Id: {Id}", request.Id);
            return Ok(result);
        }
    }
}
