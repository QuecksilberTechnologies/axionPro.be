

using axionpro.application.DTOs.Leave;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Leave
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaveController> _logger;  // 🔹 Microsoft ILogger उपयोग करें

        public LeaveController(IMediator mediator, ILogger<LeaveController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateLeaveType([FromBody] CreateLeaveTypeRequestDTO createLeaveTypeDTO)
        {
            if (createLeaveTypeDTO == null)
            {
                _logger.LogWarning("Received null request for creating leave type.");  // ✅ अब सही है
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInformation($"Received request to create a new leave type: {createLeaveTypeDTO.LeaveName}");

            var command = new CreateLeaveTypeCommand(createLeaveTypeDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllLeaves([FromQuery] GetLeaveTypeRequestDTO leaveRequestDTO)
        {
            if (leaveRequestDTO == null)
            {
                _logger.LogWarning("Received null request for getting leaves.");
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInformation("Fetching all leave types...");

            var query = new GetAllLeaveTypeQuery(leaveRequestDTO);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPost("update")]
        // [Authorize]
        public async Task<IActionResult> UpdateLeave([FromBody] UpdateLeaveTypeRequestDTO updateLeaveTypeDTO)
        {
            _logger.LogInformation("Received request for update a leave" + updateLeaveTypeDTO.ToString());
            var command = new UpdateLeaveTypeCommand(updateLeaveTypeDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
        [HttpGet("delete")]
        // [Authorize]
        public async Task<IActionResult> DeleteLeave([FromQuery] DeleteLeaveRequestDTO request)
        {
            if (request == null)
            {
                _logger.LogWarning("DeleteLeave request is null.");
                return BadRequest(new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Invalid request data.",
                    Data = false
                });
            }

            _logger.LogInformation("Received request to delete LeaveType Id: {Id} by UserId: {UserId}", request.Id, request.EmployeeId);

            var command = new DeleteLeaveTypeCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                _logger.LogWarning("Failed to delete LeaveType Id: {Id}", request.Id);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully deleted LeaveType Id: {Id}", request.Id);
            return Ok(result);
        }
    
    

    }
    
}


