// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Leave operations.
// ================================================================



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
        /// <summary>
        /// Create Leave Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to create leave type.
        /// </remarks>
        /// <param name="createLeaveTypeDTO">The request body used to create leave type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

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

            return Ok(result);
        }
        /// <summary>
        /// Get All Leaves.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all leaves.
        /// </remarks>
        /// <param name="leaveRequestDTO">The query parameters used to get all leaves.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]      
        public async Task<IActionResult> GetAllLeaves([FromQuery] GetLeaveTypeRequestDTO leaveRequestDTO)
        {
            var query = new GetAllLeaveTypeQuery(leaveRequestDTO);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        /// <summary>
        /// Update Leave.
        /// </summary>
        /// <remarks>
        /// Handles the request to update leave.
        /// </remarks>
        /// <param name="updateLeaveTypeDTO">The request body used to update leave.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]
        
        
        
        // [Authorize]
        public async Task<IActionResult> UpdateLeave([FromBody] UpdateLeaveTypeRequestDTO updateLeaveTypeDTO)
        {
            _logger.LogInformation("Received request for update a leave" + updateLeaveTypeDTO.ToString());
            var command = new UpdateLeaveTypeCommand(updateLeaveTypeDTO);
            var result = await _mediator.Send(command);           
            return Ok(result);
        }
        /// <summary>
        /// Delete Leave.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete leave.
        /// </remarks>
        /// <param name="request">The query parameters used to delete leave.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("delete")]      
        // [Authorize]
        public async Task<IActionResult> DeleteLeave([FromQuery] DeleteLeaveRequestDTO request)
        {                       

            _logger.LogInformation("Received request to delete LeaveType Id: {Id} by UserId: {UserId}", request.Id, request.EmployeeId);
            var command = new DeleteLeaveTypeCommand(request);
            var result = await _mediator.Send(command);
            _logger.LogInformation("Successfully deleted LeaveType Id: {Id}", request.Id);
            return Ok(result);
        }
    
    

    }
    
}


