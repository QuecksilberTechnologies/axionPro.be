

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Leave
{
   
    [Route("api/[controller]")]
    public class EmployeeLeavePolicyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PolicyMappingLeaveTypeController> _logger;

        public EmployeeLeavePolicyController(IMediator mediator, ILogger<PolicyMappingLeaveTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        // ✅ Map EmployeeLeavePolicyMapping 
        [HttpPost("add")]
        public async Task<IActionResult> MapEmployeeyAsync([FromBody] AddLeaveBalanceToEmployeeRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new AddLeaveBalanceCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
        // ✅ Map EmployeeLeavePolicyMapping 
        [HttpPost("LeaveBalance/update")]
        public async Task<IActionResult> UpdateEmployeeyAsync([FromBody] UpdateLeaveBalanceToEmployeeRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateLeaveBalanceCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        // ✅ Map EmployeeLeavePolicyMapping 
        [HttpPost("map")]
        public async Task<IActionResult> MapEmployeeyAsync([FromBody] CreateEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to create EmployeeLeavePolicyMapping: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new EmployeeLeavePolicyMapCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        //  ✅ Get All Employee mapped EmployeeLeavePolicyMapping
        [HttpGet("Mapped/Leave/Policy/get")]
        public async Task<IActionResult> GetAllLeavePoliciesAsync([FromQuery] GetLeaveTypeWithPolicyMappingRequestDTO getLeavePolicyRequestDTO)
        {
            _logger.LogInformation("Fetching all Employee Mapped...");
            var query = new GetAllLeavePolicyQuery(getLeavePolicyRequestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        //  ✅ Get All Employee mapped EmployeeLeavePolicyMapping
        [HttpGet("EmployeeLeavePolicy/Mapped/get")]
        public async Task<IActionResult> GetAllEmployeeLeavePoliciesAsync([FromQuery] GetEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            _logger.LogInformation("Fetching all Employee Mapped...");
            var query = new GetAllEmployeeLeavePolicyQuery(requestDTO);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //// ✅ Update EmployeeLeavePolicyMapping
        [HttpPost("update")]
        public async Task<IActionResult> UpdateLeavePolicyAsync([FromBody] UpdateEmployeeLeavePolicyMappingRequestDTO requestDTO)
        {
            _logger.LogInformation("Received request to update LeavePolicy: {Request}", JsonConvert.SerializeObject(requestDTO));
            var command = new UpdateEmployeeLeavePolicyMapCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
        // ✅ Delete EmployeeLeavePolicyMapping (Soft Delete)
        //[HttpPost("delete")]
        //// [Authorize]
        //public async Task<IActionResult> DeleteLeavePolicy([FromBody] DeletePolicyLeaveTypeMappingRequestDTO request)
        //{
        //    if (request == null)
        //    {
        //        _logger.LogWarning("DeleteLeave request is null.");
        //        return BadRequest(new ApiResponse<bool>
        //        {
        //            IsSucceeded = false,
        //            Message = "Invalid request data.",
        //            Data = false
        //        });
        //    }

        //    _logger.LogInformation("Received request to EmployeeLeavePolicyMapping LeaveType Id: {Id} by UserId: {UserId}", request.Id, request.UserId);

        //    var command = new DeleteLeavePolicyCommand(request);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //    {
        //        _logger.LogWarning("Failed to delete EmployeeLeavePolicyMapping Id: {Id}", request.Id);
        //        return BadRequest(result);
        //    }

        //    _logger.LogInformation("Successfully deleted EmployeeLeavePolicyMapping Id: {Id}", request.Id);
        //    return Ok(result);
        //}

 
    }
}
