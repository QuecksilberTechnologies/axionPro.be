using axionpro.application.DTOS.InsurancePoliciesMapping;
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
        [HttpPost("map")]
        [ProducesResponseType(typeof(ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(
            [FromBody] CreatePolicyTypeInsuranceMappingRequetDTO dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<bool>.Fail("Invalid request."));

            try
            {
                _logger.LogInfo("Create insurance policy started.");

                var command = new CreatePolicyTypeInsuranceMappingCommand(dto);
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (ValidationException vex)
            {
                _logger.LogError($"Validation error: {vex.Message}");
                return BadRequest(ApiResponse<bool>.Fail("Validation failed."));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mapping of insurance and policy type  failed: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.Fail("Internal server error.")
                );
            }
        }

        // 🔹 GET INSURANCE LIST (GRID)
        [HttpGet("get-all")]
         public async Task<IActionResult> GetList( [FromQuery] GetPolicyTypeInsuranceMappingRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching mapped insurance policy list.");

            var query = new GetPolicyInsuranceRequestCommand(requestDto);
            var result = await _mediator.Send(query);         

            return Ok(result);
        }
        // 🔹 GET INSURANCE LIST (GRID)
        [HttpGet("get-details")]
         public async Task<IActionResult> GetDetailList( [FromQuery] GetPolicyTypeInsuranceMapDetailsRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching mapped insurance policy list.");

            var query = new GetPolicyInsuranceDetailRequestCommand(requestDto);
            var result = await _mediator.Send(query);         

            return Ok(result);
        }
        // 🔹 DELETE POLICY INSURANCE MAPPING
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(
            [FromQuery] DeletePolicyTypeInsuranceMappingRequestDTO requestDto)
        {
            _logger.LogInfo("Deleting insurance policy.");

            var command = new DeletePolicyTypeInsuranceQuery(requestDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        // 🔹 UPDATE POLICY INSURANCE MAPPING
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
        //[ProducesResponseType(typeof(ApiResponse<GetInsurancePolicyResponseDTO>), StatusCodes.Status200OK)]
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
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
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
