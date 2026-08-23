// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles authenticated Host administration of physical Tenant device registration and lifecycle.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using static axionpro.application.Features.HostDeviceCmd.Handlers.TenantDeviceCommandValidation;

namespace axionpro.application.Features.HostDeviceCmd.Handlers;

#region Command

/// <summary>Creates a Host-managed physical Tenant device.</summary>
public sealed class CreateTenantDeviceCommand(CreateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>> { public CreateTenantDeviceRequestDTO DTO { get; } = dto; }
/// <summary>Updates a Host-managed physical Tenant device.</summary>
public sealed class UpdateTenantDeviceCommand(UpdateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>> { public UpdateTenantDeviceRequestDTO DTO { get; } = dto; }
/// <summary>Changes a Host-managed physical Tenant device active state.</summary>
public sealed class UpdateTenantDeviceStatusCommand(UpdateTenantDeviceStatusRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>> { public UpdateTenantDeviceStatusRequestDTO DTO { get; } = dto; }
/// <summary>Soft deletes a Host-managed physical Tenant device.</summary>
public sealed class DeleteTenantDeviceCommand(long id) : IRequest<ApiResponse<bool>> { public long Id { get; } = id; }

#endregion

#region Query

/// <summary>Retrieves one Host-managed physical Tenant device.</summary>
public sealed class GetTenantDeviceByIdQuery(long id) : IRequest<ApiResponse<TenantDeviceResponseDTO>> { public long Id { get; } = id; }
/// <summary>Retrieves a database-paged Host-managed physical Tenant device list.</summary>
public sealed class GetAllTenantDevicesQuery(GetTenantDeviceListRequestDTO filter) : IRequest<ApiResponse<List<TenantDeviceResponseDTO>>> { public GetTenantDeviceListRequestDTO Filter { get; } = filter; }

#endregion

#region Handler

/// <summary>Handles creation of Host-managed physical Tenant devices.</summary>
public sealed class CreateTenantDeviceCommandHandler : IRequestHandler<CreateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    #region Fields
    private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper; private readonly ICommonRequestService _commonRequestService; private readonly ILogger<CreateTenantDeviceCommandHandler> _logger;
    #endregion
    #region Constructor
    /// <summary>Initializes handler dependencies.</summary>
    public CreateTenantDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<CreateTenantDeviceCommandHandler> logger) { _unitOfWork = unitOfWork; _mapper = mapper; _commonRequestService = commonRequestService; _logger = logger; }
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(CreateTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync(); Validate(request.DTO); var dto = request.DTO;
        await ValidateReferencesAsync(_unitOfWork, dto.TenantId, dto.TenantLocationId, dto.DeviceMasterId, cancellationToken);
        await ValidateUniqueValuesAsync(_unitOfWork, dto.TenantId, dto.SerialNumber, dto.DeviceCode, dto.AssetTag, null, cancellationToken);
        var entity = _mapper.Map<TenantDevice>(dto); entity.DeviceCode = dto.DeviceCode.Trim(); entity.SerialNumber = dto.SerialNumber.Trim(); entity.AssetTag = NormalizeOptional(dto.AssetTag); entity.IsSoftDeleted = false; entity.AddedById = hostUserId; entity.AddedDateTime = DateTime.UtcNow;
        await _unitOfWork.TenantDeviceRepository.AddAsync(entity, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await _unitOfWork.TenantDeviceRepository.GetByIdAsync(entity.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Created TenantDevice {TenantDeviceId} for Tenant {TenantId}, Location {TenantLocationId}, Master {DeviceMasterId}, Serial {SerialNumber} by HostUser {HostUserId}.", entity.Id, entity.TenantId, entity.TenantLocationId, entity.DeviceMasterId, entity.SerialNumber, hostUserId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(HostDeviceResponseMapper.ToResponse(_mapper, stored), AppConstants.SuccessMessages.TenantDeviceCreated);
    }
    #endregion
}

/// <summary>Handles updates to Host-managed physical Tenant devices while preserving runtime telemetry.</summary>
public sealed class UpdateTenantDeviceCommandHandler : IRequestHandler<UpdateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper; private readonly ICommonRequestService _commonRequestService; private readonly ILogger<UpdateTenantDeviceCommandHandler> _logger;
    /// <summary>Initializes handler dependencies.</summary>
    public UpdateTenantDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<UpdateTenantDeviceCommandHandler> logger) { _unitOfWork = unitOfWork; _mapper = mapper; _commonRequestService = commonRequestService; _logger = logger; }
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(UpdateTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync(); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); Validate(request.DTO); var dto = request.DTO;
        var entity = await _unitOfWork.TenantDeviceRepository.GetForUpdateAsync(dto.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        await ValidateReferencesAsync(_unitOfWork, dto.TenantId, dto.TenantLocationId, dto.DeviceMasterId, cancellationToken); await ValidateUniqueValuesAsync(_unitOfWork, dto.TenantId, dto.SerialNumber, dto.DeviceCode, dto.AssetTag, entity.Id, cancellationToken);
        _mapper.Map(dto, entity); entity.DeviceCode = dto.DeviceCode.Trim(); entity.SerialNumber = dto.SerialNumber.Trim(); entity.AssetTag = NormalizeOptional(dto.AssetTag); entity.UpdatedById = hostUserId; entity.UpdatedDateTime = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await _unitOfWork.TenantDeviceRepository.GetByIdAsync(entity.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Updated TenantDevice {TenantDeviceId} for Tenant {TenantId}, Location {TenantLocationId}, Master {DeviceMasterId}, Serial {SerialNumber} by HostUser {HostUserId}.", entity.Id, entity.TenantId, entity.TenantLocationId, entity.DeviceMasterId, entity.SerialNumber, hostUserId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(HostDeviceResponseMapper.ToResponse(_mapper, stored), AppConstants.SuccessMessages.TenantDeviceUpdated);
    }
}

/// <summary>Handles TenantDevice active-state changes.</summary>
public sealed class UpdateTenantDeviceStatusCommandHandler : IRequestHandler<UpdateTenantDeviceStatusCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper; private readonly ICommonRequestService _commonRequestService; private readonly ILogger<UpdateTenantDeviceStatusCommandHandler> _logger;
    /// <summary>Initializes handler dependencies.</summary>
    public UpdateTenantDeviceStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<UpdateTenantDeviceStatusCommandHandler> logger) { _unitOfWork = unitOfWork; _mapper = mapper; _commonRequestService = commonRequestService; _logger = logger; }
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(UpdateTenantDeviceStatusCommand request, CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync(); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var entity = await _unitOfWork.TenantDeviceRepository.GetForUpdateAsync(request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (request.DTO.IsActive) await ValidateReferencesAsync(_unitOfWork, entity.TenantId, entity.TenantLocationId, entity.DeviceMasterId, cancellationToken);
        if (!request.DTO.IsActive && await _unitOfWork.TenantDeviceRepository.HasActiveEnrollmentsAsync(entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        entity.IsActive = request.DTO.IsActive; entity.UpdatedById = hostUserId; entity.UpdatedDateTime = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await _unitOfWork.TenantDeviceRepository.GetByIdAsync(entity.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Changed TenantDevice {TenantDeviceId} status to {IsActive} by HostUser {HostUserId}.", entity.Id, entity.IsActive, hostUserId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(HostDeviceResponseMapper.ToResponse(_mapper, stored), AppConstants.SuccessMessages.TenantDeviceStatusUpdated);
    }
}

/// <summary>Handles TenantDevice soft deletion.</summary>
public sealed class DeleteTenantDeviceCommandHandler : IRequestHandler<DeleteTenantDeviceCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork; private readonly ICommonRequestService _commonRequestService; private readonly ILogger<DeleteTenantDeviceCommandHandler> _logger;
    /// <summary>Initializes handler dependencies.</summary>
    public DeleteTenantDeviceCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<DeleteTenantDeviceCommandHandler> logger) { _unitOfWork = unitOfWork; _commonRequestService = commonRequestService; _logger = logger; }
    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync(); var entity = await _unitOfWork.TenantDeviceRepository.GetForUpdateAsync(request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (await _unitOfWork.TenantDeviceRepository.HasEnrollmentsAsync(entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = hostUserId; entity.SoftDeletedDateTime = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Soft deleted TenantDevice {TenantDeviceId} for Tenant {TenantId}, Serial {SerialNumber} by HostUser {HostUserId}.", entity.Id, entity.TenantId, entity.SerialNumber, hostUserId);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantDeviceDeleted);
    }
}

/// <summary>Handles TenantDevice retrieval by identifier.</summary>
public sealed class GetTenantDeviceByIdQueryHandler : IRequestHandler<GetTenantDeviceByIdQuery, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper; private readonly ICommonRequestService _commonRequestService;
    /// <summary>Initializes handler dependencies.</summary>
    public GetTenantDeviceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService) { _unitOfWork = unitOfWork; _mapper = mapper; _commonRequestService = commonRequestService; }
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(GetTenantDeviceByIdQuery request, CancellationToken cancellationToken)
    { await _commonRequestService.ValidateHostUserRequestAsync(); var entity = await _unitOfWork.TenantDeviceRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound); return ApiResponse<TenantDeviceResponseDTO>.Success(HostDeviceResponseMapper.ToResponse(_mapper, entity), AppConstants.SuccessMessages.TenantDeviceRetrieved); }
}

/// <summary>Handles database-paged TenantDevice retrieval.</summary>
public sealed class GetAllTenantDevicesQueryHandler : IRequestHandler<GetAllTenantDevicesQuery, ApiResponse<List<TenantDeviceResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper; private readonly ICommonRequestService _commonRequestService;
    /// <summary>Initializes handler dependencies.</summary>
    public GetAllTenantDevicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService) { _unitOfWork = unitOfWork; _mapper = mapper; _commonRequestService = commonRequestService; }
    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantDeviceResponseDTO>>> Handle(GetAllTenantDevicesQuery request, CancellationToken cancellationToken)
    { await _commonRequestService.ValidateHostUserRequestAsync(); var page = await _unitOfWork.TenantDeviceRepository.GetPagedAsync(request.Filter ?? new GetTenantDeviceListRequestDTO(), cancellationToken); return ApiResponse<List<TenantDeviceResponseDTO>>.SuccessPaginated(page.Data.Select(x => HostDeviceResponseMapper.ToResponse(_mapper, x)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, page.TotalPages, AppConstants.SuccessMessages.TenantDeviceRetrieved); }
}

