// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant employee work arrangements and lifecycle dependencies.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo.Handlers;

#region Command
/// <summary>Creates an employee work arrangement.</summary>
public sealed class CreateEmployeeWorkArrangementCommand(CreateEmployeeWorkArrangementRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    /// <summary>Gets arrangement values.</summary>
    public CreateEmployeeWorkArrangementRequestDTO DTO { get; } = dto;
}
/// <summary>Updates an employee work arrangement.</summary>
public sealed class UpdateEmployeeWorkArrangementCommand(UpdateEmployeeWorkArrangementRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    /// <summary>Gets arrangement values.</summary>
    public UpdateEmployeeWorkArrangementRequestDTO DTO { get; } = dto;
}
/// <summary>Soft deletes an employee work arrangement.</summary>
public sealed class DeleteEmployeeWorkArrangementCommand(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<bool>>
{
    /// <summary>Gets identifier.</summary>
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Changes employee work arrangement active state.</summary>
public sealed class UpdateEmployeeWorkArrangementStatusCommand(UpdateEmployeeWorkArrangementStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    /// <summary>Gets status values.</summary>
    public UpdateEmployeeWorkArrangementStatusRequestDTO DTO { get; } = dto;
}
#endregion
#region Query
/// <summary>Retrieves one employee work arrangement.</summary>
public sealed class GetEmployeeWorkArrangementByIdQuery(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    /// <summary>Gets identifier.</summary>
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Retrieves filtered employee work arrangements.</summary>
public sealed class GetEmployeeWorkArrangementsQuery(EmployeeWorkArrangementFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeWorkArrangementResponseDTO>>>
{
    /// <summary>Gets filters.</summary>
    public EmployeeWorkArrangementFilterRequestDTO Filter { get; } = filter;
}
#endregion
#region Handler
/// <summary>Handles employee work arrangement creation.</summary>
public sealed class CreateEmployeeWorkArrangementCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeWorkArrangementCommand, ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes handler.</summary>
    public CreateEmployeeWorkArrangementCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = m;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeWorkArrangementResponseDTO>> Handle(CreateEmployeeWorkArrangementCommand request, CancellationToken ct)
    {
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO); await ValidateRefs(tenantId, employeeId, request.DTO, null, ct);
        var e = _mapper.Map<EmployeeWorkArrangement>(request.DTO); e.EmployeeId = employeeId; e.TenantId = tenantId; e.IsSoftDeleted = false; e.AddedById = actorId; e.AddedDateTime = DateTime.UtcNow; await UnitOfWork.EmployeeWorkArrangementRepository.AddAsync(e, ct); await UnitOfWork.SaveChangesAsync(ct);
        Logger.LogInformation("Work arrangement {WorkArrangementId} created for Employee {EmployeeId} and Tenant {TenantId}.", e.Id, e.EmployeeId, tenantId);
        return ApiResponse<EmployeeWorkArrangementResponseDTO>.Success(_mapper.Map<EmployeeWorkArrangementResponseDTO>((await UnitOfWork.EmployeeWorkArrangementRepository.GetByIdAsync(tenantId, e.Id, ct))!), AppConstants.SuccessMessages.EmployeeWorkArrangementCreated);
    }
    #endregion
    private async Task ValidateRefs(long tenantId, long employeeId, CreateEmployeeWorkArrangementRequestDTO dto, long? excludeId, CancellationToken ct)
    {
        if (!await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleEmployeeAsync(tenantId, employeeId, ct) || !await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleAttendancePolicyAsync(tenantId, dto.AttendancePolicyId, ct) || (dto.PrimaryTenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleLocationAsync(tenantId, dto.PrimaryTenantLocationId.Value, ct))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (dto.IsActive && await UnitOfWork.EmployeeWorkArrangementRepository.CurrentArrangementExistsAsync(tenantId, employeeId, excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasCurrentWorkArrangement);
    }
    private static void Validate(CreateEmployeeWorkArrangementRequestDTO dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.EmployeeId) || dto.AttendancePolicyId <= 0 || dto.EffectiveFrom == default || (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom) || !Enum.IsDefined(dto.WorkMode) || (dto.HybridType.HasValue && !Enum.IsDefined(dto.HybridType.Value)) || (dto.WorkMode == WorkMode.Hybrid && !dto.HybridType.HasValue) || (dto.WorkMode != WorkMode.Hybrid && dto.HybridType.HasValue) || !Within(dto.MinimumOfficeDaysPerWeek, 7) || !Within(dto.MinimumOfficeDaysPerMonth, 31) || !Within(dto.MaximumWFHDaysPerMonth, 31)) throw new ValidationErrorException(dto?.HybridType.HasValue == true || dto?.WorkMode == WorkMode.Hybrid ? AppConstants.ErrorMessages.InvalidHybridConfiguration : AppConstants.ErrorMessages.InvalidEffectiveDateRange);
    }
    private static bool Within(short? value, short maximum) => !value.HasValue || (value.Value >= 0 && value.Value <= maximum);
}

/// <summary>Handles employee work arrangement updates.</summary>
public sealed class UpdateEmployeeWorkArrangementCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkArrangementCommand, ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes handler.</summary>
    public UpdateEmployeeWorkArrangementCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = m;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeWorkArrangementResponseDTO>> Handle(UpdateEmployeeWorkArrangementCommand request, CancellationToken ct)
    {
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO);
        var e = await UnitOfWork.EmployeeWorkArrangementRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkArrangementNotFound);
        if (e.EmployeeId != employeeId) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        if (!request.DTO.IsActive && e.IsActive && await UnitOfWork.EmployeeWorkArrangementRepository.HasLiveActiveDependenciesAsync(tenantId, e.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeWorkArrangementInUse);
        await ValidateRefs(tenantId, employeeId, request.DTO, e.Id, ct);
        _mapper.Map(request.DTO, e); e.EmployeeId = employeeId; e.UpdatedById = actorId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct);
        return ApiResponse<EmployeeWorkArrangementResponseDTO>.Success(_mapper.Map<EmployeeWorkArrangementResponseDTO>((await UnitOfWork.EmployeeWorkArrangementRepository.GetByIdAsync(tenantId, e.Id, ct))!), AppConstants.SuccessMessages.EmployeeWorkArrangementUpdated);
    }
    #endregion
    private async Task ValidateRefs(long tenantId, long employeeId, CreateEmployeeWorkArrangementRequestDTO dto, long? excludeId, CancellationToken ct)
    {
        if (!await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleEmployeeAsync(tenantId, employeeId, ct) || !await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleAttendancePolicyAsync(tenantId, dto.AttendancePolicyId, ct) || (dto.PrimaryTenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkArrangementRepository.IsEligibleLocationAsync(tenantId, dto.PrimaryTenantLocationId.Value, ct))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (dto.IsActive && await UnitOfWork.EmployeeWorkArrangementRepository.CurrentArrangementExistsAsync(tenantId, employeeId, excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasCurrentWorkArrangement);
    }
    private static void Validate(CreateEmployeeWorkArrangementRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeId) || dto.AttendancePolicyId <= 0 || dto.EffectiveFrom == default || (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom) || !Enum.IsDefined(dto.WorkMode) || (dto.HybridType.HasValue && !Enum.IsDefined(dto.HybridType.Value)) || (dto.WorkMode == WorkMode.Hybrid && !dto.HybridType.HasValue) || (dto.WorkMode != WorkMode.Hybrid && dto.HybridType.HasValue) || !Within(dto.MinimumOfficeDaysPerWeek, 7) || !Within(dto.MinimumOfficeDaysPerMonth, 31) || !Within(dto.MaximumWFHDaysPerMonth, 31)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidHybridConfiguration);
    }
    private static bool Within(short? value, short maximum) => !value.HasValue || (value.Value >= 0 && value.Value <= maximum);
}

