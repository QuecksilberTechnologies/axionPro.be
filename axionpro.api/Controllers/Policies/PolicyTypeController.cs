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
        /// Used-In-Angular: retrieves policy types.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves policy type.</para>
        /// <para>Handler flow: GetPolicyTypeCommand is processed by GetPolicyTypeCommandHandler; operation(s): GetPolicyTypesAsync, GetByPolicyTypeIdsAsync, GetFileUrl.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetPolicyTypeResponseDTO: Id (int), PolicyName (string), Description (string?), IsActive (bool), IsStructured (bool), PolicyTypeEnumVal (int), EmployeeTypeIds (List&lt;int&gt;), DocDetails (List&lt;GetPolicyTypeDocumentResponseDTO&gt;)</para>
        /// <para>Angular function(s): PolicyTypeApi.getPolicyTypes (app/core/services/policy-type-api.ts:81).</para>
        /// <para>Angular purpose: retrieves policy types.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: retrieves policy type names.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves all policy type.</para>
        /// <para>Handler flow: GetAllPolicyTypeCommand is processed by GetAllPolicyTypeCommandHandler; operation(s): GetAllPolicyTypesAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetAllPolicyTypeResponseDTO: Id (int), PolicyName (string)</para>
        /// <para>Angular function(s): PolicyTypeApi.getPolicyTypeNames (app/core/services/policy-type-api.ts:88).</para>
        /// <para>Angular purpose: retrieves policy type names.</para>
        /// <para>Integrated UI page(s): /app/policies/attendance-policies; /app/policies/attendance-policies/new; /app/policies/attendance-policies/:attendancePolicyId/edit; /app/profile/insurance-info; /app/policies/insurance-policies; /app/policies/insurance-policy-type-mapping</para>
        /// <para>Angular UI component(s): AttendancePolicies (app/features/attendance-policies/attendance-policies.ts); AttendancePolicyForm (app/features/attendance-policies/attendance-policy-form/attendance-policy-form.ts); UpsertInsurancePolicyDialog (app/features/policies/policies-insurance/upsert-insurance-policy-dialog/upsert-insurance-policy-dialog.ts); UpsertPolicyTypeInsuranceMapDialog (app/features/policies/policy-type-insurance-map/upsert-policy-type-insurance-map-dialog/upsert-policy-type-insurance-map-dialog.ts); EmployeeInsuranceForm (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-form/employee-insurance-form.ts); EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts); PoliciesInsurance (app/features/policies/policies-insurance/policies-insurance.ts); PolicyTypeInsuranceMap (app/features/policies/policy-type-insurance-map/policy-type-insurance-map.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: retrieves policy type unstruct details.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves all un structured policy type.</para>
        /// <para>Handler flow: GetAllUnStructuredPolicyTypeCommand is processed by GetAllUnStructuredPolicyTypeCommandHandler; operation(s): GetAllAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetUnStructuredPolicyTypeResponseDTO: Id (long), TenantId (long), EmployeeTypeId (int), EmployeeTypeName (string), PolicyTypeId (int), PolicyTypeName (string), IsActive (bool), StartDate (DateTime)</para>
        /// <para>Angular function(s): PolicyTypeApi.getPolicyTypeUnstructDetails (app/core/services/policy-type-api.ts:109).</para>
        /// <para>Angular purpose: retrieves policy type unstruct details.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): PolicyTypeUnstructDetail (app/features/policies/policy-types/policy-type-unstruct-detail/policy-type-unstruct-detail.ts); PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: creates policy type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: creates policy type.</para>
        /// <para>Handler flow: CreatePolicyTypeCommand is processed by CreatePolicyTypeCommandHandler; operation(s): CreatePolicyTypeAsync, AddRangeAsync, AddAsync, GetFileUrl, DeleteFileAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetPolicyTypeResponseDTO: Id (int), PolicyName (string), Description (string?), IsActive (bool), IsStructured (bool), PolicyTypeEnumVal (int), EmployeeTypeIds (List&lt;int&gt;), DocDetails (List&lt;GetPolicyTypeDocumentResponseDTO&gt;)</para>
        /// <para>Angular function(s): PolicyTypeApi.createPolicyType (app/core/services/policy-type-api.ts:94).</para>
        /// <para>Angular purpose: creates policy type.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): UpsertPolicyTypesDialog (app/features/policies/policy-types/upsert-policy-types-dialog/upsert-policy-types-dialog.ts); PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: updates policy type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates policy type.</para>
        /// <para>Handler flow: UpdatePolicyTypeCommand is processed by UpdatePolicyTypeCommandHandler; operation(s): GetPolicyTypeByIdAsync, UpdatePolicyTypeAsync, GetByEmployeeTypeByPolicyTypeIdAsync, UpdateRangeAsync, AddRangeAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetPolicyTypeResponseDTO: Id (int), PolicyName (string), Description (string?), IsActive (bool), IsStructured (bool), PolicyTypeEnumVal (int), EmployeeTypeIds (List&lt;int&gt;), DocDetails (List&lt;GetPolicyTypeDocumentResponseDTO&gt;)</para>
        /// <para>Angular function(s): PolicyTypeApi.updatePolicyType (app/core/services/policy-type-api.ts:100).</para>
        /// <para>Angular purpose: updates policy type.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): UpsertPolicyTypesDialog (app/features/policies/policy-types/upsert-policy-types-dialog/upsert-policy-types-dialog.ts); PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
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
        /// Used-In-Angular: deletes policy type.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes policy type.</para>
        /// <para>Handler flow: DeletePolicyTypeCommand is processed by DeletePolicyTypeCommandHandler; operation(s): GetPolicyTypeByIdAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): PolicyTypeApi.deletePolicyType (app/core/services/policy-type-api.ts:116).</para>
        /// <para>Angular purpose: deletes policy type.</para>
        /// <para>Integrated UI page(s): /app/policies</para>
        /// <para>Angular UI component(s): PolicyTypes (app/features/policies/policy-types/policy-types.ts)</para>
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePolicyTypeAsync([FromQuery] DeletePolicyTypeDTO requestDTO)
        {
            _logger.LogInfo($"Received request to delete PolicyType: {JsonConvert.SerializeObject(requestDTO)}");
            var command = new DeletePolicyTypeCommand(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: deletes policy type doc.</para>
                /// <para>Handler flow: DeletePolicyTypeDocCommand is processed by DeletePolicyTypeDocCommandHandler; operation(s): GetPolicyTypeOnlyDocByIdAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: DELETE /api/policytype/delete-doc.</para>
                /// </remarks>
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
