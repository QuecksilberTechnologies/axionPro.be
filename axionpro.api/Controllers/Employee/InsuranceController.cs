
using Asp.Versioning;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.Contact.Command;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.AccessControl;

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
    
    [HttpPost("create")]

    //  [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateBaseEmployeeRequestDTO employeeCreateDto)
    {
        var command = new CreateBaseEmployeeInfoCommand(employeeCreateDto);
         _logger.LogInfo("Creating new employee-contact"); // Log the info message

         var result = await _mediator.Send(command);
        if (!result.IsSucceeded)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Get all employees that belong to the specified tenant.
    /// </summary>
    [HttpGet("get")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] GetContactRequestDTO requestDto)
    {
        try
        {
             
            
            var command = new GetContactInfoQuery(requestDto);

            // ✅ Send command instead of DTO
            ApiResponse<List<GetContactResponseDTO>> result = await _mediator.Send(command);

            if (result.IsSucceeded)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while fetching employee info.",
                new List<string> { ex.Message });
            return StatusCode(500, errorResponse);
        }
    }

    //[HttpPost("get-user-self-employement-info")]
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmployeeField([FromBody] GenricUpdateRequestDTO commandDto)
    {
        try
        {
            ApiResponse<bool> result = ApiResponse<bool>.Fail("Invalid entity name.");

            if (commandDto.EntityName == "Employee")
            {
                var command = new UpdateEmployeeCommand(commandDto);
                result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);
            }
            if (commandDto.EntityName == "EmployeeContact")
            {
                var command = new UpdateContactInfoCommand(commandDto);
                result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);
            }
            else if (commandDto.EntityName == "EmployeeBankDetail")
            {
                var command = new UpdateBankCommand(commandDto);
                result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);
            }
            //else if (commandDto.EntityName == "EmployeePersonalDetail")
            //{
            //    var command = new UpdateIdentityInfoCommand(commandDto);
            //    result = await _mediator.Send(command);

            //    if (result.IsSucceeded)
            //        return Ok(result);
            //}
            else if (commandDto.EntityName == "EmployeeEducation")
            {
                var command = new UpdateEducationInfoCommand(commandDto);
                result = await _mediator.Send(command);

                if (result.IsSucceeded)
                    return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee info.",
                new List<string> { ex.Message });
            return StatusCode(500, errorResponse);
        }
    }







}