/// <summary>Handles safe employee work arrangement soft deletion.</summary>
public sealed class DeleteEmployeeWorkArrangementCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeWorkArrangementCommand, ApiResponse<bool>>
{
    /// <summary>Initializes handler.</summary>
    public DeleteEmployeeWorkArrangementCommandHandler(IUnitOfWork u, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) { }
    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeWorkArrangementCommand request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var tenantId = validation.TenantId; var e = await UnitOfWork.EmployeeWorkArrangementRepository.GetForUpdateAsync(tenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkArrangementNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); if (await UnitOfWork.EmployeeWorkArrangementRepository.HasAnyDependenciesAsync(tenantId, e.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeWorkArrangementInUse); e.IsSoftDeleted = true; e.IsActive = false; e.SoftDeletedById = validation.LoggedInEmployeeId; e.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeWorkArrangementDeleted); }
}

/// <summary>Handles employee work arrangement status changes.</summary>
public sealed class UpdateEmployeeWorkArrangementStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkArrangementStatusCommand, ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public UpdateEmployeeWorkArrangementStatusCommandHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeWorkArrangementResponseDTO>> Handle(UpdateEmployeeWorkArrangementStatusCommand request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var tenantId = validation.TenantId; if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var e = await UnitOfWork.EmployeeWorkArrangementRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkArrangementNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); if (!request.DTO.IsActive && e.IsActive && await UnitOfWork.EmployeeWorkArrangementRepository.HasLiveActiveDependenciesAsync(tenantId, e.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeWorkArrangementInUse); if (request.DTO.IsActive && await UnitOfWork.EmployeeWorkArrangementRepository.CurrentArrangementExistsAsync(tenantId, e.EmployeeId, e.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasCurrentWorkArrangement); e.IsActive = request.DTO.IsActive; e.UpdatedById = validation.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<EmployeeWorkArrangementResponseDTO>.Success(_mapper.Map<EmployeeWorkArrangementResponseDTO>((await UnitOfWork.EmployeeWorkArrangementRepository.GetByIdAsync(tenantId, e.Id, ct))!), AppConstants.SuccessMessages.EmployeeWorkArrangementStatusUpdated); }
}

