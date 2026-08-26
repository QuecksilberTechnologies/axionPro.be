// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Policy Type operations.
// ================================================================

using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.PolicyType;
using axionpro.application.Features.PolicyTypeCmd.Handlers;
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
        /// Get All Policy Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all policy types.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get all policy types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all")]    
        public async Task<IActionResult> GetAllPolicyTypesAsync([FromQuery] GetPolicyTypeRequestDTO requestDTO)
        {
                _logger.LogInfo($"Received request to get PolicyTypes. Params: {JsonConvert.SerializeObject(requestDTO)}");

                // Query use karein, Command nahi
                // var query = new GetAllPolicyTypesQuery(requestDTO);
                var query = new GetPolicyTypeCommand(requestDTO);
                var result = await _mediator.Send(query);
            return Ok(result);         
            
        }
        /// <summary>
        /// Get DDL Policy Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get ddl policy types.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get ddl policy types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-ddl")]     
        public async Task<IActionResult> GetDDLPolicyTypesAsync(
             [FromQuery] GetAllPolicyTypeRequestDTO requestDTO)
       
            {
                _logger.LogInfo(
                    "Received request to get PolicyType DDL. Params: {Params}" );

                // --------------------------------------------------
                // 🔹 MediatR Command (returns List<GetPolicyTypeResponseDTO>)
                // --------------------------------------------------
                var query = new GetAllPolicyTypeCommand(requestDTO);
                var result = await _mediator.Send(query);

                // --------------------------------------------------
                // 🔹 Safety: null / empty list
                // --------------------------------------------------
                
                return Ok(result);
          
           
        }
        /// <summary>
        /// Get Unstructured Policy Types.
        /// </summary>
        /// <remarks>
        /// Handles the request to get unstructured policy types.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get unstructured policy types.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all-unstruct")]     
        public async Task<IActionResult> GetUnstructuredPolicyTypesAsync(
             [FromQuery] GetAllUnStructuredPolicyTypeRequestDTO requestDTO)
       
            {
                _logger.LogInfo(
                    "Received request to get mapped PolicyType . Params: {Params}" );

                // --------------------------------------------------
                // 🔹 MediatR Command (returns List<GetPolicyTypeResponseDTO>)
                // --------------------------------------------------
                var query = new GetAllUnStructuredPolicyTypeCommand(requestDTO);
                var result = await _mediator.Send(query);

                // --------------------------------------------------
                // 🔹 Safety: null / empty list
                // --------------------------------------------------
                
                return Ok(result);
          
           
        }
 

        /// <summary>
        /// Create Policy Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to create policy type.
        /// </remarks>
        /// <param name="requestDTO">The form data used to create policy type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePolicyTypeAsync([FromForm] CreatePolicyTypeRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to create PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new CreatePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Update Policy Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to update policy type.
        /// </remarks>
        /// <param name="requestDTO">The form data used to update policy type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePolicyTypeAsync([FromForm] UpdatePolicyTypeRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to update PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new UpdatePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Delete Policy Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete policy type.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to delete policy type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]        
        public async Task<IActionResult> DeletePolicyTypeAsync([FromQuery] DeletePolicyTypeDTO requestDTO)
        {
            _logger.LogInfo($"Received request to delete PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new DeletePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Delete Policy Type Doc Only.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete policy type doc only.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to delete policy type doc only.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete-doc")]
        public async Task<IActionResult> DeletePolicyTypeDocOnlyAsync([FromQuery] DeleteRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to delete PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new DeletePolicyTypeDocCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
