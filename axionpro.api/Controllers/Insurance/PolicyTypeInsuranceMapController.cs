// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Policy Type Insurance Map operations.
// ================================================================

using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.InsuranceInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Insurance
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyTypeInsuranceMapController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public PolicyTypeInsuranceMapController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // 🔹 CREATE INSURANCE And POLICY TYPE MAPPING
        /// <summary>
        /// Create.
        /// </summary>
        /// <remarks>
        /// Handles the request to create.
        /// </remarks>
        /// <param name="dto">The request body used to create.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPost("map")]     
        
        public async Task<IActionResult> Create(
            [FromBody] CreatePolicyTypeInsuranceMappingRequetDTO dto)
        {         
           
                _logger.LogInfo("Create insurance policy started.");

                var command = new CreatePolicyTypeInsuranceMappingCommand(dto);
                var result = await _mediator.Send(command);

                return Ok(result);
                     
        }

        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Get List.
        /// </summary>
        /// <remarks>
        /// Handles the request to get list.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get list.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all-map-insurance")]
        
        public async Task<IActionResult> GetList( [FromQuery] GetInsuranceForEmployeeDDLRequestDTO  requestDto)
          {
            _logger.LogInfo("Fetching mapped insurance policy list.");

            var query = new GetAllInsuranceForEmployee(requestDto);
            var result = await _mediator.Send(query);         

            return Ok(result);
        }
        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Get List.
        /// </summary>
        /// <remarks>
        /// Handles the request to get list.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get list.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-all")]
               
        public async Task<IActionResult> GetList( [FromQuery] GetPolicyTypeInsuranceMappingRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching mapped insurance policy list.");

            var query = new GetPolicyInsuranceRequestCommand(requestDto);
            var result = await _mediator.Send(query);         

            return Ok(result);
        }
        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Get Detail List.
        /// </summary>
        /// <remarks>
        /// Handles the request to get detail list.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get detail list.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get-details")]
        
        public async Task<IActionResult> GetDetailList( [FromQuery] GetPolicyTypeInsuranceMapDetailsRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching mapped insurance policy list.");
            var query = new GetPolicyInsuranceDetailRequestCommand(requestDto);
            var result = await _mediator.Send(query);   
            return Ok(result);
        }
        // 🔹 DELETE POLICY INSURANCE MAPPING
        /// <summary>
        /// Delete.
        /// </summary>
        /// <remarks>
        /// Handles the request to delete.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to delete.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpDelete("delete")]        
        public async Task<IActionResult> Delete(
            [FromQuery] DeletePolicyTypeInsuranceMappingRequestDTO requestDto)        {
            _logger.LogInfo("Deleting insurance policy.");
            var command = new DeletePolicyTypeInsuranceQuery(requestDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // 🔹 UPDATE POLICY INSURANCE MAPPING
        /// <summary>
        /// Update.
        /// </summary>
        /// <remarks>
        /// Handles the request to update.
        /// </remarks>
        /// <param name="requestDto">The request body used to update.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpPut("update")]    
        public async Task<IActionResult> Update(
            [FromBody] UpdatePolicyTypeInsuranceMappingRequestDTO requestDto)
        {
            _logger.LogInfo("Updating insurance policy.");
            var command = new UpdatePolicyTypeInsuranceMappingCommand(requestDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        //// 🔹 GET INSURANCE BY ID
        //[HttpGet("get-by-id")]
        //
        //public async Task<IActionResult> GetById([FromQuery] int insurancePolicyId)
        //{
        //    if (insurancePolicyId <= 0)
        //        return BadRequest(ApiResponse<bool>.Fail("Invalid InsurancePolicyId."));

        //    try
        //    {
        //        var query = new GetInsuranceByIdQuery(insurancePolicyId);
        //        var result = await _mediator.Send(query);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Get insurance by id failed: {ex.Message}");
        //        return StatusCode(
        //            StatusCodes.Status500InternalServerError,
        //            ApiResponse<bool>.Fail("Internal server error.")
        //        );
        //    }
        //}

        // 🔹 DELETE (SOFT DELETE)
        //[HttpDelete("delete")]
        //
        //public async Task<IActionResult> Delete([FromQuery] int insurancePolicyId)
        //{
        //    if (insurancePolicyId <= 0)
        //        return BadRequest(ApiResponse<bool>.Fail("Invalid InsurancePolicyId."));

        //    try
        //    {
        //        _logger.LogInfo($"Deleting insurance policy Id: {insurancePolicyId}");

        //        var command = new DeleteInsuranceCommand(insurancePolicyId);
        //        var result = await _mediator.Send(command);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Delete insurance failed: {ex.Message}");
        //        return StatusCode(
        //            StatusCodes.Status500InternalServerError,
        //            ApiResponse<bool>.Fail("Internal server error.")
        //        );
        //    }
        //}
    }
}
