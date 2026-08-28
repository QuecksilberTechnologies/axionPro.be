// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles Host and Tenant administration of physical Tenant devices.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using static axionpro.application.Features.HostDeviceCmd.Handlers.TenantDeviceCommandValidation;

namespace axionpro.application.Features.HostDeviceCmd.Handlers;

#region Command

/// <summary>Creates a physical Tenant device.</summary>
public sealed class CreateTenantDeviceCommand(CreateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public CreateTenantDeviceRequestDTO DTO { get; } = dto;
}

/// <summary>Updates a physical Tenant device.</summary>
public sealed class UpdateTenantDeviceCommand(UpdateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public UpdateTenantDeviceRequestDTO DTO { get; } = dto;
}

/// <summary>Changes a physical Tenant device active state.</summary>
public sealed class UpdateTenantDeviceStatusCommand(UpdateTenantDeviceStatusRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public UpdateTenantDeviceStatusRequestDTO DTO { get; } = dto;
}

/// <summary>Soft deletes a physical Tenant device.</summary>
public sealed class DeleteTenantDeviceCommand(long id, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<bool>>
{
    public long Id { get; } = id;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

#endregion

#region Query

/// <summary>Retrieves one physical Tenant device.</summary>
public sealed class GetTenantDeviceByIdQuery(long id, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public long Id { get; } = id;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

/// <summary>Retrieves a database-paged physical Tenant device list.</summary>
public sealed class GetAllTenantDevicesQuery(GetTenantDeviceListRequestDTO filter) : IRequest<ApiResponse<List<TenantDeviceResponseDTO>>>
{
    public GetTenantDeviceListRequestDTO Filter { get; } = filter;
}

#endregion

#region Handler

/// <summary>Represents the trusted Tenant scope and identifier-protection key for a TenantDevice request.</summary>
public sealed record TenantDeviceAccessScope(long TenantId, long ActorId, string TenantEncryptionKey);

/// <summary>
/// Resolves the authoritative Tenant scope for TenantDevice endpoints using the
/// existing Host and Tenant runtime permission flows.
/// </summary>
public abstract class TenantDeviceAccessHandlerBase : TenantConfigurationHandlerBase
{
    private readonly IIdEncoderService _idEncoderService;

    protected TenantDeviceAccessHandlerBase(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService,
        ILogger<TenantConfigurationHandlerBase> logger)
        : base(unitOfWork, commonRequestService, logger)
    {
        _idEncoderService = idEncoderService;
    }

    /// <summary>Validates the principal and resolves its authoritative Tenant, audit actor, and trusted encoding key.</summary>
    protected async Task<TenantDeviceAccessScope> ResolveTenantScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        return principal.UserType switch
        {
            LoginUserType.Host => await ResolveHostTenantScopeAsync(accessRequest, cancellationToken),
            LoginUserType.TenantEmployee => await ResolveTenantEmployeeScopeAsync(accessRequest, cancellationToken),
            _ => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized)
        };
    }

    /// <summary>Maps a device response and protects the Tenant identifier with the current trusted key.</summary>
    protected TenantDeviceResponseDTO MapResponse(IMapper mapper, TenantDevice entity, TenantDeviceAccessScope scope)
    {
        var response = mapper.Map<TenantDeviceResponseDTO>(entity);
        response.TenantId = HostTenantIdentifierProtector.Encrypt(
            entity.TenantId,
            scope.TenantEncryptionKey,
            _idEncoderService);
        return response;
    }

    private async Task<TenantDeviceAccessScope> ResolveHostTenantScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            CommonRequestService,
            UnitOfWork.StoreProcedureRepository,
            accessRequest.ModuleId,
            accessRequest.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            accessRequest.TenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);

        return new TenantDeviceAccessScope(tenantId, hostContext.HostUserId, hostContext.TenantEncryptionKey);
    }

    private async Task<TenantDeviceAccessScope> ResolveTenantEmployeeScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        var tenantValidation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!tenantValidation.Success || string.IsNullOrWhiteSpace(tenantValidation.Claims?.TenantEncriptionKey))
        {
            throw new UnauthorizedAccessException(tenantValidation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var (tenantId, actorId) = await ValidateTenantPermissionAsync(accessRequest, cancellationToken);
        if (tenantId != tenantValidation.TenantId)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        return new TenantDeviceAccessScope(tenantId, actorId, tenantValidation.Claims.TenantEncriptionKey);
    }
}

/// <summary>Handles creation of physical Tenant devices.</summary>
public sealed class CreateTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<CreateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTenantDeviceCommandHandler> _logger;

    /// <summary>Initializes handler dependencies.</summary>
    public CreateTenantDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger, ILogger<CreateTenantDeviceCommandHandler> logger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(CreateTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        Validate(request.DTO);
        await ValidateReferencesAsync(UnitOfWork, scope.TenantId, request.DTO.TenantLocationId, request.DTO.DeviceMasterId, cancellationToken);
        await ValidateUniqueValuesAsync(UnitOfWork, scope.TenantId, request.DTO.SerialNumber, request.DTO.DeviceCode, request.DTO.AssetTag, null, cancellationToken);

        var entity = _mapper.Map<TenantDevice>(request.DTO);
        entity.TenantId = scope.TenantId;
        entity.DeviceCode = request.DTO.DeviceCode.Trim();
        entity.SerialNumber = request.DTO.SerialNumber.Trim();
        entity.AssetTag = NormalizeOptional(request.DTO.AssetTag);
        entity.IsSoftDeleted = false;
        entity.AddedById = scope.ActorId;
        entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.TenantDeviceRepository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Created TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, entity.TenantId, scope.ActorId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceCreated);
    }
}

