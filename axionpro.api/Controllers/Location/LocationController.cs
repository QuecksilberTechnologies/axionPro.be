// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Location operations.
// ================================================================

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

namespace axionpro.api.Controllers.Location
{
    /// <summary>
    /// handled-DDL/Option-related-actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public LocationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }  
        
       
        /// <summary>
        /// get Country.
        /// </summary>
        /// <remarks>
        /// Handles the request to get country.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get country.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("country/option")] 
        public async Task<IActionResult> getCountry([FromQuery] GetCountryOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Country : {requestDTO.UserEmployeeId}");

            var command = new GetCountryQuery(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);        }

        /// <summary>
        /// get State.
        /// </summary>
        /// <remarks>
        /// Handles the request to get state.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get state.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("State/option")]   
        public async Task<IActionResult> getState([FromQuery] GetStateOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get State : {requestDTO.UserEmployeeId}");
            var command = new GetStateQuery(requestDTO);
            var result = await _mediator.Send(command);         
            return Ok(result);
        }
        /// <summary>
        /// get District.
        /// </summary>
        /// <remarks>
        /// Handles the request to get district.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get district.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("District/option")]         
        public async Task<IActionResult> getDistrict([FromQuery] GetDistrictOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get District : {requestDTO.UserEmployeeId}");
            var command = new GetDistrictQuery(requestDTO);
            var result = await _mediator.Send(command);          
            return Ok(result);
        }
    }



}
