// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles Host and Tenant administration of physical Tenant devices and their configurations.
// ================================================================

using System.Text.Json;
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

namespace axionpro.application.Features.HostDeviceCmd.Handlers;

#region Tenant Device Commands

/// <summary>Creates a physical Tenant device without creating its connection configuration.</summary>
public sealed class CreateTenantDeviceCommand(CreateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public CreateTenantDeviceRequestDTO DTO { get; } = dto;
}

/// <summary>Updates a physical Tenant device installation record.</summary>
public sealed class UpdateTenantDeviceCommand(UpdateTenantDeviceRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public UpdateTenantDeviceRequestDTO DTO { get; } = dto;
}

/// <summary>Changes a physical Tenant device active state.</summary>
public sealed class UpdateTenantDeviceStatusCommand(UpdateTenantDeviceStatusRequestDTO dto) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public UpdateTenantDeviceStatusRequestDTO DTO { get; } = dto;
}

/// <summary>Soft deletes a physical Tenant device after its dependent configuration has been hard deleted.</summary>
public sealed class DeleteTenantDeviceCommand(long id, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<bool>>
{
    public long Id { get; } = id;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

#endregion

#region Tenant Device Queries

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

#region Tenant Device Configuration Commands

/// <summary>Creates the separate connection configuration for a Tenant device.</summary>
public sealed class CreateTenantDeviceConfigurationCommand(CreateTenantDeviceConfigurationRequestDTO dto) : IRequest<ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    public CreateTenantDeviceConfigurationRequestDTO DTO { get; } = dto;
}

/// <summary>Updates the separate connection configuration for a Tenant device.</summary>
public sealed class UpdateTenantDeviceConfigurationCommand(UpdateTenantDeviceConfigurationRequestDTO dto) : IRequest<ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    public UpdateTenantDeviceConfigurationRequestDTO DTO { get; } = dto;
}

