// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Resolves lightweight dashboard widgets from the authenticated RoleType persona.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.Dashboard;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DashboardCmd;

#region Queries

public sealed record GetDashboardConfigurationQuery(PermissionRequestDTO Permission)
    : IRequest<ApiResponse<DashboardConfigurationDTO>>;

public sealed record GetDashboardWidgetQuery(PermissionRequestDTO Permission, string WidgetCode)
    : IRequest<ApiResponse<DashboardWidgetDTO>>;

#endregion

#region Handler base

public abstract class PersonaDashboardHandlerBase(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantConfigurationHandlerBase> logger)
    : TenantConfigurationHandlerBase(unitOfWork, commonRequestService, logger)
{
    protected async Task<(long TenantId, int RoleTypeId, string RoleTypeCode)> ValidateAsync(
        PermissionRequestDTO permission,
        CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ValidateTenantPermissionAsync(permission, cancellationToken);
        var context = await ValidateTenantDataAccessContextAsync();
        var roleTypeCode = ConstantValues.GetRoleTypeCode(context.RoleTypeId);

        if (string.IsNullOrWhiteSpace(roleTypeCode))
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        return (tenantId, context.RoleTypeId, roleTypeCode);
    }
}

#endregion

#region Configuration

public sealed class GetDashboardConfigurationHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantConfigurationHandlerBase> logger)
    : PersonaDashboardHandlerBase(unitOfWork, commonRequestService, logger),
      IRequestHandler<GetDashboardConfigurationQuery, ApiResponse<DashboardConfigurationDTO>>
{
    public async Task<ApiResponse<DashboardConfigurationDTO>> Handle(
        GetDashboardConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var context = await ValidateAsync(request.Permission, cancellationToken);
        var widgets = DashboardPersonaCatalogue.GetWidgets(context.RoleTypeCode);
        var response = new DashboardConfigurationDTO(
            context.RoleTypeCode,
            ConstantValues.GetRoleTypeDisplayName(context.RoleTypeId),
            widgets);

        return ApiResponse<DashboardConfigurationDTO>.Success(response);
    }
}

#endregion

#region Widget

