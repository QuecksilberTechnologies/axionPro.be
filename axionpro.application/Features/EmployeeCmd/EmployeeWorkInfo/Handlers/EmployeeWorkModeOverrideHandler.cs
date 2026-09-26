// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant temporary work-mode override configuration without approval workflow.
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
using System.Globalization;

namespace axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo.Handlers;

#region Command

/// <summary>Creates a temporary employee work-mode override.</summary>
public sealed class CreateEmployeeWorkModeOverrideCommand(CreateEmployeeWorkModeOverrideRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public CreateEmployeeWorkModeOverrideRequestDTO DTO { get; } = dto; }
/// <summary>Updates a temporary employee work-mode override without changing approval state.</summary>
public sealed class UpdateEmployeeWorkModeOverrideCommand(UpdateEmployeeWorkModeOverrideRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public UpdateEmployeeWorkModeOverrideRequestDTO DTO { get; } = dto; }
/// <summary>Soft deletes temporary override configuration.</summary>
public sealed class DeleteEmployeeWorkModeOverrideCommand(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<bool>>
{
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Changes temporary override configuration state without changing approval state.</summary>
public sealed class UpdateEmployeeWorkModeOverrideStatusCommand(UpdateEmployeeWorkModeOverrideStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> { public UpdateEmployeeWorkModeOverrideStatusRequestDTO DTO { get; } = dto; }
/// <summary>Approves or rejects a pending temporary override.</summary>
public sealed class DecideEmployeeWorkModeOverrideCommand(EmployeeWorkModeOverrideDecisionRequestDTO dto, bool approve) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    public EmployeeWorkModeOverrideDecisionRequestDTO DTO { get; } = dto;
    public bool Approve { get; } = approve;
}

#endregion

#region Query

/// <summary>Retrieves one temporary work-mode override.</summary>
public sealed class GetEmployeeWorkModeOverrideByIdQuery(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Retrieves filtered temporary work-mode overrides.</summary>
public sealed class GetEmployeeWorkModeOverridesQuery(EmployeeWorkModeOverrideFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>> { public EmployeeWorkModeOverrideFilterRequestDTO Filter { get; } = filter; }

#endregion

/// <summary>Applies the shared employee, arrangement, location, policy, and overlap rules.</summary>
internal static class EmployeeWorkModeOverrideValidator
{
    public static async Task ValidateAsync(
        IUnitOfWork unitOfWork,
        long tenantId,
        long employeeId,
        CreateEmployeeWorkModeOverrideRequestDTO dto,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        if (!await unitOfWork.EmployeeWorkModeOverrideRequestRepository.IsEligibleEmployeeAsync(tenantId, employeeId, cancellationToken))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkArrangementEmployeeNotFound);
        }

        EmployeeWorkArrangement? arrangement = null;
        if (dto.EmployeeWorkArrangementId.HasValue)
        {
            arrangement = await unitOfWork.EmployeeWorkArrangementRepository.GetByIdAsync(
                tenantId,
                dto.EmployeeWorkArrangementId.Value,
                cancellationToken);
            if (arrangement is null || !arrangement.IsActive)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
            }

            if (arrangement.EmployeeId != employeeId)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideArrangementEmployeeMismatch);
            }

            if (dto.FromDate < arrangement.EffectiveFrom
                || (arrangement.EffectiveTo.HasValue && dto.ToDate > arrangement.EffectiveTo.Value))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideArrangementDateMismatch);
            }
        }

        if (dto.RequestedWorkMode == WorkMode.WorkFromHome && dto.TenantLocationId.HasValue)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideLocationNotAllowed);
        }

        if (dto.RequestedWorkMode is WorkMode.Office or WorkMode.Field or WorkMode.ClientSite
            && !dto.TenantLocationId.HasValue)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideLocationRequired);
        }

        if (dto.TenantLocationId.HasValue)
        {
            var location = await unitOfWork.EmployeeWorkArrangementRepository.GetLocationForValidationAsync(
                tenantId,
                dto.TenantLocationId.Value,
                cancellationToken);
            if (location is null || !location.IsActive)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.WorkArrangementLocationNotFound);
            }

            if (!EmployeeWorkConfigurationRules.IsLocationTypeCompatible(dto.RequestedWorkMode, (TenantLocationType)location.LocationType))
            {
                throw new ValidationErrorException(string.Format(
                    CultureInfo.InvariantCulture,
                    AppConstants.ErrorMessages.WorkArrangementLocationTypeDetails,
                    (TenantLocationType)location.LocationType,
                    dto.RequestedWorkMode));
            }

            if (!await unitOfWork.EmployeeWorkArrangementRepository.HasCoveringAttendanceLocationAssignmentAsync(
                tenantId,
                employeeId,
                dto.TenantLocationId.Value,
                dto.FromDate,
                dto.ToDate,
                cancellationToken))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideLocationAssignmentRequired);
            }
        }

        if (arrangement?.PolicyVersionId is long policyVersionId)
        {
            var configuration = await unitOfWork.EmployeeWorkArrangementRepository.GetAttendanceConfigurationAsync(
                tenantId,
                policyVersionId,
                cancellationToken)
                ?? throw new ValidationErrorException(AppConstants.ErrorMessages.AttendancePolicyConfigurationRequired);
            if (!EmployeeWorkConfigurationRules.IsAllowedByAttendancePolicy(dto.RequestedWorkMode, dto.TenantLocationId, configuration))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverridePolicyMismatch);
            }
        }

        if (dto.IsActive)
        {
            var conflict = await unitOfWork.EmployeeWorkModeOverrideRequestRepository.GetOverlappingOverrideAsync(
                tenantId,
                employeeId,
                dto.FromDate,
                dto.ToDate,
                excludeId,
                cancellationToken);
            if (conflict is not null)
            {
                throw new ConflictException(string.Format(
                    CultureInfo.InvariantCulture,
                    AppConstants.ErrorMessages.WorkModeOverrideDateOverlap,
                    dto.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    dto.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    conflict.Id,
                    conflict.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    conflict.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
            }
        }
    }
}

