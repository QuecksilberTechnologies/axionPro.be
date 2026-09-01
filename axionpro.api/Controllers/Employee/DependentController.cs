// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Dependent operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Dependent related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class DependentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DependentController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Validates IMEI number. Must be 15 digits and numeric only.
        /// </summary>


        /// <summary>
        /// Used-In-Angular: creates employee dependent.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: creates dependent.</para>
        /// <para>Handler flow: CreateDependentCommand is processed by CreateDependentCommandHandler; operation(s): CreateAsync, DeleteFileAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDependentResponseDTO: Id (long), EmployeeId (string?), DependentName (string?), Relation (int?), RelationType (string?), DateOfBirth (DateTime?), IsCoveredInPolicy (bool?), IsMarried (bool?), Remark (string?), Description (string?), IsActive (bool?), HasProofUploaded (bool), HasUploadedAll (bool), CompletionPercentage (double), FilePath (string?), InfoVerifiedById (string?), IsInfoVerified (bool?), IsEditAllowed (bool?), InfoVerifiedDateTime (DateTime?)</para>
        /// <para>Angular function(s): EmployeeDependentApi.createEmployeeDependent (app/core/services/employee-dependent-api.ts:72).</para>
        /// <para>Angular purpose: creates employee dependent.</para>
        /// <para>Integrated UI page(s): /app/profile/dependent-info</para>
        /// <para>Angular UI component(s): EmployeeDependentForm (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-form/employee-dependent-form.ts); EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts)</para>
        /// </remarks>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDependentInfo([FromForm] CreateDependentRequestDTO Dto)
        {

                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                }

                _logger.LogInfo("Creating new empolyee Dependent process started.");

                var command = new CreateDependentCommand(Dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-Dependent created successfully.");
                return Ok(result);


        }


        /// <summary>
        /// Used-In-Angular: retrieves employee dependents.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves dependent info.</para>
        /// <para>Handler flow: GetDependentInfoQuery is processed by GetDependentInfoQueryHandler; operation(s): GetInfo.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDependentResponseDTO: Id (long), EmployeeId (string?), DependentName (string?), Relation (int?), RelationType (string?), DateOfBirth (DateTime?), IsCoveredInPolicy (bool?), IsMarried (bool?), Remark (string?), Description (string?), IsActive (bool?), HasProofUploaded (bool), HasUploadedAll (bool), CompletionPercentage (double), FilePath (string?), InfoVerifiedById (string?), IsInfoVerified (bool?), IsEditAllowed (bool?), InfoVerifiedDateTime (DateTime?)</para>
        /// <para>Angular function(s): EmployeeDependentApi.getEmployeeDependents (app/core/services/employee-dependent-api.ts:79).</para>
        /// <para>Angular purpose: retrieves employee dependents.</para>
        /// <para>Integrated UI page(s): /app/profile/dependent-info</para>
        /// <para>Angular UI component(s): EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> Getinfo([FromQuery] GetDependentRequestDTO requestDto)
        {

                _logger.LogInfo("Fetching all  .");

                var command = new GetDependentInfoQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);

        }
        /// <summary>
        /// Used-In-Angular: retrieves employee dependents detail.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves dependent counts.</para>
        /// <para>Handler flow: GetDependentCountsQuery is processed by GetDependentCountsQueryHandler; operation(s): GetDetailInfo.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDependentsDetailResponseDTO: TotalDependents (int), TotalChilds (int), TotalSpouses (int), TotalParents (int), TotalInLaws (int), Dependents (List&lt;GetDependentResponseDTO&gt;)</para>
        /// <para>Angular function(s): EmployeeDependentApi.getEmployeeDependentsDetail (app/core/services/employee-dependent-api.ts:88).</para>
        /// <para>Angular purpose: retrieves employee dependents detail.</para>
        /// <para>Integrated UI page(s): /app/profile/insurance-info</para>
        /// <para>Angular UI component(s): EmployeeInsuranceForm (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-form/employee-insurance-form.ts); EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts)</para>
        /// </remarks>
        [HttpGet("get-in-detail")]
        public async Task<IActionResult> GetInDetail([FromQuery] GetDependentRequestDTO requestDto)
        {

                _logger.LogInfo("Fetching all .");

                var command = new GetDependentCountsQuery(requestDto);
                var result = await _mediator.Send(command);

                return Ok(result);

        }


      /// <summary>
      /// Used-In-Angular: deletes employee dependent.
      /// </summary>
      /// <remarks>
      /// <para>Angular usage status: Used-In-Angular.</para>
      /// <para>API endpoint purpose: deletes contact.</para>
      /// <para>Handler flow: DeleteContactQuery is processed by DeleteContactInfoQueryHandler; operation(s): GetSingleRecordAsync, DeleteAsync.</para>
      /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
      /// <para>Angular function(s): EmployeeDependentApi.deleteEmployeeDependent (app/core/services/employee-dependent-api.ts:103).</para>
      /// <para>Angular purpose: deletes employee dependent.</para>
      /// <para>Integrated UI page(s): /app/profile/dependent-info</para>
      /// <para>Angular UI component(s): EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts)</para>
      /// </remarks>
      [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {

                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteContactQuery(dto);
                var result = await _mediator.Send(command);



                _logger.LogInfo("Dependent deleted successfully.");
                return Ok(result);


        }


        /// <summary>
        /// Used-In-Angular: updates employee dependent.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates dependent.</para>
        /// <para>Handler flow: UpdateDependentCommand is processed by UpdateDependentInfoCommandHandler; operation(s): GetSingleRecordAsync, UpdateAsync, DeleteFileAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeDependentApi.updateEmployeeDependent (app/core/services/employee-dependent-api.ts:96).</para>
        /// <para>Angular purpose: updates employee dependent.</para>
        /// <para>Integrated UI page(s): /app/profile/dependent-info</para>
        /// <para>Angular UI component(s): EmployeeDependentForm (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-form/employee-dependent-form.ts); EmployeeDependentInfo (app/features/user-menu/employee-profile/employee-dependent-info/employee-dependent-info.ts)</para>
        /// </remarks>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateDependentRequestDTO dto)
        {

                _logger.LogInfo($"Updating employee-Dependent record. Id: {dto.Id}");

                var command = new UpdateDependentCommand(dto);
                var result = await _mediator.Send(command);

                _logger.LogInfo("Employee-Dependent updated successfully.");
                return Ok(result);

        }



    }
}
