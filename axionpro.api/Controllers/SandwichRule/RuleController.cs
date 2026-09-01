// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Rule operations.
// ================================================================


using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;

using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.SandwichRule
{
    /// <summary>
    /// handled-sandwich-related-operations.
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]


    public class RuleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public RuleController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #region CRUD-GET-DAY-COMBINATION BY-TENANT-ADMIN
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: creates day combination.</para>
                /// <para>Handler flow: CreateDayCombinationCommand is processed by CreateDayCombinationCommandHandler; operation(s): AddDayCombinationAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDayCombinationResponseDTO: Id (int), TenantId (long?), CombinationName (string), StartDay (int), EndDay (int), Remark (string?), IsActive (bool), IsSoftDeleted (bool), AddedById (long), AddedDateTime (DateTime), UpdatedById (long?), UpdatedDateTime (DateTime?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/daycombination/add.</para>
                /// </remarks>

                [HttpPost("/Sandwich/DayCombination/add")]
                public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] CreateDayCombinationRequestDTO dTO)
                {
                 // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

                    var query = new CreateDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
                    var result = await _mediator.Send(query);
                    return Ok(result);
                }
        #endregion
        #region Update--DAY-COMBINATION BY-TENANT-ADMIN
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: updates day combination.</para>
                /// <para>Handler flow: UpdateDayCombinationCommand is processed by UpdateDayCombinationCommandHandler; operation(s): UpdateDayCombinationAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/daycombination/update.</para>
                /// </remarks>

                [HttpPost("/Sandwich/DayCombination/update")]
                public async Task<IActionResult> UpdateDayCombinationByTenantUser([FromBody] UpdateDayCombinationRequestDTO dto)
                {
                    var query = new UpdateDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
                    var result = await _mediator.Send(query);

                    return Ok(result);
                }
        #endregion
        #region Update-DAY-COMBINATION BY-TENANT-ADMIN
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: deletes day combination.</para>
                /// <para>Handler flow: DeleteDayCombinationCommand is processed by DeleteDayCombinationCommandHandler; operation(s): DeleteDayCombinationAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/daycombination/delete.</para>
                /// </remarks>

                [HttpPost("/Sandwich/DayCombination/delete")]
                public async Task<IActionResult> DeleteDayCombinationByTenantUser([FromBody] DeleteDayCombinationRequestDTO dto)
                {
                               // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

                    var query = new DeleteDayCombinationCommand(dto);  //  Fix: No parameter needed in GetAllAssetQuery
                    var result = await _mediator.Send(query);

                    return Ok(result);
                }
        #endregion
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves day combination.</para>
                /// <para>Handler flow: GetDayCombinationCommand is processed by GetDayCombinationCommandHandler; operation(s): GetAllActiveDayCombinationsAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetDayCombinationResponseDTO: Id (int), TenantId (long?), CombinationName (string), StartDay (int), EndDay (int), Remark (string?), IsActive (bool), IsSoftDeleted (bool), AddedById (long), AddedDateTime (DateTime), UpdatedById (long?), UpdatedDateTime (DateTime?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/daycombination/get.</para>
                /// </remarks>

                [HttpPost("/Sandwich/DayCombination/get")]
                public async Task<IActionResult> GetAllDayCombinationByTenantUser([FromBody] GetDayCombinationRequestDTO dTO)
                {

                    // _logger.LogInformation("Received request to get Assets for userId: {LoginId}", AssetRequestDTO.Id);

                    var query = new GetDayCombinationCommand(dTO);  //  Fix: No parameter needed in GetAllAssetQuery
                    var result = await _mediator.Send(query);

                    return Ok(result);
                }

        #region CRUD-SANDWICH-RULE-BY-TENANT-ADMIN

        // 🔹 GET ALL
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves sandwich rule.</para>
                /// <para>Handler flow: GetSandwichRuleCommand is processed by GetSandwichRuleCommandHandler; operation(s): GetAllActiveSandwichRulesAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveSandwitchRuleResponseDTO: Id (long), TenantId (long?), RuleName (string?), IsIncludeHoliday (bool), IsIncludeWeekend (bool), IsActive (bool), Remark (string?), AddedById (long?), AddedDateTime (DateTime), UpdatedById (long?), UpdatedDateTime (DateTime?), IsSoftDeleted (bool)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/sandwich/get.</para>
                /// </remarks>
                [HttpGet("/Sandwich/get")]
                public async Task<IActionResult> GetAllSandwichRule([FromQuery] GetLeaveSandwitchRuleRequestDTO dto)
                {

                    var query = new GetSandwichRuleCommand(dto);
                    var result = await _mediator.Send(query);

                    return Ok(result);
                }

        // 🔹 CREATE
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: creates sandwich rule.</para>
                /// <para>Handler flow: CreateSandwichRuleCommand is processed by CreateSandwichRuleCommandHandler; operation(s): AddSandwichAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetLeaveSandwitchRuleResponseDTO: Id (long), TenantId (long?), RuleName (string?), IsIncludeHoliday (bool), IsIncludeWeekend (bool), IsActive (bool), Remark (string?), AddedById (long?), AddedDateTime (DateTime), UpdatedById (long?), UpdatedDateTime (DateTime?), IsSoftDeleted (bool)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/add.</para>
                /// </remarks>
                [HttpPost("/Sandwich/add")]
                public async Task<IActionResult> CreateSandwichRule([FromBody] CreateLeaveSandwichRuleRequestDTO dto)
                {

                    var command = new CreateSandwichRuleCommand(dto);
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }

        // 🔹 UPDATE
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: updates sandwich rule.</para>
                /// <para>Handler flow: UpdateSandwichRuleCommand is processed by UpdateSandwichRuleCommandHandler; operation(s): UpdateSandwichAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/sandwich/update.</para>
                /// </remarks>
                [HttpPost("/Sandwich/update")]
                public async Task<IActionResult> UpdateSandwichRule([FromBody] UpdateLeaveSandwitchRuleRequestDTO dto)
                {


                    var command = new UpdateSandwichRuleCommand(dto);
                    var result = await _mediator.Send(command);


                    return Ok(result);
                }

        // 🔹 DELETE (Soft Delete)
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: deletes sandwich rule.</para>
                /// <para>Handler flow: DeleteSandwichRuleCommand is processed by DeleteSandwichRuleCommandHandler; operation(s): DeleteSandwichAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: DELETE /api/sandwich/delete.</para>
                /// </remarks>
                [HttpDelete("/Sandwich/delete")]
                public async Task<IActionResult> DeleteSandwichRule([FromQuery] DeleteLeaveSandwitchRuleRequestDTO dto)
                {

                    var command = new DeleteSandwichRuleCommand(dto);
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }

        #endregion

    }

}