/// <summary>Maps an override while preserving the public tenant-salted Employee identifier.</summary>
internal static class EmployeeWorkModeOverrideResponseMapper
{
    public static EmployeeWorkModeOverrideResponseDTO Map(IMapper mapper, EmployeeWorkModeOverrideRequest entity, Func<long, string> encodeEmployeeId)
    {
        var response = mapper.Map<EmployeeWorkModeOverrideResponseDTO>(entity);
        response.EmployeeId = encodeEmployeeId(entity.EmployeeId);
        return response;
    }
}

#region Handler

/// <summary>Handles temporary override creation.</summary>
public sealed class CreateEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeWorkModeOverrideCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public CreateEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(CreateEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    {
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO); await EmployeeWorkModeOverrideValidator.ValidateAsync(UnitOfWork, tenantId, employeeId, request.DTO, null, cancellationToken);
        var entity = _mapper.Map<EmployeeWorkModeOverrideRequest>(request.DTO); entity.EmployeeId = employeeId; entity.TenantId = tenantId; entity.ApprovalStatus = (short)WorkModeOverrideApprovalStatus.Pending; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.AddAsync(entity, cancellationToken); await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        var validation = await ValidateTenantDataAccessContextAsync();
        return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(EmployeeWorkModeOverrideResponseMapper.Map(_mapper, stored!, id => EncodeEmployeeId(id, validation)), AppConstants.SuccessMessages.EmployeeWorkModeOverrideCreated);
    }
    private static void Validate(CreateEmployeeWorkModeOverrideRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        if (string.IsNullOrWhiteSpace(dto.Reason) || dto.Reason.Trim().Length > 500)
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideReasonRequired);
        if (dto.FromDate == default || dto.ToDate < dto.FromDate)
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidEffectiveDateRange);
        if (dto.RequestedWorkMode is not (WorkMode.Office or WorkMode.WorkFromHome or WorkMode.Field or WorkMode.ClientSite))
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidOverrideWorkMode);
    }
}