public sealed class GetDashboardWidgetHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : PersonaDashboardHandlerBase(unitOfWork, commonRequestService, logger),
      IRequestHandler<GetDashboardWidgetQuery, ApiResponse<DashboardWidgetDTO>>
{
    public async Task<ApiResponse<DashboardWidgetDTO>> Handle(
        GetDashboardWidgetQuery request,
        CancellationToken cancellationToken)
    {
        var context = await ValidateAsync(request.Permission, cancellationToken);
        var widgetCode = request.WidgetCode.Trim().ToUpperInvariant();
        var definition = DashboardPersonaCatalogue.GetWidgets(context.RoleTypeCode)
            .SingleOrDefault(widget => widget.Code == widgetCode);

        if (definition is null)
        {
            throw new NotFoundException("Dashboard widget is not available for the authenticated role type.");
        }

        var widget = context.RoleTypeCode == ConstantValues.RoleTypeAdminCode
            ? await BuildTenantAdminWidgetAsync(context.TenantId, definition, cancellationToken)
            : DashboardPersonaCatalogue.BuildPlaceholder(context.RoleTypeCode, definition);

        return ApiResponse<DashboardWidgetDTO>.Success(widget);
    }

    private async Task<DashboardWidgetDTO> BuildTenantAdminWidgetAsync(
        long tenantId,
        DashboardWidgetDefinitionDTO definition,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return definition.Code switch
        {
            DashboardPersonaCatalogue.Summary => MapSummary(
                definition,
                await repository.GetSummaryAsync(tenantId, now, cancellationToken)),
            DashboardPersonaCatalogue.EmployeeOverview => MapEmployeeOverview(
                definition,
                await repository.GetEmployeeOverviewAsync(
                    tenantId,
                    DateOnly.FromDateTime(now),
                    cancellationToken)),
            DashboardPersonaCatalogue.Birthdays => MapEmployees(
                definition,
                await repository.GetUpcomingBirthdaysAsync(tenantId, now, 30, cancellationToken)),
            DashboardPersonaCatalogue.Onboarding => MapEmployees(
                definition,
                await repository.GetRecentOnboardingAsync(tenantId, 10, cancellationToken)),
            DashboardPersonaCatalogue.DepartmentHeadcount => MapDepartments(
                definition,
                await repository.GetDepartmentHeadcountAsync(tenantId, cancellationToken)),
            DashboardPersonaCatalogue.CurrentlyOnLeave => MapLeave(
                definition,
                await repository.GetCurrentlyOnLeaveAsync(
                    tenantId,
                    DateOnly.FromDateTime(now),
                    cancellationToken)),
            DashboardPersonaCatalogue.Locations => MapLocations(
                definition,
                await repository.GetLocationsAsync(tenantId, 3, cancellationToken)),
            _ => DashboardPersonaCatalogue.BuildPlaceholder(
                ConstantValues.RoleTypeAdminCode,
                definition)
        };
    }

    private static DashboardWidgetDTO MapSummary(
        DashboardWidgetDefinitionDTO definition,
        DashboardSummaryDTO data)
    {
        DashboardMetricDTO[] metrics =
        [
            new("TOTAL_EMPLOYEES", "Total Employees", data.TotalEmployees),
            new("NEW_HIRES", "New Hires This Month", data.NewHiresThisMonth),
            new("OPEN_POSITIONS", "Open Positions", data.OpenPositions),
            new("PENDING_APPROVALS", "Pending Approvals", data.PendingApprovals)
        ];
        return Dynamic(definition, metrics, []);
    }

    private static DashboardWidgetDTO MapEmployeeOverview(
        DashboardWidgetDefinitionDTO definition,
        EmployeeOverviewDTO data)
    {
        DashboardMetricDTO[] metrics =
        [
            new("TOTAL", "Total", data.Total),
            new("ACTIVE", "Active", data.Active),
            new("INACTIVE", "Inactive", data.Inactive),
            new("ON_LEAVE", "On Leave", data.OnLeave)
        ];
        return Dynamic(definition, metrics, []);
    }

    private static DashboardWidgetDTO MapEmployees(
        DashboardWidgetDefinitionDTO definition,
        IReadOnlyList<DashboardEmployeeDTO> employees)
    {
        var items = employees.Select(employee => new DashboardWidgetItemDTO(
            employee.EmployeeId,
            employee.EmployeeName,
            string.Join(" · ", new[] { employee.DepartmentName, employee.DesignationName, employee.RoleName }
                .Where(value => !string.IsNullOrWhiteSpace(value))),
            null,
            employee.Date?.ToString("O"))).ToArray();
        return Dynamic(definition, [], items);
    }

    private static DashboardWidgetDTO MapDepartments(
        DashboardWidgetDefinitionDTO definition,
        IReadOnlyList<DepartmentHeadcountDTO> departments)
    {
        var metrics = departments.Select(department => new DashboardMetricDTO(
            department.DepartmentId.ToString(),
            department.DepartmentName,
            department.Headcount)).ToArray();
        return Dynamic(definition, metrics, []);
    }

    private static DashboardWidgetDTO MapLeave(
        DashboardWidgetDefinitionDTO definition,
        IReadOnlyList<DashboardLeaveDTO> employees)
    {
        var items = employees.Select(employee => new DashboardWidgetItemDTO(
            employee.EmployeeId,
            employee.EmployeeName,
            employee.DepartmentName,
            "Approved",
            $"{employee.FromDate:yyyy-MM-dd}/{employee.ToDate:yyyy-MM-dd}"))
            .ToArray();
        return Dynamic(definition, [], items);
    }

    private static DashboardWidgetDTO MapLocations(
        DashboardWidgetDefinitionDTO definition,
        LocationOverviewDTO data)
    {
        DashboardMetricDTO[] metrics =
        [
            new("TOTAL", "Total", data.Total),
            new("ACTIVE", "Active", data.Active),
            new("HEAD_OFFICE", "Head Office", data.HeadOffice)
        ];
        var items = data.Locations.Select(location => new DashboardWidgetItemDTO(
            location.Code,
            location.Name,
            location.LocationType.ToString(),
            location.IsActive ? "Active" : "Inactive",
            null)).ToArray();
        return Dynamic(definition, metrics, items);
    }

    private static DashboardWidgetDTO Dynamic(
        DashboardWidgetDefinitionDTO definition,
        IReadOnlyList<DashboardMetricDTO> metrics,
        IReadOnlyList<DashboardWidgetItemDTO> items)
    {
        return new DashboardWidgetDTO(
            definition.Code,
            definition.Title,
            false,
            "DATABASE",
            metrics,
            items);
    }
}

