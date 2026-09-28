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

public sealed record GetSuperAdminSummaryQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<DashboardSummaryDTO>>;
public sealed record GetSuperAdminEmployeeOverviewQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<EmployeeOverviewDTO>>;
public sealed record GetSuperAdminBirthdaysQuery(PermissionRequestDTO Permission, int Days = 30) : IRequest<ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>>;
public sealed record GetSuperAdminOnboardingQuery(PermissionRequestDTO Permission, int Limit = 10) : IRequest<ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>>;
public sealed record GetSuperAdminDepartmentHeadcountQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<IReadOnlyList<DepartmentHeadcountDTO>>>;
public sealed record GetSuperAdminLeaveQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<IReadOnlyList<DashboardLeaveDTO>>>;
public sealed record GetSuperAdminLocationsQuery(PermissionRequestDTO Permission, int Limit = 3) : IRequest<ApiResponse<LocationOverviewDTO>>;
public sealed record GetSuperAdminStorageQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<StorageStatusDTO>>;
public sealed record GetSuperAdminHiringPipelineQuery(PermissionRequestDTO Permission) : IRequest<ApiResponse<HiringPipelineDTO>>;

public abstract class SuperAdminDashboardHandlerBase(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<TenantConfigurationHandlerBase> logger) : TenantConfigurationHandlerBase(unitOfWork, commonRequestService, logger)
{
    protected async Task<long> ValidateAsync(PermissionRequestDTO permission, CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ValidateTenantPermissionAsync(permission, cancellationToken);
        var context = await ValidateTenantDataAccessContextAsync();
        if (context.RoleTypeId != ConstantValues.RoleTypeAdmin)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
        return tenantId;
    }
}

public sealed class GetSuperAdminSummaryHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminSummaryQuery, ApiResponse<DashboardSummaryDTO>>
{
    public async Task<ApiResponse<DashboardSummaryDTO>> Handle(
        GetSuperAdminSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var summary = await repository.GetSummaryAsync(tenantId, DateTime.UtcNow, cancellationToken);
        return ApiResponse<DashboardSummaryDTO>.Success(summary);
    }
}
public sealed class GetSuperAdminEmployeeOverviewHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminEmployeeOverviewQuery, ApiResponse<EmployeeOverviewDTO>>
{
    public async Task<ApiResponse<EmployeeOverviewDTO>> Handle(
        GetSuperAdminEmployeeOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var overview = await repository.GetEmployeeOverviewAsync(
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);
        return ApiResponse<EmployeeOverviewDTO>.Success(overview);
    }
}
public sealed class GetSuperAdminBirthdaysHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminBirthdaysQuery, ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>>
{
    public async Task<ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>> Handle(
        GetSuperAdminBirthdaysQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var birthdays = await repository.GetUpcomingBirthdaysAsync(
            tenantId,
            DateTime.UtcNow,
            Math.Clamp(request.Days, 1, 366),
            cancellationToken);
        return ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>.Success(birthdays);
    }
}
public sealed class GetSuperAdminOnboardingHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminOnboardingQuery, ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>>
{
    public async Task<ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>> Handle(
        GetSuperAdminOnboardingQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var employees = await repository.GetRecentOnboardingAsync(
            tenantId,
            Math.Clamp(request.Limit, 1, 50),
            cancellationToken);
        return ApiResponse<IReadOnlyList<DashboardEmployeeDTO>>.Success(employees);
    }
}
public sealed class GetSuperAdminDepartmentHeadcountHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminDepartmentHeadcountQuery, ApiResponse<IReadOnlyList<DepartmentHeadcountDTO>>>
{
    public async Task<ApiResponse<IReadOnlyList<DepartmentHeadcountDTO>>> Handle(
        GetSuperAdminDepartmentHeadcountQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var departments = await repository.GetDepartmentHeadcountAsync(tenantId, cancellationToken);
        return ApiResponse<IReadOnlyList<DepartmentHeadcountDTO>>.Success(departments);
    }
}
public sealed class GetSuperAdminLeaveHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminLeaveQuery, ApiResponse<IReadOnlyList<DashboardLeaveDTO>>>
{
    public async Task<ApiResponse<IReadOnlyList<DashboardLeaveDTO>>> Handle(
        GetSuperAdminLeaveQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var employees = await repository.GetCurrentlyOnLeaveAsync(
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);
        return ApiResponse<IReadOnlyList<DashboardLeaveDTO>>.Success(employees);
    }
}
public sealed class GetSuperAdminLocationsHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger,
    ISuperAdminDashboardRepository repository)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminLocationsQuery, ApiResponse<LocationOverviewDTO>>
{
    public async Task<ApiResponse<LocationOverviewDTO>> Handle(
        GetSuperAdminLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = await ValidateAsync(request.Permission, cancellationToken);
        var locations = await repository.GetLocationsAsync(
            tenantId,
            Math.Clamp(request.Limit, 1, 50),
            cancellationToken);
        return ApiResponse<LocationOverviewDTO>.Success(locations);
    }
}
public sealed class GetSuperAdminStorageHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminStorageQuery, ApiResponse<StorageStatusDTO>>
{
    public async Task<ApiResponse<StorageStatusDTO>> Handle(
        GetSuperAdminStorageQuery request,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(request.Permission, cancellationToken);
        var categories = new[]
        {
            new StorageCategoryDTO("Documents", 14.2m),
            new StorageCategoryDTO("Media", 9.8m),
            new StorageCategoryDTO("Videos", 5.6m),
            new StorageCategoryDTO("Other", 1.8m)
        };
        return ApiResponse<StorageStatusDTO>.Success(new StorageStatusDTO(50m, 31.4m, categories));
    }
}
public sealed class GetSuperAdminHiringPipelineHandler(
    IUnitOfWork uow,
    ICommonRequestService common,
    ILogger<TenantConfigurationHandlerBase> logger)
    : SuperAdminDashboardHandlerBase(uow, common, logger),
      IRequestHandler<GetSuperAdminHiringPipelineQuery, ApiResponse<HiringPipelineDTO>>
{
    public async Task<ApiResponse<HiringPipelineDTO>> Handle(
        GetSuperAdminHiringPipelineQuery request,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(request.Permission, cancellationToken);
        var stages = new[]
        {
            new HiringStageDTO("Applied", 148),
            new HiringStageDTO("Screening", 89),
            new HiringStageDTO("Interview", 42),
            new HiringStageDTO("Offer", 14),
            new HiringStageDTO("Hired", 8)
        };
        return ApiResponse<HiringPipelineDTO>.Success(new HiringPipelineDTO(148, stages));
    }
}
