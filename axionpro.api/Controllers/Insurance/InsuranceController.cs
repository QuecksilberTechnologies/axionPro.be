using axionpro.application.DTOS.InsurancePolicy;
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
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetList(
            [FromQuery] GetAllInsurancePolicyRequestDTO requestDto)
        {
            _logger.LogInfo("Fetching insurance policy list.");

            var query = new GetAllInsuranceQuery(requestDto);
            var result = await _mediator.Send(query);

            // ❌ No InternalServerError
            // ❌ No try-catch drama
            // ✅ ApiResponse decides success/fail

            return Ok(result);
        }

        // 🔹 GET INSURANCE LIST (GRID)
        [HttpGet("get-all")]
         public async Task<IActionResult> GetList(
            [FromQuery] GetInsurancePolicyRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching insurance policy list.");

            var query = new GetInsuranceQuery(requestDto);
            var result = await _mediator.Send(query);

            // ❌ No InternalServerError
            // ❌ No try-catch drama
            // ✅ ApiResponse decides success/fail

            return Ok(result);
        }
        // 🔹 DELETE INSURANCE POLICY
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(
            [FromQuery] DeleteInsurancePolicyRequestDTO requestDto)
        {
            _logger.LogInfo("Deleting insurance policy.");

            var command = new DeleteInsurancePolicyQuery(requestDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        // 🔹 UPDATE INSURANCE POLICY
        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateInsurancePolicyRequestDTO requestDto)
        {
            _logger.LogInfo("Updating insurance policy.");

            var command = new UpdateInsurancePolicyCommand(requestDto);
            var result = await _mediator.Send(command);

            // ❌ No InternalServerError
            // ❌ No try-catch drama
            // ✅ ApiResponse decides success/fail

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
