// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Insurance operations.
// ================================================================

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
        /// <summary>
        /// Used-In-Angular: creates insurance policies.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.createInsurancePolicies (app/core/services/policies-insurance-api.ts:69).</para>
        /// <para>Angular purpose: creates insurance policies.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policies</para>
        /// <para>Angular UI component(s): UpsertInsurancePolicyDialog (app/features/policies/policies-insurance/upsert-insurance-policy-dialog/upsert-insurance-policy-dialog.ts); PoliciesInsurance (app/features/policies/policies-insurance/policies-insurance.ts)</para>
        /// </remarks>
        [HttpPost("create")]  
        public async Task<IActionResult> Create(
            [FromBody] CreateInsurancePolicyRequestDTO dto)
        {
                _logger.LogInfo("Create insurance policy started.");

                var command = new CreateInsuranceCommand(dto);
                var result = await _mediator.Send(command);

                return Ok(result);
           
            
        }

        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Used-In-Angular: retrieves insurance policy names.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.getInsurancePolicyNames (app/core/services/policies-insurance-api.ts:83).</para>
        /// <para>Angular purpose: retrieves insurance policy names.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): UpsertPolicyTypeInsuranceMapDialog (app/features/policies/policy-type-insurance-map/upsert-policy-type-insurance-map-dialog/upsert-policy-type-insurance-map-dialog.ts); PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
        [HttpGet("get-ddl")]     
        
        public async Task<IActionResult> GetList(
            [FromQuery] GetAllInsurancePolicyRequestDTO requestDto)
        {
            _logger.LogInfo("Fetching insurance policy list.");

            var query = new GetAllInsuranceQuery(requestDto);
            var result = await _mediator.Send(query);


            return Ok(result);
        }

        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Used-In-Angular: retrieves insurance details.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.getInsuranceDetails (app/core/services/policies-insurance-api.ts:103).</para>
        /// <para>Angular purpose: retrieves insurance details.</para>
        /// <para>Integrated UI page(s): /app/profile/insurance-info</para>
        /// <para>Angular UI component(s): EmployeeInsuranceForm (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-form/employee-insurance-form.ts); EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts)</para>
        /// </remarks>
        [HttpGet("get-detail-ddl")]     
        
        public async Task<IActionResult> GetDetailList(
            [FromQuery] GetAllInsurancePolicyRequestWithEmployeeIdDTO requestDto)
        {
            _logger.LogInfo("Fetching insurance policy list.");

            var query = new GetConsumedInsuranceListQuery(requestDto);
            var result = await _mediator.Send(query);


            return Ok(result);
        }

        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Used-In-Angular: retrieves insurance policies.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.getInsurancePolicies (app/core/services/policies-insurance-api.ts:76).</para>
        /// <para>Angular purpose: retrieves insurance policies.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policies</para>
        /// <para>Angular UI component(s): PoliciesInsurance (app/features/policies/policies-insurance/policies-insurance.ts)</para>
        /// </remarks>
        [HttpGet("get-all")]        
        public async Task<IActionResult> GetList(
            [FromQuery] GetInsurancePolicyRequestDTO requestDto)
          {
            _logger.LogInfo("Fetching insurance policy list.");
            var query = new GetInsuranceQuery(requestDto);
            var result = await _mediator.Send(query);

            return Ok(result);
           }

        // 🔹 DELETE INSURANCE POLICY
        /// <summary>
        /// Used-In-Angular: deletes insurance policy.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.deleteInsurancePolicy (app/core/services/policies-insurance-api.ts:96).</para>
        /// <para>Angular purpose: deletes insurance policy.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policies</para>
        /// <para>Angular UI component(s): PoliciesInsurance (app/features/policies/policies-insurance/policies-insurance.ts)</para>
        /// </remarks>
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
        /// <summary>
        /// Used-In-Angular: updates insurance policies.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PoliciesInsuranceApi.updateInsurancePolicies (app/core/services/policies-insurance-api.ts:89).</para>
        /// <para>Angular purpose: updates insurance policies.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policies</para>
        /// <para>Angular UI component(s): UpsertInsurancePolicyDialog (app/features/policies/policies-insurance/upsert-insurance-policy-dialog/upsert-insurance-policy-dialog.ts); PoliciesInsurance (app/features/policies/policies-insurance/policies-insurance.ts)</para>
        /// </remarks>
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