internal static class TenantDeviceCommandValidation
{
    internal static void Validate(TenantDeviceRequestDTO? dto)
    {
        if (dto is null || dto.TenantId <= 0 || dto.TenantLocationId <= 0 || dto.DeviceMasterId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceCode) || string.IsNullOrWhiteSpace(dto.SerialNumber) || (dto.CommunicationType.HasValue && !Enum.IsDefined(dto.CommunicationType.Value))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }

    internal static async Task ValidateReferencesAsync(IUnitOfWork unitOfWork, long tenantId, long tenantLocationId, long deviceMasterId, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.TenantDeviceRepository.IsEligibleTenantAsync(tenantId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceManagementTenant);
        if (!await unitOfWork.TenantDeviceRepository.IsActiveTenantLocationAsync(tenantLocationId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceManagementTenantLocation);
        if (!await unitOfWork.TenantDeviceRepository.TenantLocationBelongsToTenantAsync(tenantId, tenantLocationId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.TenantLocationDoesNotBelongToTenant);
        if (!await unitOfWork.TenantDeviceRepository.IsEligibleDeviceMasterAsync(deviceMasterId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceMaster);
    }

    internal static async Task ValidateUniqueValuesAsync(IUnitOfWork unitOfWork, long tenantId, string serialNumber, string deviceCode, string? assetTag, long? excludeId, CancellationToken cancellationToken)
    {
        if (await unitOfWork.TenantDeviceRepository.SerialNumberExistsAsync(serialNumber, excludeId, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantDeviceSerialNumber);
        if (await unitOfWork.TenantDeviceRepository.DeviceCodeExistsAsync(tenantId, deviceCode, excludeId, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantDeviceCode);
        if (await unitOfWork.TenantDeviceRepository.AssetTagExistsAsync(tenantId, assetTag, excludeId, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantDeviceAssetTag);
    }

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

#endregion
