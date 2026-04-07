
 
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
    ///Create new employee.
    /// </summary>
    
    [HttpPost("employee-insurance-enroll")]
    //  [Authorize]   
    public async Task<IActionResult> EnrolledEmployee([FromBody] CreateEmployeeEnrolledRequestDTO employeeCreateDto)
    {
        var command = new CreateEmployeeInsuranceEnrollCommand(employeeCreateDto);
         _logger.LogInfo("Creating enrolled employee"); // Log the info message

         var result = await _mediator.Send(command);       
        return Ok(result);
    }
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
    /// Get all employees that belong to the specified tenant.
    /// </summary>
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
  








