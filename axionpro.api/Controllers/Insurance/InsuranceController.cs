using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Features.InsuranceInfo.Handlers;
using axionpro.application.Wrappers;
using axionpro.application.Interfaces.ILogger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Insurance
{
    [Route("api/insurance")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public InsuranceController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // 🔹 CREATE INSURANCE POLICY
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<GetInsurancePolicyResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(
            [FromBody] CreateInsurancePolicyRequestDTO dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<bool>.Fail("Invalid request."));

            try
            {
                _logger.LogInfo("Create insurance policy started.");

                var command = new CreateInsuranceCommand(dto);
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
                _logger.LogError($"Create insurance failed: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.Fail("Internal server error.")
                );
            }
        }

        // 🔹 GET INSURANCE LIST (GRID)
        //[HttpGet("get")]
        //[ProducesResponseType(typeof(ApiResponse<List<GetInsurancePolicyResponseDTO>>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetList()
        //{
        //    try
        //    {
        //        _logger.LogInfo("Fetching insurance policy list.");

        //        var query = new GetInsuranceListQuery();
        //        var result = await _mediator.Send(query);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Get insurance list failed: {ex.Message}");
        //        return StatusCode(
        //            StatusCodes.Status500InternalServerError,
        //            ApiResponse<bool>.Fail("Internal server error.")
        //        );
        //    }
        //}

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
