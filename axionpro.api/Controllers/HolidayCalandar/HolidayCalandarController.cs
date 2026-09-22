// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates tenant holiday calendar CRUD requests.
// ================================================================

using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.Features.HolidayCalandarCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HolidayCalandar;

/// <summary>
/// Manages location-based organization holidays for the authenticated tenant.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class HolidayCalandarController(IMediator mediator) : ControllerBase
{
    #region Read

    /// <summary>Lists active holidays for the current tenant.</summary>
    /// <remarks>
    /// ModuleId must identify TENANT_POLICY_HOLIDAY_CALENDAR and OperationId must
    /// identify View. TenantLocationId and HolidayYear are optional filters.
    /// Tenant identity comes from the authenticated request, not the query.
    /// </remarks>
    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] BasicRequestDTO request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ListHolidaysQuery(request), cancellationToken));
    }

    /// <summary>Gets one nondeleted holiday, including inactive, belonging to the current tenant.</summary>
    /// <remarks>Requires the View operation for TENANT_POLICY_HOLIDAY_CALENDAR.</remarks>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(
        long id,
        [FromQuery] HolidayByIdRequestDTO request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        return Ok(await mediator.Send(new GetHolidayQuery(request), cancellationToken));
    }

    #endregion

    #region Write

    /// <summary>Creates a holiday for an active location owned by the current tenant.</summary>
    /// <remarks>Requires the Add operation for TENANT_POLICY_HOLIDAY_CALENDAR.</remarks>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] SaveHolidayRequestDTO request,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new CreateHolidayCommand(request), cancellationToken));
    }

    /// <summary>Updates an active holiday belonging to the current tenant.</summary>
    /// <remarks>Requires the Update operation for TENANT_POLICY_HOLIDAY_CALENDAR.</remarks>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateHolidayRequestDTO request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        return Ok(await mediator.Send(new UpdateHolidayCommand(request), cancellationToken));
    }

    /// <summary>Soft-deletes an active holiday belonging to the current tenant.</summary>
    /// <remarks>Requires the Delete operation for TENANT_POLICY_HOLIDAY_CALENDAR.</remarks>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        [FromQuery] HolidayByIdRequestDTO request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        return Ok(await mediator.Send(new DeleteHolidayCommand(request), cancellationToken));
    }

    #endregion

    #region Import and export

    /// <summary>Imports bounded CSV/XLSX holidays for active tenant locations.</summary>
    /// <remarks>
    /// Requires Import permission. Supply exactly one CSV/XLSX file or pasted
    /// table, with TenantLocationId, HolidayName, HolidayDate, IsOptional and
    /// Description columns. Invalid input or an existing nondeleted date saves
    /// no rows; inactive entries also block the date until soft-deleted.
    /// </remarks>
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(
        [FromForm] ImportHolidayRequestDTO request,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ImportHolidaysCommand(request), cancellationToken));
    }

    /// <summary>Exports active tenant holidays as a reusable UTF-8 CSV file.</summary>
    /// <remarks>
    /// Requires Export permission. Optional TenantLocationId and HolidayYear
    /// filters use the same tenant scope as GET /get.
    /// </remarks>
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] BasicRequestDTO request,
        CancellationToken cancellationToken)
    {
        var content = await mediator.Send(new ExportHolidaysQuery(request), cancellationToken);
        return File(content, "text/csv; charset=utf-8", "organization-holidays.csv");
    }

    #endregion
}