/// <summary>Handles updates to physical Tenant devices while preserving runtime telemetry and Tenant ownership.</summary>
public sealed class UpdateTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<UpdateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTenantDeviceCommandHandler> _logger;

    /// <summary>Initializes handler dependencies.</summary>
    public UpdateTenantDeviceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger, ILogger<UpdateTenantDeviceCommandHandler> logger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(UpdateTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        Validate(request.DTO);
        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, request.DTO.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        await ValidateReferencesAsync(UnitOfWork, scope.TenantId, request.DTO.TenantLocationId, request.DTO.DeviceMasterId, cancellationToken);
        await ValidateUniqueValuesAsync(UnitOfWork, scope.TenantId, request.DTO.SerialNumber, request.DTO.DeviceCode, request.DTO.AssetTag, entity.Id, cancellationToken);

        _mapper.Map(request.DTO, entity);
        entity.DeviceCode = request.DTO.DeviceCode.Trim();
        entity.SerialNumber = request.DTO.SerialNumber.Trim();
        entity.AssetTag = NormalizeOptional(request.DTO.AssetTag);
        entity.UpdatedById = scope.ActorId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Updated TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, entity.TenantId, scope.ActorId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceUpdated);
    }
}

/// <summary>Handles TenantDevice active-state changes.</summary>
public sealed class UpdateTenantDeviceStatusCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<UpdateTenantDeviceStatusCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTenantDeviceStatusCommandHandler> _logger;

    /// <summary>Initializes handler dependencies.</summary>
    public UpdateTenantDeviceStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger, ILogger<UpdateTenantDeviceStatusCommandHandler> logger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(UpdateTenantDeviceStatusCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, request.DTO.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (request.DTO.IsActive)
        {
            await ValidateReferencesAsync(UnitOfWork, scope.TenantId, entity.TenantLocationId, entity.DeviceMasterId, cancellationToken);
        }

        if (!request.DTO.IsActive && await UnitOfWork.TenantDeviceRepository.HasActiveEnrollmentsAsync(entity.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        }

        entity.IsActive = request.DTO.IsActive;
        entity.UpdatedById = scope.ActorId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        _logger.LogInformation("Changed TenantDevice {TenantDeviceId} status to {IsActive} for Tenant {TenantId} by actor {ActorId}.", entity.Id, entity.IsActive, scope.TenantId, scope.ActorId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceStatusUpdated);
    }
}

/// <summary>Handles TenantDevice soft deletion.</summary>
public sealed class DeleteTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<DeleteTenantDeviceCommand, ApiResponse<bool>>
{
    private readonly ILogger<DeleteTenantDeviceCommandHandler> _logger;

    /// <summary>Initializes handler dependencies.</summary>
    public DeleteTenantDeviceCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger, ILogger<DeleteTenantDeviceCommandHandler> logger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteTenantDeviceCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (await UnitOfWork.TenantDeviceRepository.HasEnrollmentsAsync(entity.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        }

        entity.IsSoftDeleted = true;
        entity.IsActive = false;
        entity.SoftDeletedById = scope.ActorId;
        entity.SoftDeletedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Soft deleted TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, entity.TenantId, scope.ActorId);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantDeviceDeleted);
    }
}

/// <summary>Handles TenantDevice retrieval by identifier.</summary>
public sealed class GetTenantDeviceByIdQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetTenantDeviceByIdQuery, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;

    /// <summary>Initializes handler dependencies.</summary>
    public GetTenantDeviceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(GetTenantDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapResponse(_mapper, entity, scope), AppConstants.SuccessMessages.TenantDeviceRetrieved);
    }
}

/// <summary>Handles database-paged TenantDevice retrieval.</summary>
public sealed class GetAllTenantDevicesQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetAllTenantDevicesQuery, ApiResponse<List<TenantDeviceResponseDTO>>>
{
    private readonly IMapper _mapper;

    /// <summary>Initializes handler dependencies.</summary>
    public GetAllTenantDevicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantDeviceResponseDTO>>> Handle(GetAllTenantDevicesQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new GetTenantDeviceListRequestDTO();
        var scope = await ResolveTenantScopeAsync(filter, cancellationToken);
        var page = await UnitOfWork.TenantDeviceRepository.GetPagedAsync(scope.TenantId, filter, cancellationToken);
        var data = page.Data.Select(entity => MapResponse(_mapper, entity, scope)).ToList();
        return ApiResponse<List<TenantDeviceResponseDTO>>.SuccessPaginated(data, page.PageNumber, page.PageSize, page.TotalCount, page.TotalPages, AppConstants.SuccessMessages.TenantDeviceRetrieved);
    }
}

internal static class TenantDeviceCommandValidation
{
    internal static void Validate(TenantDeviceRequestDTO? dto)
    {
        if (dto is null || dto.TenantLocationId <= 0 || dto.DeviceMasterId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceCode) || string.IsNullOrWhiteSpace(dto.SerialNumber) || (dto.CommunicationType.HasValue && !Enum.IsDefined(dto.CommunicationType.Value)))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
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