/// <summary>Hard deletes the separate connection configuration for a Tenant device.</summary>
public sealed class DeleteTenantDeviceConfigurationCommand(long id, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<bool>>
{
    public long Id { get; } = id;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

#endregion

#region Tenant Device Configuration Queries

/// <summary>Retrieves one Tenant device configuration.</summary>
public sealed class GetTenantDeviceConfigurationByIdQuery(long id, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    public long Id { get; } = id;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

/// <summary>Retrieves a database-paged Tenant device configuration list.</summary>
public sealed class GetAllTenantDeviceConfigurationsQuery(GetTenantDeviceConfigurationListRequestDTO filter) : IRequest<ApiResponse<List<TenantDeviceConfigurationResponseDTO>>>
{
    public GetTenantDeviceConfigurationListRequestDTO Filter { get; } = filter;
}

#endregion

#region Shared Access

/// <summary>Represents the trusted Tenant scope and identifier-protection key for a device request.</summary>
public sealed record TenantDeviceAccessScope(long TenantId, long ActorId, string TenantEncryptionKey);

/// <summary>Resolves authoritative Host or Tenant access for TenantDevice resources through the established permission flows.</summary>
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

    /// <summary>Validates the principal and resolves the authoritative Tenant, audit actor, and encryption key.</summary>
    protected async Task<TenantDeviceAccessScope> ResolveTenantScopeAsync(TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
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

    /// <summary>Maps a Tenant device response and protects the Tenant identifier.</summary>
    protected TenantDeviceResponseDTO MapDeviceResponse(IMapper mapper, TenantDevice entity, TenantDeviceAccessScope scope)
    {
        var response = mapper.Map<TenantDeviceResponseDTO>(entity);
        response.TenantId = EncryptTenantId(entity.TenantId, scope.TenantEncryptionKey);
        return response;
    }

    /// <summary>Maps a configuration response and protects the parent device Tenant identifier.</summary>
    protected TenantDeviceConfigurationResponseDTO MapConfigurationResponse(IMapper mapper, TenantDeviceConfiguration entity, TenantDeviceAccessScope scope)
    {
        var response = mapper.Map<TenantDeviceConfigurationResponseDTO>(entity);
        response.TenantId = EncryptTenantId(entity.TenantDevice.TenantId, scope.TenantEncryptionKey);
        return response;
    }

    private string EncryptTenantId(long tenantId, string tenantEncryptionKey) =>
        HostTenantIdentifierProtector.Encrypt(tenantId, tenantEncryptionKey, _idEncoderService);

    private async Task<TenantDeviceAccessScope> ResolveHostTenantScopeAsync(TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            CommonRequestService,
            UnitOfWork.StoreProcedureRepository,
            accessRequest.ModuleId,
            accessRequest.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(accessRequest.TenantId, hostContext.TenantEncryptionKey, _idEncoderService);
        return new TenantDeviceAccessScope(tenantId, hostContext.HostUserId, hostContext.TenantEncryptionKey);
    }

    private async Task<TenantDeviceAccessScope> ResolveTenantEmployeeScopeAsync(TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
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

#endregion

#region Tenant Device Handlers

/// <summary>Handles creation of a Tenant device before configuration is supplied.</summary>
public sealed class CreateTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<CreateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTenantDeviceCommandHandler> _logger;

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
        TenantDeviceValidation.Validate(request.DTO);
        await TenantDeviceValidation.ValidateReferencesAsync(UnitOfWork, scope.TenantId, request.DTO.TenantLocationId, request.DTO.DeviceMasterId, cancellationToken);
        await TenantDeviceValidation.ValidateUniqueDeviceCodeAsync(UnitOfWork, scope.TenantId, request.DTO.DeviceCode, null, cancellationToken);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var master = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(request.DTO.DeviceMasterId, cancellationToken)
                ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceMaster);
            if (!master.IsActive || master.IsSoftDeleted || master.IsOccupied)
            {
                throw new ConflictException(AppConstants.ErrorMessages.DeviceMasterAlreadyRegisteredWithTenant);
            }

            var entity = _mapper.Map<TenantDevice>(request.DTO);
            TenantDeviceValidation.ApplyNormalizedValues(entity, request.DTO);
            entity.TenantId = scope.TenantId;
            entity.IsSoftDeleted = false;
            entity.AddedById = scope.ActorId;
            entity.AddedDateTime = DateTime.UtcNow;
            master.IsOccupied = true;
            master.UpdatedById = scope.ActorId;
            master.UpdatedDateTime = DateTime.UtcNow;

            await UnitOfWork.TenantDeviceRepository.AddAsync(entity, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);

            var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
            _logger.LogInformation("Created TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, scope.TenantId, scope.ActorId);
            return ApiResponse<TenantDeviceResponseDTO>.Success(MapDeviceResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceCreated);
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>Handles Tenant device installation updates while preserving separate configuration data.</summary>
public sealed class UpdateTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<UpdateTenantDeviceCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTenantDeviceCommandHandler> _logger;

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
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        TenantDeviceValidation.Validate(request.DTO);

        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, request.DTO.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        await TenantDeviceValidation.ValidateReferencesAsync(UnitOfWork, scope.TenantId, request.DTO.TenantLocationId, request.DTO.DeviceMasterId, cancellationToken);
        await TenantDeviceValidation.ValidateUniqueDeviceCodeAsync(UnitOfWork, scope.TenantId, request.DTO.DeviceCode, entity.Id, cancellationToken);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.TenantDeviceRepository.HasActiveEnrollmentsAsync(entity.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        }

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (entity.DeviceMasterId != request.DTO.DeviceMasterId)
            {
                var replacementMaster = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(request.DTO.DeviceMasterId, cancellationToken)
                    ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceMaster);
                if (!replacementMaster.IsActive || replacementMaster.IsSoftDeleted || replacementMaster.IsOccupied)
                {
                    throw new ConflictException(AppConstants.ErrorMessages.DeviceMasterAlreadyRegisteredWithTenant);
                }

                var previousMaster = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(entity.DeviceMasterId, cancellationToken)
                    ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidDeviceMaster);
                previousMaster.IsOccupied = false;
                previousMaster.UpdatedById = scope.ActorId;
                previousMaster.UpdatedDateTime = DateTime.UtcNow;
                replacementMaster.IsOccupied = true;
                replacementMaster.UpdatedById = scope.ActorId;
                replacementMaster.UpdatedDateTime = DateTime.UtcNow;
            }

            _mapper.Map(request.DTO, entity);
            TenantDeviceValidation.ApplyNormalizedValues(entity, request.DTO);
            entity.UpdatedById = scope.ActorId;
            entity.UpdatedDateTime = DateTime.UtcNow;
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);

            var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
            _logger.LogInformation("Updated TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, scope.TenantId, scope.ActorId);
            return ApiResponse<TenantDeviceResponseDTO>.Success(MapDeviceResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceUpdated);
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>Handles TenantDevice active-state changes.</summary>
public sealed class UpdateTenantDeviceStatusCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<UpdateTenantDeviceStatusCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTenantDeviceStatusCommandHandler> _logger;

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
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);

        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, request.DTO.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (request.DTO.IsActive)
        {
            await TenantDeviceValidation.ValidateReferencesAsync(UnitOfWork, scope.TenantId, entity.TenantLocationId, entity.DeviceMasterId, cancellationToken);
        }
        else if (await UnitOfWork.TenantDeviceRepository.HasActiveEnrollmentsAsync(entity.Id, cancellationToken))
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
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapDeviceResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceStatusUpdated);
    }
}

