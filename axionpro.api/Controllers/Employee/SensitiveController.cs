// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Sensitive operations.
// ================================================================


using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee Personal & Related operations.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class SensitiveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public SensitiveController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

         /// <summary>
         /// Used-In-Angular: creates employee sensitive.
         /// </summary>
         /// <remarks>
         /// <para>Angular usage status: Used-In-Angular.</para>
         /// <para>API endpoint purpose: creates identity info.</para>
         /// <para>Handler flow: CreateIdentityInfoCommand is processed by CreateEmployeeIdentityCommandHandler; operation(s): Add, CreateAsync, DeleteFileAsync.</para>
         /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
         /// <para>Angular function(s): EmployeeIdentityApi.createEmployeeSensitive (app/core/services/employee-identity-api.ts:75).</para>
         /// <para>Angular purpose: creates employee sensitive.</para>
         /// <para>Integrated UI page(s): /app/profile/identity-info</para>
         /// <para>Angular UI component(s): EmployeeIdentityForm (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-form/employee-identity-form.ts); EmployeeIdentityInfo (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-info.ts)</para>
         /// </remarks>

         [HttpPost("Create")]
        public async Task<IActionResult> Createpersonalinfo([FromForm] CreateEmployeeIdentityRequestDTO dto)
        {

                var command = new CreateIdentityInfoCommand(dto);
                _logger.LogInfo("Creating new employee personal info info...");

                var result = await _mediator.Send(command);
                return Ok(result);

        }

        /// <summary>
        /// Used-In-Angular: retrieves employee identities.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves identity info.</para>
        /// <para>Handler flow: GetIdentityInfoQuery is processed by GetIdentityInfoQueryHandler; operation(s): GetIdentityRecordAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeIdentityResponseDTO: Id (long?), EmployeeId (string?), CountryCode (string), CountryName (string), IdentityCategoryName (string), IdentityCategoryDocumentId (int), DocumentCode (string), DocumentName (string), Description (string?), IsMandatory (bool), EmployeeIdentityId (long?), IdentityValue (string?)</para>
        /// <para>Angular function(s): EmployeeIdentityApi.getEmployeeIdentities (app/core/services/employee-identity-api.ts:69).</para>
        /// <para>Angular purpose: retrieves employee identities.</para>
        /// <para>Integrated UI page(s): /app/profile/identity-info</para>
        /// <para>Angular UI component(s): EmployeeIdentityInfo (app/features/user-menu/employee-profile/employee-identity-info/employee-identity-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]
        public async Task<IActionResult> GetSensitiveData([FromQuery] GetIdentityRequestDTO commandDto)
        {
                var command = new GetIdentityInfoQuery(commandDto);
                var result = await _mediator.Send(command);

                    return Ok(result);


        }
        /// <summary>
        /// Updates employee details.
        /// </summary>
        //[HttpPost("update")]
        //
        //
        //
        //public async Task<IActionResult> Update([FromBody] GenricUpdateRequestDTO dto)
        //{
        //    try
        //    {
        //        _logger.LogInfo($"Updating employee-personal info record. EmployeeId: {dto._EmployeeId}");

        //        var command = new UpdateIdentityInfoCommand(dto);
        //        var result = await _mediator.Send(command);

        //        if (!result.IsSucceeded)
        //        {
        //            _logger.LogInfo($"Failed to update employee-personal info with Id: {dto._EmployeeId}");
        //            return BadRequest(result);
        //        }

        //        _logger.LogInfo("Employee-personal info updated successfully.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating employee-personal info: {ex.Message}");
        //        var errorResponse = ApiResponse<bool>.Fail("An unexpected error occurred while updating employee-personal info info.",
        //            new List<string> { ex.Message });
        //        return StatusCode(500, errorResponse);
        //    }
        //}


    }
}
