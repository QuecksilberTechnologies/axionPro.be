using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Features.DashboardCmd;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Dashboard;

/// <summary>
/// Supplies independently loadable dashboard widgets through the existing permission pipeline.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController(IMediator mediator, ILoggerService logger) : ControllerBase
{
    /// <summary>Returns the authenticated RoleType persona and its independently loadable widgets.</summary>
    [HttpGet("Configuration")]
    public async Task<IActionResult> Configuration(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetDashboardConfigurationQuery(permission),
            cancellationToken));
    }

    /// <summary>Returns one widget allowed for the authenticated RoleType persona.</summary>
    [HttpGet("Widget/{widgetCode}")]
    public async Task<IActionResult> Widget(
        [FromRoute] string widgetCode,
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetDashboardWidgetQuery(permission, widgetCode),
            cancellationToken));
    }

    /// <summary>Returns Tenant Super-Admin dashboard totals.</summary>
    [HttpGet("SuperAdmin/Summary")]
    public async Task<IActionResult> Summary(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        logger.LogInfo("Received Super-Admin dashboard summary request.");
        return Ok(await mediator.Send(new GetSuperAdminSummaryQuery(permission), cancellationToken));
    }

    /// <summary>Returns active, inactive, and currently-on-leave employee totals.</summary>
    [HttpGet("SuperAdmin/EmployeeOverview")]
    public async Task<IActionResult> EmployeeOverview(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSuperAdminEmployeeOverviewQuery(permission), cancellationToken));
    }

    /// <summary>Returns upcoming birthdays with department, designation, and role context.</summary>
    [HttpGet("SuperAdmin/Birthdays")]
    public async Task<IActionResult> Birthdays(
        [FromQuery] PermissionRequestDTO permission,
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetSuperAdminBirthdaysQuery(permission, days), cancellationToken));
    }

    /// <summary>Returns the most recently onboarded employees.</summary>
    [HttpGet("SuperAdmin/Onboarding")]
    public async Task<IActionResult> Onboarding(
        [FromQuery] PermissionRequestDTO permission,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetSuperAdminOnboardingQuery(permission, limit), cancellationToken));
    }

    /// <summary>Returns active employee headcount grouped by active department.</summary>
    [HttpGet("SuperAdmin/DepartmentHeadcount")]
    public async Task<IActionResult> DepartmentHeadcount(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSuperAdminDepartmentHeadcountQuery(permission), cancellationToken));
    }

    /// <summary>Returns employees whose approved, non-cancelled leave includes today.</summary>
    [HttpGet("SuperAdmin/CurrentlyOnLeave")]
    public async Task<IActionResult> CurrentlyOnLeave(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSuperAdminLeaveQuery(permission), cancellationToken));
    }

    /// <summary>Returns tenant location totals and a limited location list.</summary>
    [HttpGet("SuperAdmin/Locations")]
    public async Task<IActionResult> Locations(
        [FromQuery] PermissionRequestDTO permission,
        [FromQuery] int limit = 3,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetSuperAdminLocationsQuery(permission, limit), cancellationToken));
    }

    /// <summary>Returns temporary storage values until authoritative storage metering is available.</summary>
    [HttpGet("SuperAdmin/Storage")]
    public async Task<IActionResult> Storage(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSuperAdminStorageQuery(permission), cancellationToken));
    }

    /// <summary>Returns temporary hiring values until the recruitment module is available.</summary>
    [HttpGet("SuperAdmin/HiringPipeline")]
    public async Task<IActionResult> HiringPipeline(
        [FromQuery] PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetSuperAdminHiringPipelineQuery(permission), cancellationToken));
    }
}
