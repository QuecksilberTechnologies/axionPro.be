using axionpro.application.DTOs.Operation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Location;
using axionpro.application.DTOS.Role;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.DesignationCmd.Handlers;
using axionpro.application.Features.GenderCmd.Handlers;
using axionpro.application.Features.LocationCmd.Handlers;
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.Features.OperationCmd.Queries;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Features.TransportCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Option
{
    /// <summary>
    /// handled-DDL/Option-related-actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public OptionController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all department.
        /// </summary>
        [HttpGet("/Department/get")]

        public async Task<IActionResult> getDepartment([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Department : {requestDTO.UserEmployeeId}");

            var command = new GetDepartmentOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all designation.
        /// </summary>
        [HttpGet("/Designation/get")]

        public async Task<IActionResult> getDesignation([FromQuery] GetDesignationOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Designation : {requestDTO.UserEmployeeId}");

            var command = new GetDesignationOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all designation.
        /// </summary>
        [HttpGet("/Role/get")]

        public async Task<IActionResult> getRole([FromQuery] GetRoleOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Role : {requestDTO.UserEmployeeId}");

            var command = new GetRoleOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Get all designation.
        /// </summary>
        [HttpGet("/Gender/get")]

        public async Task<IActionResult> getGender([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Gender : {requestDTO.UserEmployeeId}");

            var command = new GetGenderOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Get all location.
        /// </summary>
        [HttpGet("/Location/Country")]
        public async Task<IActionResult> getCountry([FromQuery] GetCountryOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Country : {requestDTO.UserEmployeeId}");

            var command = new GetCountryQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        /// Get all Country.
        /// </summary>
        [HttpGet("/Location/Country/State")]
        public async Task<IActionResult> getState([FromQuery] GetStateOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get State : {requestDTO.UserEmployeeId}");

            var command = new GetStateQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        /// Get all District.
        /// </summary>
        [HttpGet("/Location/Country/State/District")]
        public async Task<IActionResult> getDistrict([FromQuery] GetDistrictOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get District : {requestDTO.UserEmployeeId}");

            var command = new GetDistrictQuery(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
    }



}
