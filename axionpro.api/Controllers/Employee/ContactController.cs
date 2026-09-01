// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Contact operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Contact;

using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee-Contact related operations like create, update, delete, and view.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ContactController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Used-In-Angular: creates employee contact.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: creates contact info.</para>
        /// <para>Handler flow: CreateContactInfoCommand is processed by CreateContactInfoCommandHandler; operation(s): CreateAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetContactResponseDTO: Id (long), EmployeeId (string?), ContactName (string?), Relation (int?), ContactType (int?), ContactNumber (string?), AlternateNumber (string?), Email (string?), IsPrimary (bool), CountryName (string?), StateName (string?), DistrictName (string?), CountryId (int?), StateId (int?), DistrictId (int?), HouseNo (string?), LandMark (string?), Street (string?), Address (string?), Remark (string?), IsActive (bool?), IsEditAllowed (bool?), IsInfoVerified (bool?), InfoVerifiedById (string?), InfoVerifiedDateTime (DateTime?), Description (string?), CompletionPercentage (double)</para>
        /// <para>Angular function(s): EmployeeContactsAPI.createEmployeeContact (app/core/services/employee-contacts-api.ts:79).</para>
        /// <para>Angular purpose: creates employee contact.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): EmployeeContactForm (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-form/employee-contact-form.ts)</para>
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> CreateContactInfo([FromBody] CreateContactRequestDTO Dto)
        {
                // ✅ IMEI validation
                if (Dto == null)
                {
                    _logger.LogInfo($"Invalid IMEI: {Dto}");
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                _logger.LogInfo("Creating new empolyee contact process started.");

                var command = new CreateContactInfoCommand(Dto);
                var result = await _mediator.Send(command);


                _logger.LogInfo("Employee-contact created successfully.");
                return Ok(result);
            }




        /// <summary>
        /// Used-In-Angular: retrieves employee contacts.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves contact info.</para>
        /// <para>Handler flow: GetContactInfoQuery is processed by GetContactInfoQueryHandler; operation(s): GetInfo.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetContactResponseDTO: Id (long), EmployeeId (string?), ContactName (string?), Relation (int?), ContactType (int?), ContactNumber (string?), AlternateNumber (string?), Email (string?), IsPrimary (bool), CountryName (string?), StateName (string?), DistrictName (string?), CountryId (int?), StateId (int?), DistrictId (int?), HouseNo (string?), LandMark (string?), Street (string?), Address (string?), Remark (string?), IsActive (bool?), IsEditAllowed (bool?), IsInfoVerified (bool?), InfoVerifiedById (string?), InfoVerifiedDateTime (DateTime?), Description (string?), CompletionPercentage (double)</para>
        /// <para>Angular function(s): EmployeeContactsAPI.getEmployeeContacts (app/core/services/employee-contacts-api.ts:86).</para>
        /// <para>Angular purpose: retrieves employee contacts.</para>
        /// <para>Integrated UI page(s): /app/profile/contact-info</para>
        /// <para>Angular UI component(s): EmployeeContactInfo (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]
                public async Task<IActionResult> GetBankinfo([FromQuery] GetContactRequestDTO requestDto)

            {
                _logger.LogInfo("Fetching all bank.");

                var command = new GetContactInfoQuery(requestDto);
                var result = await _mediator.Send(command);
                return Ok(result);

        }



        /// <summary>
        /// Used-In-Angular: updates employee contact.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates employee contact.</para>
        /// <para>Handler flow: UpdateEmployeeContactCommand is processed by UpdateContactInfoCommandHandler; operation(s): GetSingleRecordAsync, UpdateContactAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeContactsAPI.updateEmployeeContact (app/core/services/employee-contacts-api.ts:92).</para>
        /// <para>Angular purpose: updates employee contact.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): EmployeeContactForm (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-form/employee-contact-form.ts)</para>
        /// </remarks>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequestDTO dto)
        {
                _logger.LogInfo($"Updating employee-contact record. EmployeeId: {dto.Id}");

                var command = new UpdateEmployeeContactCommand(dto);
                var result = await _mediator.Send(command);



                _logger.LogInfo("Employee-contact updated successfully.");
                return Ok(result);


        }

        /// <summary>
        /// Used-In-Angular: deletes employee contact.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes contact.</para>
        /// <para>Handler flow: DeleteContactQuery is processed by DeleteContactInfoQueryHandler; operation(s): GetSingleRecordAsync, DeleteAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeContactsAPI.deleteEmployeeContact (app/core/services/employee-contacts-api.ts:99).</para>
        /// <para>Angular purpose: deletes employee contact.</para>
        /// <para>Integrated UI page(s): /app/profile/contact-info</para>
        /// <para>Angular UI component(s): EmployeeContactInfo (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-info.ts)</para>
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {

                _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

                var command = new DeleteContactQuery(dto);
                var result = await _mediator.Send(command);


                _logger.LogInfo("Employee deleted successfully.");
                return Ok(result);

        }
    }
}
