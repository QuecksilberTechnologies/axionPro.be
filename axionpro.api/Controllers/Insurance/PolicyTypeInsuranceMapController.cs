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
        /// Used-In-Angular: assigns or maps policy type insurance.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PolicyTypeInsuranceMapApi.mapPolicyTypeInsurance (app/core/services/policy-type-insurance-map-api.ts:88).</para>
        /// <para>Angular purpose: assigns or maps policy type insurance.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): UpsertPolicyTypeInsuranceMapDialog (app/features/policies/policy-type-insurance-map/upsert-policy-type-insurance-map-dialog/upsert-policy-type-insurance-map-dialog.ts); PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
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
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: GET /api/policytypeinsurancemap/get-all-map-insurance.</para>
        //         /// </remarks>
        //         [HttpGet("get-all-map-insurance")]
        //
        //         public async Task<IActionResult> GetList( [FromQuery] GetInsuranceForEmployeeDDLRequestDTO  requestDto)
        //           {
        //             _logger.LogInfo("Fetching mapped insurance policy list.");
        //
        //             var query = new GetAllInsuranceForEmployee(requestDto);
        //             var result = await _mediator.Send(query);
        //
        //             return Ok(result);
        //         }
        #endregion
        // 🔹 GET INSURANCE LIST (GRID)
        /// <summary>
        /// Used-In-Angular: retrieves policy type insurance maps.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PolicyTypeInsuranceMapApi.getPolicyTypeInsuranceMaps (app/core/services/policy-type-insurance-map-api.ts:63).</para>
        /// <para>Angular purpose: retrieves policy type insurance maps.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: retrieves policy type insurance map details.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PolicyTypeInsuranceMapApi.getPolicyTypeInsuranceMapDetails (app/core/services/policy-type-insurance-map-api.ts:73).</para>
        /// <para>Angular purpose: retrieves policy type insurance map details.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): PolicyTypeMapDetail (app/features/policies/policy-types/policy-type-map-detail/policy-type-map-detail.ts); PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: deletes policy type insurance map.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PolicyTypeInsuranceMapApi.deletePolicyTypeInsuranceMap (app/core/services/policy-type-insurance-map-api.ts:107).</para>
        /// <para>Angular purpose: deletes policy type insurance map.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: updates policy type insurance map.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): PolicyTypeInsuranceMapApi.updatePolicyTypeInsuranceMap (app/core/services/policy-type-insurance-map-api.ts:96).</para>
        /// <para>Angular purpose: updates policy type insurance map.</para>
        /// <para>Integrated UI page(s): /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): UpsertPolicyTypeInsuranceMapDialog (app/features/policies/policy-type-insurance-map/upsert-policy-type-insurance-map-dialog/upsert-policy-type-insurance-map-dialog.ts); PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
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
