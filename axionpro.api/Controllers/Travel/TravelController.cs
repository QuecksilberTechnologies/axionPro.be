// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Travel operations.
// ================================================================

using axionpro.application.DTOs.Transport;
using axionpro.application.Features.ClientCmd.Commands;
using axionpro.application.Features.ClientCmd.Queries;
using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Features.TransportCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Travel
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public TravelController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Get All Travel Mode Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all travel mode type.
        /// </remarks>
        /// <param name="travelModeRequestDTO">The query parameters used to get all travel mode type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>


        [HttpGet("getalltravelmodetype")]       
        public async Task<IActionResult> GetAllTravelModeType([FromQuery] TravelModeRequestDTO travelModeRequestDTO)
        {
            _logger.LogInfo($"Received request to get clientRequestType from userId: {travelModeRequestDTO.Id}");

            var command = new GetAllTravelModeTypeQuery(travelModeRequestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Create Travel Mode Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to create travel mode type.
        /// </remarks>
        /// <param name="createTravelModeDTO">The request body used to create travel mode type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("addtravelmode")] 
        public async Task<IActionResult> CreateTravelModeType([FromBody] CreateTravelModeDTO createTravelModeDTO)
        {   

            _logger.LogInfo($"Received request to create a new leave type: {createTravelModeDTO.TravelModeName}");
            var command = new CreateTravelModeTypeCommand(createTravelModeDTO);
            var result = await _mediator.Send(command);
            return Ok(result);        }
        /// <summary>
        /// Update Travel Mode Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to update travel mode type.
        /// </remarks>
        /// <param name="updateTravelModeDTO">The request body used to update travel mode type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>

        [HttpPost("updatetravelmodetype")]        
        public async Task<IActionResult> UpdateTravelModeType([FromBody] UpdateTravelModeDTO updateTravelModeDTO)
        {
            _logger.LogInfo("Received request for update a leave" + updateTravelModeDTO.ToString());
            var command = new UpdateTravelModeTypeCommand(updateTravelModeDTO);
            var result = await _mediator.Send(command);            
            return Ok(result);
        }
        //  [HttpPost("getalltendermaincategory")]
        //public async Task<IActionResult> GetAllTenderMainCategories([FromBody] TenderCategoryRequestDTO? tenderCategoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get categories from userId: {LoginId}" + tenderCategoryRequestDTO.Id.ToString());
        //    var command = new GetTenderMainCategoryRequestCommand(tenderCategoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


        //[HttpPost("getallmainchildcategory")]
        //public async Task<IActionResult> GetAllMainChildCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get sub-categories from userId: {LoginId}" + categoryRequestDTO.Id.ToString());
        //    var command = new GetMainChildCategoryCommand(categoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


    }

}
