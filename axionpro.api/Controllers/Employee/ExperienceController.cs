// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Experience operations.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;

using axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee
{
    /// <summary>
    /// Handles all Employee Experience & Related operations.
    /// </summary>
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ExperienceController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // <summary>
        // Create new employee experience record.
        // </summary>
        /// <summary>
        /// Used-In-Angular: creates employee experience.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: creates experience info.</para>
        /// <para>Handler flow: CreateExperienceInfoCommand is processed by CreateExperienceInfoCommandHandler; operation(s): GetExtension, Add, AddAsync, SaveChangesAsync, GetFileUrl.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeExperienceResponseDTO: Id (long), EmployeeId (string?), Ctc (decimal?), CompanyName (string?), Designation (string?), EmployeeIdOfCompany (string?), StartDate (DateTime?), EndDate (DateTime?), Experience (int?), IsWFH (bool), WorkingCountryId (int?), WorkingStateId (int?)</para>
        /// <para>Angular function(s): EmployeeExperienceAPI.createEmployeeExperience (app/core/services/employee-experience-api.ts:104).</para>
        /// <para>Angular purpose: creates employee experience.</para>
        /// <para>Integrated UI page(s): /app/profile/experience-info</para>
        /// <para>Angular UI component(s): EmployeeExperienceForm (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-form/employee-experience-form.ts); EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts)</para>
        /// </remarks>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateExperience([FromForm] CreateExperienceRequestDTO dto)
        {
            _logger.LogInfo("Received Experience Create Request for Employee: " + dto.EmployeeId);
            var command = new CreateExperienceInfoCommand(dto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: retrieves employee experience.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves experience info.</para>
        /// <para>Handler flow: GetExperienceInfoQuery is processed by GetExperienceInfoQueryHandler; operation(s): GetByEmployeeIdWithDocumentsAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeExperienceResponseDTO: Id (long), EmployeeId (string?), Ctc (decimal?), CompanyName (string?), Designation (string?), EmployeeIdOfCompany (string?), StartDate (DateTime?), EndDate (DateTime?), Experience (int?), IsWFH (bool), WorkingCountryId (int?), WorkingStateId (int?)</para>
        /// <para>Angular function(s): EmployeeExperienceAPI.getEmployeeExperience (app/core/services/employee-experience-api.ts:98).</para>
        /// <para>Angular purpose: retrieves employee experience.</para>
        /// <para>Integrated UI page(s): /app/profile/experience-info</para>
        /// <para>Angular UI component(s): EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts)</para>
        /// </remarks>
        [HttpGet("get")]
        //
        //
        public async Task<IActionResult> GetAllexperinceInfo([FromQuery] GetExperienceRequestDTO commandDto)
        {

                var command = new GetExperienceInfoQuery(commandDto);
                var result = await _mediator.Send(command);

                    return Ok(result);


        }
        /// <summary>
        /// Used-In-Angular: updates employee experience.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: updates experience info.</para>
        /// <para>Handler flow: UpdateExperienceInfoCommand is processed by UpdateExperienceInfoCommandHandler; operation(s): GetByIdAsync, UpdateAsync, SaveChangesAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeExperienceAPI.updateEmployeeExperience (app/core/services/employee-experience-api.ts:110).</para>
        /// <para>Angular purpose: updates employee experience.</para>
        /// <para>Integrated UI page(s): /app/profile/experience-info</para>
        /// <para>Angular UI component(s): EmployeeExperienceForm (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-form/employee-experience-form.ts); EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts)</para>
        /// </remarks>
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateExperienceRequestDTO dto)
        {
              _logger.LogInfo($"Updating employee-experience record. EmployeeId: {dto.EmployeeId}");
                var command = new UpdateExperienceInfoCommand(dto);
                var result = await _mediator.Send(command);
                _logger.LogInfo("Employee-experience updated successfully.");
                return Ok(result);

        }
        /// <summary>
        /// Used-In-Angular: deletes employee experience.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes experience.</para>
        /// <para>Handler flow: DeleteExperienceCommand is processed by DeleteExperienceCommandHandler; operation(s): GetByIdAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeExperienceAPI.deleteEmployeeExperience (app/core/services/employee-experience-api.ts:117).</para>
        /// <para>Angular purpose: deletes employee experience.</para>
        /// <para>Integrated UI page(s): /app/profile/experience-info</para>
        /// <para>Angular UI component(s): EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts)</para>
        /// </remarks>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteRequestDTO dto)
        {
            _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

            var command = new DeleteExperienceCommand(dto);
            var result = await _mediator.Send(command);


            _logger.LogInfo("Education deleted successfully.");
            return Ok(result);


        }
        /// <summary>
        /// Used-In-Angular: deletes employee experience doc.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: deletes experience doc.</para>
        /// <para>Handler flow: DeleteExperienceDocCommand is processed by DeleteExperienceDocCommandHandler; operation(s): GetSingleByDetailIdAsync, SaveChangesAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeeExperienceAPI.deleteEmployeeExperienceDoc (app/core/services/employee-experience-api.ts:123).</para>
        /// <para>Angular purpose: deletes employee experience doc.</para>
        /// <para>Integrated UI page(s): /app/profile/experience-info</para>
        /// <para>Angular UI component(s): EmployeeExperienceForm (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-form/employee-experience-form.ts); EmployeeExperienceInfo (app/features/user-menu/employee-profile/employee-experience-info/employee-experience-info.ts)</para>
        /// </remarks>
        [HttpDelete("delete-doc")]
        public async Task<IActionResult> DeleteDoc([FromQuery] DeleteRequestDTO dto)
        {
            _logger.LogInfo($"Deleting employee with Id: {dto.Id}");

            var command = new DeleteExperienceDocCommand(dto);
            var result = await _mediator.Send(command);


            _logger.LogInfo("Education deleted successfully.");
            return Ok(result);


        }
    }
}
