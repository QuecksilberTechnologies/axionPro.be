using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Features.PolicyTypeCmd.Handlers;
using axionpro.application.Features.PolicyTypeCmd.Queries; // Query add karni hogi
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // for object logging

namespace axionpro.api.Controllers.Policies
{
    /// <summary>
    /// Handles PolicyType related actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public PolicyTypeController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Get all Policy Types.
        /// </summary>
        /// 
       
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPolicyTypesAsync([FromQuery] GetPolicyTypeRequestDTO requestDTO)
        {
            try
            {
                _logger.LogInfo($"Received request to get PolicyTypes. Params: {JsonConvert.SerializeObject(requestDTO)}");

                // Query use karein, Command nahi
                // var query = new GetAllPolicyTypesQuery(requestDTO);
                var query = new GetPolicyTypeCommand(requestDTO);
                var result = await _mediator.Send(query);

                if (!result.IsSucceeded)
                {
                    return BadRequest(result); // Unauthorized ki jagah
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Error occurred while fetching PolicyTypes. Params: {JsonConvert.SerializeObject(requestDTO)}");

                // ApiResponse wrap kar ke bhejna better hoga
                var errorResponse = new ApiResponse<PolicyTypeResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching policy types.",
                    Data = null
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
              
        
        //[HttpGet("get-by-tenantId")]
        //public async Task<IActionResult> GetAllPolicyTypesByIdAsync([FromQuery] CreatePolicyTypeRequestDTO requestDTO)
        //{
        //    try
        //    {
        //        _logger.LogInfo($"Received request to get PolicyTypes. Params: {JsonConvert.SerializeObject(requestDTO)}");
        //        // Query use karein, Command nahi
        //        // var query = new GetAllPolicyTypesQuery(requestDTO);
        //        var query = new GetPolicyTypeCommand(requestDTO);
        //        var result = await _mediator.Send(query);

        //        if (!result.IsSucceeded)
        //        {
        //            return BadRequest(result); // Unauthorized ki jagah
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInfo($"Error occurred while fetching PolicyTypes. Params: {JsonConvert.SerializeObject(requestDTO)}");

        //        // ApiResponse wrap kar ke bhejna better hoga
        //        var errorResponse = new ApiResponse<PolicyTypeResponseDTO>
        //        {
        //            IsSucceeded = false,
        //            Message = "An unexpected error occurred while fetching policy types.",
        //            Data = null
        //        };

        //        return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        //    }
        //}
        /// <summary>
        /// Create new Policy Type.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePolicyTypeAsync([FromForm] CreatePolicyTypeRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to create PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new CreatePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update new Policy Type.
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> UpdatePolicyTypeAsync([FromBody] UpdatePolicyTypeDTO requestDTO)
        {
            _logger.LogInfo($"Received request to update PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new UpdatePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete   Policy Type.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePolicyTypeAsync([FromBody] DeletePolicyTypeDTO requestDTO)
        {
            _logger.LogInfo($"Received request to delete PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new DeletePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}
