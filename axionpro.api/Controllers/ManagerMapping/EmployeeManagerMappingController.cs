using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeManagerMappings;
using axionpro.application.DTOS.EmployeeType;
using axionpro.application.Features.EmployeeManagerMapping.Command;
using axionpro.application.Features.EmployeeTypeCmd.Handlers;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmployeeType
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeManagerMappingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeManagerMappingController> _logger;  // 🔹 Microsoft ILogger उपयोग करें

        public EmployeeManagerMappingController(IMediator mediator, ILogger<EmployeeManagerMappingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        //[HttpPost("get")]
        //public async Task<IActionResult> CreateLeaveType([FromBody] CreateLeaveTypeDTO createLeaveTypeDTO)
        //{
        //    if (createLeaveTypeDTO == null)
        //    {
        //        _logger.LogWarning("Received null request for creating leave type.");  // ✅ अब सही है
        //        return BadRequest(new { success = false, message = "Invalid request" });
        //    }

        //    _logger.LogInformation($"Received request to create a new leave type: {createLeaveTypeDTO.LeaveName}");

        //    var command = new CreateLeaveTypeCommand(createLeaveTypeDTO);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSucceeded)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}
        /// <summary>
        /// Returns list of all Employee Types.
        /// </summary>


        [HttpPost("add")]
        // [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] AddEmployeeManagerMappingDTO DTO)
        {
            _logger.LogInformation("Received request to mapping manager: " + DTO.ToString());
            var command = new AddEmployeeManagerMappingCommand(DTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }



    }

}