/// <summary>Handles temporary override updates without approval changes.</summary>
public sealed class UpdateEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkModeOverrideCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public UpdateEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(UpdateEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO);
        var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound);
        if (entity.EmployeeId != employeeId) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        if (entity.ApprovalStatus != (short)WorkModeOverrideApprovalStatus.Pending) throw new ConflictException(AppConstants.ErrorMessages.WorkModeOverrideDecisionFinal);
        await EmployeeWorkModeOverrideValidator.ValidateAsync(UnitOfWork, tenantId, employeeId, request.DTO, entity.Id, cancellationToken); _mapper.Map(request.DTO, entity); entity.EmployeeId = employeeId; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        var validation = await ValidateTenantDataAccessContextAsync();
        return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(EmployeeWorkModeOverrideResponseMapper.Map(_mapper, stored!, id => EncodeEmployeeId(id, validation)), AppConstants.SuccessMessages.EmployeeWorkModeOverrideUpdated);
    }
    private static void Validate(CreateEmployeeWorkModeOverrideRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        if (string.IsNullOrWhiteSpace(dto.Reason) || dto.Reason.Trim().Length > 500)
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideReasonRequired);
        if (dto.FromDate == default || dto.ToDate < dto.FromDate)
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidEffectiveDateRange);
        if (dto.RequestedWorkMode is not (WorkMode.Office or WorkMode.WorkFromHome or WorkMode.Field or WorkMode.ClientSite))
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidOverrideWorkMode);
    }
}

/// <summary>Handles temporary override soft deletion.</summary>
public sealed class DeleteEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeWorkModeOverrideCommand, ApiResponse<bool>>
{
    public DeleteEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    { var validation = await ValidateTenantDataAccessContextAsync(); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(validation.TenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); await EnsureEmployeeDataAccessAsync(validation, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, cancellationToken); entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = validation.LoggedInEmployeeId; entity.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeWorkModeOverrideDeleted); }
}

/// <summary>Handles temporary override active-state changes without approval changes.</summary>
public sealed class UpdateEmployeeWorkModeOverrideStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkModeOverrideStatusCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public UpdateEmployeeWorkModeOverrideStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(UpdateEmployeeWorkModeOverrideStatusCommand request, CancellationToken cancellationToken)
    { var validation = await ValidateTenantDataAccessContextAsync(); var tenantId = validation.TenantId; if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); await EnsureEmployeeDataAccessAsync(validation, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, cancellationToken); if (request.DTO.IsActive) await EmployeeWorkModeOverrideValidator.ValidateAsync(UnitOfWork, tenantId, entity.EmployeeId, ToValidationDto(entity, true), entity.Id, cancellationToken); entity.IsActive = request.DTO.IsActive; entity.UpdatedById = validation.LoggedInEmployeeId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); var stored = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken); return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(EmployeeWorkModeOverrideResponseMapper.Map(_mapper, stored!, id => EncodeEmployeeId(id, validation)), AppConstants.SuccessMessages.EmployeeWorkModeOverrideStatusUpdated); }

    internal static CreateEmployeeWorkModeOverrideRequestDTO ToValidationDto(EmployeeWorkModeOverrideRequest entity, bool isActive) => new()
    {
        EmployeeWorkArrangementId = entity.EmployeeWorkArrangementId,
        RequestedWorkMode = (WorkMode)entity.RequestedWorkMode,
        FromDate = entity.FromDate,
        ToDate = entity.ToDate,
        TenantLocationId = entity.TenantLocationId,
        Reason = entity.Reason,
        IsActive = isActive
    };
}