#endregion

#region Persona catalogue

internal static class DashboardPersonaCatalogue
{
    internal const string Summary = "SUMMARY";
    internal const string EmployeeOverview = "EMPLOYEE_OVERVIEW";
    internal const string Birthdays = "BIRTHDAYS";
    internal const string Onboarding = "RECENT_ONBOARDING";
    internal const string DepartmentHeadcount = "DEPARTMENT_HEADCOUNT";
    internal const string CurrentlyOnLeave = "CURRENTLY_ON_LEAVE";
    internal const string Locations = "LOCATIONS";

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<DashboardWidgetDefinitionDTO>> Widgets =
        new Dictionary<string, IReadOnlyList<DashboardWidgetDefinitionDTO>>(StringComparer.Ordinal)
        {
            [ConstantValues.RoleTypeAdminCode] = Definitions(
                (Summary, "Company Summary", false),
                (EmployeeOverview, "Employee Overview", false),
                (CurrentlyOnLeave, "Currently On Leave", false),
                (Locations, "Locations", false),
                (Birthdays, "Upcoming Birthdays", false),
                ("STORAGE_STATUS", "Storage Status", true),
                ("HIRING_PIPELINE", "Hiring Pipeline", true),
                (DepartmentHeadcount, "Department Headcount", false),
                (Onboarding, "Recent Onboarding", false)),
            [ConstantValues.RoleTypeManagerCode] = Definitions(
                ("TEAM_SUMMARY", "Team Summary", true),
                ("TEAM_ATTENDANCE", "Team Attendance", true),
                ("TEAM_LEAVE", "Team Leave", true),
                ("TEAM_BIRTHDAYS", "Team Birthdays", true),
                ("TEAM_ONBOARDING", "Recent Team Onboarding", true)),
            [ConstantValues.RoleTypeEmployeeCode] = Definitions(
                ("MY_ATTENDANCE", "My Attendance", true),
                ("MY_LEAVE", "My Leave", true),
                ("MY_TASKS", "My Tasks", true),
                ("MY_DOCUMENTS", "My Documents", true),
                ("ANNOUNCEMENTS", "Announcements", true)),
            [ConstantValues.RoleTypeClientCode] = Definitions(
                ("CLIENT_SUMMARY", "Client Summary", true),
                ("OPEN_TICKETS", "Open Tickets", true),
                ("RECENT_ACTIVITY", "Recent Activity", true),
                ("CLIENT_SITE_EMPLOYEES", "Client-side Employees", true),
                ("CLIENT_DOCUMENTS", "Documents", true))
        };

    internal static IReadOnlyList<DashboardWidgetDefinitionDTO> GetWidgets(string roleTypeCode)
    {
        return Widgets.TryGetValue(roleTypeCode, out var widgets)
            ? widgets
            : [];
    }

    internal static DashboardWidgetDTO BuildPlaceholder(
        string roleTypeCode,
        DashboardWidgetDefinitionDTO definition)
    {
        var metrics = PlaceholderMetrics(roleTypeCode, definition.Code);
        var items = PlaceholderItems(roleTypeCode, definition.Code);
        return new DashboardWidgetDTO(
            definition.Code,
            definition.Title,
            true,
            "TEMPORARY_STATIC",
            metrics,
            items);
    }

    private static DashboardWidgetDefinitionDTO[] Definitions(
        params (string Code, string Title, bool IsPlaceholder)[] definitions)
    {
        return definitions.Select((definition, index) => new DashboardWidgetDefinitionDTO(
            definition.Code,
            definition.Title,
            index + 1,
            definition.IsPlaceholder)).ToArray();
    }