/// <summary>Handles employee work arrangement retrieval.</summary>
public sealed class GetEmployeeWorkArrangementByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkArrangementByIdQuery, ApiResponse<EmployeeWorkArrangementResponseDTO>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public GetEmployeeWorkArrangementByIdQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeWorkArrangementResponseDTO>> Handle(GetEmployeeWorkArrangementByIdQuery request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var e = await UnitOfWork.EmployeeWorkArrangementRepository.GetByIdAsync(validation.TenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkArrangementNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); return ApiResponse<EmployeeWorkArrangementResponseDTO>.Success(_mapper.Map<EmployeeWorkArrangementResponseDTO>(e)); }
}

/// <summary>Handles filtered employee work arrangement retrieval.</summary>
public sealed class GetEmployeeWorkArrangementsQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkArrangementsQuery, ApiResponse<List<EmployeeWorkArrangementResponseDTO>>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public GetEmployeeWorkArrangementsQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<List<EmployeeWorkArrangementResponseDTO>>> Handle(GetEmployeeWorkArrangementsQuery request, CancellationToken ct) { var filter = request.Filter ?? new EmployeeWorkArrangementFilterRequestDTO(); var validation = await ValidateTenantDataAccessContextAsync(); long? employeeId = null; if (!string.IsNullOrWhiteSpace(filter.EmployeeId)) { var context = await ValidateTenantAndDecodeOptionalEmployeeIdAsync(filter.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); employeeId = context.EmployeeId; } filter.ResolvedEmployeeId = employeeId; var p = await UnitOfWork.EmployeeWorkArrangementRepository.GetPagedAsync(validation.TenantId, filter, validation.LoggedInEmployeeId, validation.RoleTypeId, ct); return Paged(p.Data.Select(entity => _mapper.Map<EmployeeWorkArrangementResponseDTO>(entity)).ToList(), p.PageNumber, p.PageSize, p.TotalCount, "Employee work arrangements retrieved successfully."); }
}
#endregion