/// <summary>Handles TenantDevice soft deletion.</summary>
public sealed class DeleteTenantDeviceCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<DeleteTenantDeviceCommand, ApiResponse<bool>>
{
    private readonly ILogger<DeleteTenantDeviceCommandHandler> _logger;

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
        if (await UnitOfWork.TenantDeviceRepository.HasEnrollmentsAsync(entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceEnrollmentInUse);
        if (await UnitOfWork.TenantDeviceRepository.HasConfigurationAsync(scope.TenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceConfigurationInUse);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var master = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(entity.DeviceMasterId, cancellationToken);
            entity.IsSoftDeleted = true;
            entity.IsActive = false;
            entity.SoftDeletedById = scope.ActorId;
            entity.SoftDeletedDateTime = DateTime.UtcNow;
            if (master is not null)
            {
                master.IsOccupied = false;
                master.UpdatedById = scope.ActorId;
                master.UpdatedDateTime = DateTime.UtcNow;
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Soft deleted TenantDevice {TenantDeviceId} for Tenant {TenantId} by actor {ActorId}.", entity.Id, scope.TenantId, scope.ActorId);
            return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantDeviceDeleted);
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>Handles TenantDevice retrieval by identifier.</summary>
public sealed class GetTenantDeviceByIdQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetTenantDeviceByIdQuery, ApiResponse<TenantDeviceResponseDTO>>
{
    private readonly IMapper _mapper;

    public GetTenantDeviceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(GetTenantDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        return ApiResponse<TenantDeviceResponseDTO>.Success(MapDeviceResponse(_mapper, entity, scope), AppConstants.SuccessMessages.TenantDeviceRetrieved);
    }
}

/// <summary>Handles database-paged TenantDevice retrieval.</summary>
public sealed class GetAllTenantDevicesQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetAllTenantDevicesQuery, ApiResponse<List<TenantDeviceResponseDTO>>>
{
    private readonly IMapper _mapper;

    public GetAllTenantDevicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantDeviceResponseDTO>>> Handle(GetAllTenantDevicesQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new GetTenantDeviceListRequestDTO();
        var scope = await ResolveTenantScopeAsync(filter, cancellationToken);
        var page = await UnitOfWork.TenantDeviceRepository.GetPagedAsync(scope.TenantId, filter, cancellationToken);
        return ApiResponse<List<TenantDeviceResponseDTO>>.SuccessPaginated(page.Data.Select(entity => MapDeviceResponse(_mapper, entity, scope)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, page.TotalPages, AppConstants.SuccessMessages.TenantDeviceRetrieved);
    }
}

#endregion

#region Tenant Device Configuration Handlers

/// <summary>Handles creation of a separate TenantDeviceConfiguration record.</summary>
public sealed class CreateTenantDeviceConfigurationCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<CreateTenantDeviceConfigurationCommand, ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    private readonly IMapper _mapper;

    public CreateTenantDeviceConfigurationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceConfigurationResponseDTO>> Handle(CreateTenantDeviceConfigurationCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        TenantDeviceConfigurationValidation.Validate(request.DTO);
        if (!await UnitOfWork.TenantDeviceConfigurationRepository.IsEligibleTenantDeviceAsync(scope.TenantId, request.DTO.TenantDeviceId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (await UnitOfWork.TenantDeviceConfigurationRepository.ExistsForTenantDeviceAsync(scope.TenantId, request.DTO.TenantDeviceId, null, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceConfigurationAlreadyExists);

        var entity = _mapper.Map<TenantDeviceConfiguration>(request.DTO);
        TenantDeviceConfigurationValidation.ApplyNormalizedValues(entity, request.DTO);
        entity.AddedById = scope.ActorId;
        entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.TenantDeviceConfigurationRepository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceConfigurationRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        return ApiResponse<TenantDeviceConfigurationResponseDTO>.Success(MapConfigurationResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceConfigurationCreated);
    }
}

/// <summary>Handles updates to a separate TenantDeviceConfiguration record.</summary>
public sealed class UpdateTenantDeviceConfigurationCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<UpdateTenantDeviceConfigurationCommand, ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    private readonly IMapper _mapper;

    public UpdateTenantDeviceConfigurationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceConfigurationResponseDTO>> Handle(UpdateTenantDeviceConfigurationCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        TenantDeviceConfigurationValidation.Validate(request.DTO);

        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateAsync(scope.TenantId, request.DTO.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        if (!await UnitOfWork.TenantDeviceConfigurationRepository.IsEligibleTenantDeviceAsync(scope.TenantId, request.DTO.TenantDeviceId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (await UnitOfWork.TenantDeviceConfigurationRepository.ExistsForTenantDeviceAsync(scope.TenantId, request.DTO.TenantDeviceId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceConfigurationAlreadyExists);

        _mapper.Map(request.DTO, entity);
        TenantDeviceConfigurationValidation.ApplyNormalizedValues(entity, request.DTO);
        entity.UpdatedById = scope.ActorId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceConfigurationRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        return ApiResponse<TenantDeviceConfigurationResponseDTO>.Success(MapConfigurationResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceConfigurationUpdated);
    }
}

/// <summary>Hard deletes a TenantDeviceConfiguration record.</summary>
public sealed class DeleteTenantDeviceConfigurationCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<DeleteTenantDeviceConfigurationCommand, ApiResponse<bool>>
{
    public DeleteTenantDeviceConfigurationCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) { }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteTenantDeviceConfigurationCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        UnitOfWork.TenantDeviceConfigurationRepository.Remove(entity);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantDeviceConfigurationDeleted);
    }
}

/// <summary>Handles TenantDeviceConfiguration retrieval by identifier.</summary>
public sealed class GetTenantDeviceConfigurationByIdQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetTenantDeviceConfigurationByIdQuery, ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    private readonly IMapper _mapper;

    public GetTenantDeviceConfigurationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceConfigurationResponseDTO>> Handle(GetTenantDeviceConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetByIdAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        return ApiResponse<TenantDeviceConfigurationResponseDTO>.Success(MapConfigurationResponse(_mapper, entity, scope), AppConstants.SuccessMessages.TenantDeviceConfigurationRetrieved);
    }
}

/// <summary>Handles database-paged TenantDeviceConfiguration retrieval.</summary>
public sealed class GetAllTenantDeviceConfigurationsQueryHandler : TenantDeviceAccessHandlerBase, IRequestHandler<GetAllTenantDeviceConfigurationsQuery, ApiResponse<List<TenantDeviceConfigurationResponseDTO>>>
{
    private readonly IMapper _mapper;

    public GetAllTenantDeviceConfigurationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantDeviceConfigurationResponseDTO>>> Handle(GetAllTenantDeviceConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new GetTenantDeviceConfigurationListRequestDTO();
        var scope = await ResolveTenantScopeAsync(filter, cancellationToken);
        var page = await UnitOfWork.TenantDeviceConfigurationRepository.GetPagedAsync(scope.TenantId, filter, cancellationToken);
        return ApiResponse<List<TenantDeviceConfigurationResponseDTO>>.SuccessPaginated(page.Data.Select(entity => MapConfigurationResponse(_mapper, entity, scope)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, page.TotalPages, AppConstants.SuccessMessages.TenantDeviceConfigurationRetrieved);
    }
}

#endregion

#region Validation

internal static class TenantDeviceValidation
{
    internal static void Validate(TenantDeviceRequestDTO? dto)
    {
        if (dto is null || dto.TenantLocationId <= 0 || dto.DeviceMasterId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceCode))
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

    internal static async Task ValidateUniqueDeviceCodeAsync(IUnitOfWork unitOfWork, long tenantId, string deviceCode, long? excludeId, CancellationToken cancellationToken)
    {
        if (await unitOfWork.TenantDeviceRepository.DeviceCodeExistsAsync(tenantId, deviceCode, excludeId, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantDeviceCode);
    }

    internal static void ApplyNormalizedValues(TenantDevice entity, TenantDeviceRequestDTO dto)
    {
        entity.DeviceCode = dto.DeviceCode.Trim();
        entity.DeviceName = Normalize(dto.DeviceName);
        entity.InstallationRemark = Normalize(dto.InstallationRemark);
        entity.Description = Normalize(dto.Description);
        entity.Remark = Normalize(dto.Remark);
    }

    internal static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

internal static class TenantDeviceConfigurationValidation
{
    internal static void Validate(TenantDeviceConfigurationRequestDTO? dto)
    {
        if (dto is null || dto.TenantDeviceId <= 0 || (dto.CommunicationType.HasValue && !Enum.IsDefined(dto.CommunicationType.Value)) || (dto.DevicePort.HasValue && dto.DevicePort <= 0) || (dto.ServerPort.HasValue && dto.ServerPort <= 0) || (dto.HeartbeatIntervalSeconds.HasValue && dto.HeartbeatIntervalSeconds <= 0))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        if (!string.IsNullOrWhiteSpace(dto.Configuration))
        {
            try
            {
                using var document = JsonDocument.Parse(dto.Configuration);
            }
            catch (JsonException)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }
        }
    }

    internal static void ApplyNormalizedValues(TenantDeviceConfiguration entity, TenantDeviceConfigurationRequestDTO dto)
    {
        entity.IpAddress = TenantDeviceValidation.Normalize(dto.IpAddress);
        entity.MacAddress = TenantDeviceValidation.Normalize(dto.MacAddress);
        entity.ServerHost = TenantDeviceValidation.Normalize(dto.ServerHost);
        entity.ServerPath = TenantDeviceValidation.Normalize(dto.ServerPath);
        entity.ServerUrl = TenantDeviceValidation.Normalize(dto.ServerUrl);
        entity.PushMode = TenantDeviceValidation.Normalize(dto.PushMode);
        entity.TimeZoneId = TenantDeviceValidation.Normalize(dto.TimeZoneId);
        entity.Configuration = TenantDeviceValidation.Normalize(dto.Configuration);
    }
}

#endregion