    private static IReadOnlyList<DashboardMetricDTO> PlaceholderMetrics(
        string roleTypeCode,
        string widgetCode)
    {
        return (roleTypeCode, widgetCode) switch
        {
            (ConstantValues.RoleTypeAdminCode, "STORAGE_STATUS") =>
                [new("CAPACITY_GB", "Capacity", 50, "GB"), new("USED_GB", "Used", 31.4m, "GB")],
            (ConstantValues.RoleTypeAdminCode, "HIRING_PIPELINE") =>
                [new("APPLIED", "Applied", 148), new("SCREENING", "Screening", 89), new("INTERVIEW", "Interview", 42), new("OFFER", "Offer", 14), new("HIRED", "Hired", 8)],
            (ConstantValues.RoleTypeManagerCode, "TEAM_SUMMARY") =>
                [new("TEAM_MEMBERS", "Team Members", 8), new("PRESENT_TODAY", "Present Today", 7), new("PENDING_APPROVALS", "Pending Approvals", 2)],
            (ConstantValues.RoleTypeManagerCode, "TEAM_ATTENDANCE") =>
                [new("PRESENT", "Present", 7), new("ABSENT", "Absent", 0), new("ON_LEAVE", "On Leave", 1)],
            (ConstantValues.RoleTypeEmployeeCode, "MY_ATTENDANCE") =>
                [new("PRESENT_DAYS", "Present Days", 18), new("WORKING_DAYS", "Working Days", 20)],
            (ConstantValues.RoleTypeEmployeeCode, "MY_LEAVE") =>
                [new("AVAILABLE", "Available", 12, "days"), new("USED", "Used", 6, "days")],
            (ConstantValues.RoleTypeClientCode, "CLIENT_SUMMARY") =>
                [new("ACTIVE_EMPLOYEES", "Active Employees", 6), new("OPEN_TICKETS", "Open Tickets", 3), new("ACTIVE_ENGAGEMENTS", "Active Engagements", 1)],
            (ConstantValues.RoleTypeClientCode, "OPEN_TICKETS") =>
                [new("OPEN", "Open", 3), new("IN_PROGRESS", "In Progress", 2), new("RESOLVED", "Resolved This Month", 5)],
            _ => []
        };
    }

    private static IReadOnlyList<DashboardWidgetItemDTO> PlaceholderItems(
        string roleTypeCode,
        string widgetCode)
    {
        return (roleTypeCode, widgetCode) switch
        {
            (ConstantValues.RoleTypeManagerCode, "TEAM_LEAVE") =>
                [new("LEAVE-001", "Aarav Sharma", "Annual leave", "Approved", "2026-10-06/2026-10-07")],
            (ConstantValues.RoleTypeManagerCode, "TEAM_BIRTHDAYS") =>
                [new("BIRTHDAY-001", "Meera Patel", "Software Engineer", null, "2026-10-12")],
            (ConstantValues.RoleTypeManagerCode, "TEAM_ONBOARDING") =>
                [new("ONBOARD-001", "Kabir Singh", "Quality Analyst", "Active", "2026-10-01")],
            (ConstantValues.RoleTypeEmployeeCode, "MY_TASKS") =>
                [new("TASK-001", "Complete profile verification", "Upload remaining employment document", "Pending", null)],
            (ConstantValues.RoleTypeEmployeeCode, "MY_DOCUMENTS") =>
                [new("DOC-001", "Employment Agreement", "Employee document", "Available", null)],
            (ConstantValues.RoleTypeEmployeeCode, "ANNOUNCEMENTS") =>
                [new("NEWS-001", "Monthly town hall", "Company-wide meeting", "Published", "2026-10-08")],
            (ConstantValues.RoleTypeClientCode, "OPEN_TICKETS") =>
                [new("TKT-1042", "Access request", "Client portal", "Open", "2026-10-03"), new("TKT-1038", "Device connectivity", "Assigned support team", "In Progress", "2026-10-02")],
            (ConstantValues.RoleTypeClientCode, "RECENT_ACTIVITY") =>
                [new("ACT-001", "Weekly status report shared", "Project delivery", "Completed", "2026-10-03"), new("ACT-002", "Ticket TKT-1038 updated", "Support", "In Progress", "2026-10-02")],
            (ConstantValues.RoleTypeClientCode, "CLIENT_SITE_EMPLOYEES") =>
                [new("EMP-001", "Riya Mehta", "Implementation Consultant", "Active", null), new("EMP-002", "Arjun Nair", "Support Engineer", "Active", null)],
            (ConstantValues.RoleTypeClientCode, "CLIENT_DOCUMENTS") =>
                [new("CLIENT-DOC-001", "Service Agreement", "Shared document", "Available", null)],
            _ => []
        };
    }
}

#endregion