/// <summary>Handles the mandatory approval decision for a pending override.</summary>
public sealed class DecideEmployeeWorkModeOverrideCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DecideEmployeeWorkModeOverrideCommand, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public DecideEmployeeWorkModeOverrideCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(DecideEmployeeWorkModeOverrideCommand request, CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        if (!request.Approve && (string.IsNullOrWhiteSpace(request.DTO.Remark) || request.DTO.Remark.Trim().Length > 500))
            throw new ValidationErrorException(AppConstants.ErrorMessages.WorkModeOverrideRejectedRemarkRequired);
        var expectedOperation = request.Approve ? "Approve" : "Reject";
        var operationName = await CommonRequestService.GetActiveOperationNameAsync(request.DTO.OperationId);
        if (!string.Equals(operationName, expectedOperation, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"The {expectedOperation} operation permission is required for this decision.");
        }
        var validation = await ValidateTenantDataAccessContextAsync();
        var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetForUpdateAsync(validation.TenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound);
        await EnsureEmployeeDataAccessAsync(validation, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, cancellationToken);
        if (entity.ApprovalStatus != (short)WorkModeOverrideApprovalStatus.Pending) throw new ConflictException(AppConstants.ErrorMessages.WorkModeOverrideDecisionFinal);
        if (request.Approve && !entity.IsActive) throw new ConflictException(AppConstants.ErrorMessages.WorkModeOverrideInactiveApproval);
        if (request.Approve) await EmployeeWorkModeOverrideValidator.ValidateAsync(UnitOfWork, validation.TenantId, entity.EmployeeId, UpdateEmployeeWorkModeOverrideStatusCommandHandler.ToValidationDto(entity, true), entity.Id, cancellationToken);
        entity.ApprovalStatus = (short)(request.Approve ? WorkModeOverrideApprovalStatus.Approved : WorkModeOverrideApprovalStatus.Rejected);
        entity.ApprovedById = request.Approve ? validation.LoggedInEmployeeId : null;
        entity.ApprovedDateTime = request.Approve ? DateTime.UtcNow : null;
        entity.ApprovalRemark = request.Approve ? request.DTO.Remark?.Trim() : null;
        entity.RejectedById = request.Approve ? null : validation.LoggedInEmployeeId;
        entity.RejectedDateTime = request.Approve ? null : DateTime.UtcNow;
        entity.RejectionRemark = request.Approve ? null : request.DTO.Remark!.Trim();
        entity.UpdatedById = validation.LoggedInEmployeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = (await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(validation.TenantId, entity.Id, cancellationToken))!;
        return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(EmployeeWorkModeOverrideResponseMapper.Map(_mapper, stored, id => EncodeEmployeeId(id, validation)), request.Approve ? AppConstants.SuccessMessages.EmployeeWorkModeOverrideApproved : AppConstants.SuccessMessages.EmployeeWorkModeOverrideRejected);
    }
}

/// <summary>Handles temporary override retrieval.</summary>
public sealed class GetEmployeeWorkModeOverrideByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkModeOverrideByIdQuery, ApiResponse<EmployeeWorkModeOverrideResponseDTO>>
{
    private readonly IMapper _mapper;
    public GetEmployeeWorkModeOverrideByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkModeOverrideResponseDTO>> Handle(GetEmployeeWorkModeOverrideByIdQuery request, CancellationToken cancellationToken)
    { var validation = await ValidateTenantDataAccessContextAsync(); var entity = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetByIdAsync(validation.TenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkModeOverrideNotFound); await EnsureEmployeeDataAccessAsync(validation, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, cancellationToken); return ApiResponse<EmployeeWorkModeOverrideResponseDTO>.Success(EmployeeWorkModeOverrideResponseMapper.Map(_mapper, entity, id => EncodeEmployeeId(id, validation))); }
}

/// <summary>Handles paged temporary override retrieval.</summary>
public sealed class GetEmployeeWorkModeOverridesQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkModeOverridesQuery, ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>>
{
    private readonly IMapper _mapper;
    public GetEmployeeWorkModeOverridesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService idEncoderService) : base(unitOfWork, commonRequestService, logger, idEncoderService) => _mapper = mapper;
    public async Task<ApiResponse<List<EmployeeWorkModeOverrideResponseDTO>>> Handle(GetEmployeeWorkModeOverridesQuery request, CancellationToken cancellationToken)
    { var filter = request.Filter ?? new EmployeeWorkModeOverrideFilterRequestDTO(); var validation = await ValidateTenantDataAccessContextAsync(); long? employeeId = null; if (!string.IsNullOrWhiteSpace(filter.EmployeeId)) { var context = await ValidateTenantAndDecodeOptionalEmployeeIdAsync(filter.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, cancellationToken); employeeId = context.EmployeeId; } filter.ResolvedEmployeeId = employeeId; var page = await UnitOfWork.EmployeeWorkModeOverrideRequestRepository.GetPagedAsync(validation.TenantId, filter, validation.LoggedInEmployeeId, validation.RoleTypeId, cancellationToken); return Paged(page.Data.Select(entity => EmployeeWorkModeOverrideResponseMapper.Map(_mapper, entity, id => EncodeEmployeeId(id, validation))).ToList(), page.PageNumber, page.PageSize, page.TotalCount, "Work mode override requests retrieved successfully."); }
}

#endregion
