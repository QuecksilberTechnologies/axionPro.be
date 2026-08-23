// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant temporary work-mode override configuration without approval workflow.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Handlers;

#region Command

/// <summary>Creates a temporary employee work-mode override.</summary>
public sealed class CreateEmployeeWorkModeOverrideCommand(CreateEmployeeWorkModeOverrideRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public CreateEmployeeWorkModeOverrideRequestDTO DTO { get; } = dto; }
/// <summary>Updates a temporary employee work-mode override without changing approval state.</summary>
public sealed class UpdateEmployeeWorkModeOverrideCommand(UpdateEmployeeWorkModeOverrideRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public UpdateEmployeeWorkModeOverrideRequestDTO DTO { get; } = dto; }
/// <summary>Soft deletes temporary override configuration.</summary>
public sealed class DeleteEmployeeWorkModeOverrideCommand(long id) : IRequest<ApiResponse<bool>> { public long Id { get; } = id; }
/// <summary>Changes temporary override configuration state without changing approval state.</summary>
public sealed class UpdateEmployeeWorkModeOverrideStatusCommand(UpdateEmployeeWorkModeOverrideStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public UpdateEmployeeWorkModeOverrideStatusRequestDTO DTO { get; } = dto; }

#endregion

#region Query

/// <summary>Retrieves one temporary work-mode override.</summary>
public sealed class GetEmployeeWorkModeOverrideByIdQuery(long id) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public long Id { get; } = id; }
/// <summary>Retrieves filtered temporary work-mode overrides.</summary>
public sealed class GetEmployeeWorkModeOverridesQuery(EmployeeWorkModeOverrideFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>> { public EmployeeWorkModeOverrideFilterRequestDTO Filter { get; } = filter; }

#endregion

#region Handler

/// <summary>Handles temporary override creation.</summary>
public sealed class CreateEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeWorkModeOverrideCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public CreateEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(CreateEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantAsync(); Validate(request.DTO); await ValidateReferencesAsync(tenantId, request.DTO, cancellationToken);
        var entity = _mapper.Map<EmployeeWorkModeOverrideRequest>(request.DTO); entity.TenantId = tenantId; entity.ApprovalStatus = WorkModeOverrideApprovalStatus.Pending; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.AddAsync(entity, cancellationToken); await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse(stored!), AppConstants.SuccessMessages.EmployeeWorkModeOverrideCreated);
    }
    private async Task ValidateReferencesAsync(long tenantId, CreateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken cancellationToken)
    {
        if (!await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleEmployeeAsync(tenantId, dto.EmployeeId, cancellationToken) || (dto.EmployeeWorkArrangementId.HasValue && !await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleArrangementAsync(tenantId, dto.EmployeeWorkArrangementId.Value, cancellationToken)) || (dto.TenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleLocationAsync(tenantId, dto.TenantLocationId.Value, cancellationToken))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
    }
    private static void Validate(CreateEmployeeWorkModeOverrideRequestDTO dto)
    {
        if (dto is null || dto.EmployeeId <= 0 || string.IsNullOrWhiteSpace(dto.Reason) || dto.FromDate == default || dto.ToDate < dto.FromDate || dto.RequestedWorkMode is not (WorkMode.Office or WorkMode.WorkFromHome or WorkMode.Field or WorkMode.ClientSite)) throw new ValidationErrorException(dto?.RequestedWorkMode == WorkMode.Hybrid ? AppConstants.ErrorMessages.InvalidOverrideWorkMode : AppConstants.ErrorMessages.InvalidEffectiveDateRange);
    }
}

/// <summary>Handles temporary override updates without approval changes.</summary>
public sealed class UpdateEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkModeOverrideCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public UpdateEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(UpdateEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantAsync(); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); Validate(request.DTO);
        var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound);
        await ValidateReferencesAsync(tenantId, request.DTO, cancellationToken); _mapper.Map(request.DTO, entity); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse(stored!), AppConstants.SuccessMessages.EmployeeWorkModeOverrideUpdated);
    }
    private async Task ValidateReferencesAsync(long tenantId, CreateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken cancellationToken)
    {
        if (!await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleEmployeeAsync(tenantId, dto.EmployeeId, cancellationToken) || (dto.EmployeeWorkArrangementId.HasValue && !await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleArrangementAsync(tenantId, dto.EmployeeWorkArrangementId.Value, cancellationToken)) || (dto.TenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleLocationAsync(tenantId, dto.TenantLocationId.Value, cancellationToken))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
    }
    private static void Validate(CreateEmployeeWorkModeOverrideRequestDTO dto)
    {
        if (dto.EmployeeId <= 0 || string.IsNullOrWhiteSpace(dto.Reason) || dto.FromDate == default || dto.ToDate < dto.FromDate || dto.RequestedWorkMode is not (WorkMode.Office or WorkMode.WorkFromHome or WorkMode.Field or WorkMode.ClientSite)) throw new ValidationErrorException(dto.RequestedWorkMode == WorkMode.Hybrid ? AppConstants.ErrorMessages.InvalidOverrideWorkMode : AppConstants.ErrorMessages.InvalidEffectiveDateRange);
    }
}

/// <summary>Handles temporary override soft deletion.</summary>
public sealed class DeleteEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeWorkModeOverrideCommand, ApiResponse<bool>>
{
    public DeleteEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    { var (tenantId, actorId) = await ValidateTenantAsync(); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeWorkModeOverrideDeleted); }
}

/// <summary>Handles temporary override active-state changes without approval changes.</summary>
public sealed class UpdateEmployeeWorkModeOverrideStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkModeOverrideStatusCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    public UpdateEmployeeWorkModeOverrideStatusCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(UpdateEmployeeWorkModeOverrideStatusCommand request, CancellationToken cancellationToken)
    { var (tenantId, actorId) = await ValidateTenantAsync(); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken); return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse(stored!), AppConstants.SuccessMessages.EmployeeWorkModeOverrideStatusUpdated); }
}

/// <summary>Handles temporary override retrieval.</summary>
public sealed class GetEmployeeWorkModeOverrideByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkModeOverrideByIdQuery, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    public GetEmployeeWorkModeOverrideByIdQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(GetEmployeeWorkModeOverrideByIdQuery request, CancellationToken cancellationToken)
    { var (tenantId, _) = await ValidateTenantAsync(); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse(entity)); }
}

/// <summary>Handles paged temporary override retrieval.</summary>
public sealed class GetEmployeeWorkModeOverridesQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkModeOverridesQuery, ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>>
{
    public GetEmployeeWorkModeOverridesQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>> Handle(GetEmployeeWorkModeOverridesQuery request, CancellationToken cancellationToken)
    { var (tenantId, _) = await ValidateTenantAsync(); var page = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetPagedAsync(tenantId, request.Filter ?? new EmployeeWorkModeOverrideFilterRequestDTO(), cancellationToken); return Paged(page.Data.Select(TenantConfigurationResponseMapper.ToResponse).ToList(), page.PageNumber, page.PageSize, page.TotalCount, "Work mode override requests retrieved successfully."); }
}

#endregion
