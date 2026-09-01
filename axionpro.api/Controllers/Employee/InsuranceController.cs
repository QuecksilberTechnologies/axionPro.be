// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Insurance operations.
// ================================================================



using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.EnrolledPolicy;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.EmployeeCmd.InsuranceInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee;

/// <summary>
/// handled-Employee-related-operations.
/// </summary>
[Route("api/Employee/[controller]")]
[ApiController]
public class InsuranceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration
    public InsuranceController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;  // Logger service ko inject karna
    }


    /// <summary>
    /// Used-In-Angular: creates insurance.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: creates employee insurance enroll.</para>
    /// <para>Handler flow: CreateEmployeeInsuranceEnrollCommand is processed by CreateEmployeeInsuranceEnrollCommandHandler; operation(s): GetExistingAsync, AddAsync, GetByEnrollmentIdAsync, AddRangeAsync, GetBulkInfo.</para>
    /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeEnrolledResponseDTO: Id (long), EmployeeId (string), PolicyTypeId (int), InsurancePolicyId (int), HasDependent (bool), StartDate (DateTime), EndDate (DateTime?), Dependents (List&lt;GetEmployeeDependentResponsePolicyDTO&gt;?)</para>
    /// <para>Angular function(s): EmployeeInsuranceApi.createInsurance (app/core/services/employee-insurance-api.ts:70).</para>
    /// <para>Angular purpose: creates insurance.</para>
    /// <para>Integrated UI page(s): /app/profile/insurance-info</para>
    /// <para>Angular UI component(s): EmployeeInsuranceForm (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-form/employee-insurance-form.ts); EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts)</para>
    /// </remarks>

    [HttpPost("employee-insurance-enroll")]
    //  [Authorize]
    public async Task<IActionResult> EnrolledEmployee([FromBody] CreateEmployeeEnrolledRequestDTO employeeCreateDto)
    {
        var command = new CreateEmployeeInsuranceEnrollCommand(employeeCreateDto);
         _logger.LogInfo("Creating enrolled employee"); // Log the info message

         var result = await _mediator.Send(command);
        return Ok(result);
    }
    /// <summary>
    /// Used-In-Angular: deletes insurance.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: deletes employee enroll.</para>
    /// <para>Handler flow: DeleteEmployeeEnrollCommand is processed by DeleteEmployeeEnrollCommandHandler; operation(s): GetByEmployeeIdAsync, GetByEnrollmentIdAsync, GetBulkInfo, UpdateAsyncRangeAsync, UpdateAsync.</para>
    /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
    /// <para>Angular function(s): EmployeeInsuranceApi.deleteInsurance (app/core/services/employee-insurance-api.ts:83).</para>
    /// <para>Angular purpose: deletes insurance.</para>
    /// <para>Integrated UI page(s): /app/profile/insurance-info</para>
    /// <para>Angular UI component(s): EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts)</para>
    /// </remarks>
    [HttpDelete("delete")]
    //  [Authorize]
    public async Task<IActionResult> DeleteEnrolledEmployee([FromBody] DeleteEnrolledEmployeePolicyRequestDTO Dto)
    {
        var command = new DeleteEmployeeEnrollCommand(Dto);
         _logger.LogInfo("Delete enrolled employee"); // Log the info message

         var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Used-In-Angular: retrieves enrolled insurances.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves all enroll employee policies.</para>
    /// <para>Handler flow: GetAllEnrollEmployeePoliciesCommand is processed by GetAllEnrolledEmployeeCommandHandler; operation(s): GetByEmployeeIdAsync, GetByEnrollmentIdAsync, Add.</para>
    /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetAllEnrolledEmployeeResponseDTO: EmployeeId (string), Policies (List&lt;GetEmployeeEnrolledResponseDTO&gt;)</para>
    /// <para>Angular function(s): EmployeeInsuranceApi.getEnrolledInsurances (app/core/services/employee-insurance-api.ts:77).</para>
    /// <para>Angular purpose: retrieves enrolled insurances.</para>
    /// <para>Integrated UI page(s): /app/profile/insurance-info</para>
    /// <para>Angular UI component(s): EmployeeInsuranceInfo (app/features/user-menu/employee-profile/employee-insurance-info/employee-insurance-info.ts)</para>
    /// </remarks>
    [HttpGet("get-all-enroll")]
    public async Task<IActionResult> Get([FromQuery] GetEnrolledEmployeeRequestDTO requestDto)
    {

            var command = new GetAllEnrollEmployeePoliciesCommand(requestDto);
        _logger.LogInfo("Get enrolled employee"); // Log the info message
        var result = await _mediator.Send(command);
        return Ok(result);


        }

    }


    //[HttpPost("get-user-self-employement-info")]
    //[HttpPost("update")]
    //
    //
    //
    //
    //public async Task<IActionResult> UpdateEmployeeField([FromBody] GenricUpdateRequestDTO commandDto)
    //{
    //    try
    //    {
    //        ApiResponse<bool> result = ApiResponse<bool>.Fail("Invalid entity name.");

    //        if (commandDto.EntityName == "Employee")
    //        {
    //            var command = new UpdateEmployeeCommand(commandDto);
    //            result = await _mediator.Send(command);

    //            if (result.IsSucceeded)
    //                return Ok(result);
    //        }
    //        if (commandDto.EntityName == "EmployeeContact")
    //        {
    //            var command = new UpdateContactInfoCommand(commandDto);
    //            result = await _mediator.Send(command);

    //            if (result.IsSucceeded)
    //                return Ok(result);
    //        }
    //        else if (commandDto.EntityName == "EmployeeBankDetail")
    //        {
    //            var command = new UpdateBankCommand(commandDto);
    //            result = await _mediator.Send(command);

    //            if (result.IsSucceeded)
    //                return Ok(result);
    //        }
    //        //else if (commandDto.EntityName == "EmployeePersonalDetail")
    //        //{
    //        //    var command = new UpdateIdentityInfoCommand(commandDto);
    //        //    result = await _mediator.Send(command);

    //        //    if (result.IsSucceeded)
    //        //        return Ok(result);
    //        //}
    //        else if (commandDto.EntityName == "EmployeeEducation")
    //        {
    //            var command = new UpdateEducationInfoCommand(commandDto);
    //            result = await _mediator.Send(command);

    //            if (result.IsSucceeded)
    //                return Ok(result);
    //        }

    //        return BadRequest(result);
    //    }
    //    catch (Exception ex)
    //    {
    //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee info.",
    //            new List<string> { ex.Message });
    //        return StatusCode(500, errorResponse);
    //    }









