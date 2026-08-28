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
    /// Supports the Angular UI flow for enrolled employee.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: creates insurance.</para>
    /// <para>Angular page(s): /app/profile/insurance-info.</para>
    /// <para>Angular API service call(s): EmployeeInsuranceApi.createInsurance (app/core/services/employee-insurance-api.ts:66).</para>
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
    /// Supports the Angular UI flow for delete enrolled employee.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: deletes insurance.</para>
    /// <para>Angular page(s): /app/profile/insurance-info.</para>
    /// <para>Angular API service call(s): EmployeeInsuranceApi.deleteInsurance (app/core/services/employee-insurance-api.ts:79).</para>
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
    /// Supports the Angular UI flow for get.
    /// </summary>
    /// <remarks>
    /// <para>Angular purpose: retrieves enrolled insurances.</para>
    /// <para>Angular page(s): /app/profile/insurance-info.</para>
    /// <para>Angular API service call(s): EmployeeInsuranceApi.getEnrolledInsurances (app/core/services/employee-insurance-api.ts:73).</para>
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
  








