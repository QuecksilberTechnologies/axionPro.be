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
using axionpro.application.Interfaces.IDeviceCommunication;
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
public sealed class DeleteTenantDeviceCommand(string encryptedId, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<bool>>
{
    public string EncryptedId { get; } = encryptedId;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

#endregion

#region Tenant Device Queries

/// <summary>Retrieves one physical Tenant device.</summary>
public sealed class GetTenantDeviceByIdQuery(string encryptedId, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public string EncryptedId { get; } = encryptedId;
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

/// <summary>Rotates the opaque HTTPS bearer URL for one configured Tenant device.</summary>
public sealed class RotateTenantDeviceHttpsIngressTokenCommand(RotateTenantDeviceHttpsIngressTokenRequestDTO dto)
    : IRequest<ApiResponse<TenantDeviceHttpsIngressEndpointResponseDTO>>
{
    public RotateTenantDeviceHttpsIngressTokenRequestDTO DTO { get; } = dto;
}

/// <summary>Hard deletes the separate connection configuration for a Tenant device.</summary>
public sealed class DeleteTenantDeviceConfigurationCommand(string encryptedId, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<bool>>
{
    public string EncryptedId { get; } = encryptedId;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

#endregion

#region Initial Device Provisioning and Runtime Commands

/// <summary>Creates a short-lived Host bootstrap URL for an unassigned HTTPS-capable device.</summary>
public sealed class IssueInitialDeviceBootstrapCommand(IssueInitialDeviceBootstrapRequestDTO dto)
    : IRequest<ApiResponse<InitialDeviceBootstrapResponseDTO>>
{
    /// <summary>Gets the Host provisioning request.</summary>
    public IssueInitialDeviceBootstrapRequestDTO DTO { get; } = dto;
}

/// <summary>Queues approved Tenant-admin runtime device settings without accepting raw vendor JSON.</summary>
public sealed class ApplyTenantDeviceRuntimeConfigurationCommand(ApplyTenantDeviceRuntimeConfigurationRequestDTO dto)
    : IRequest<ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>>
{
    /// <summary>Gets the Tenant runtime configuration request.</summary>
    public ApplyTenantDeviceRuntimeConfigurationRequestDTO DTO { get; } = dto;
}

/// <summary>Queues a Tenant-admin-authorized device reboot through the configured HTTPS gateway.</summary>
public sealed class RebootTenantDeviceCommand(RebootTenantDeviceRequestDTO dto)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    /// <summary>Gets the reboot request.</summary>
    public RebootTenantDeviceRequestDTO DTO { get; } = dto;
}

#endregion

#region Tenant Device Configuration Queries

/// <summary>Retrieves one Tenant device configuration.</summary>
public sealed class GetTenantDeviceConfigurationByIdQuery(string encryptedId, TenantDeviceAccessRequestDTO accessRequest) : IRequest<ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    public string EncryptedId { get; } = encryptedId;
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
public sealed record TenantDeviceAccessScope(
    long TenantId,
    long ActorId,
    string TenantEncryptionKey,
    LoginUserType UserType);

/// <summary>Represents the trusted list scope and identifier-protection key for a device query.</summary>
public sealed record TenantDeviceListAccessScope(
    long? TenantId,
    string TenantEncryptionKey,
    LoginUserType UserType);

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

    /// <summary>
    /// Resolves the Host-only inventory-assignment scope. Tenant users may view
    /// assigned devices but cannot create, replace, disable, or remove a
    /// physical Tenant-device assignment.
    /// </summary>
    protected async Task<TenantDeviceAccessScope> ResolveHostDeviceAssignmentScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        if (principal.UserType != LoginUserType.Host)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        return await ResolveHostTenantScopeAsync(accessRequest, cancellationToken);
    }

    /// <summary>Maps a Tenant device response and protects the Tenant identifier.</summary>
    protected TenantDeviceResponseDTO MapDeviceResponse(IMapper mapper, TenantDevice entity, TenantDeviceAccessScope scope)
        => MapDeviceResponse(mapper, entity, scope.TenantEncryptionKey, scope.UserType == LoginUserType.Host);

    /// <summary>Maps a Tenant device response while protecting its Tenant identifier with the trusted request key.</summary>
    protected TenantDeviceResponseDTO MapDeviceResponse(
        IMapper mapper,
        TenantDevice entity,
        string tenantEncryptionKey,
        bool includeDeviceMasterMetadata)
    {
        var response = mapper.Map<TenantDeviceResponseDTO>(entity);
        response.TenantId = EncryptTenantId(entity.TenantId, tenantEncryptionKey);
        response.Id = EncryptIdentifier(entity.Id, tenantEncryptionKey);
        if (!includeDeviceMasterMetadata)
        {
            response.DeviceMasterId = null;
            response.DeviceMasterName = null;
            response.DeviceMasterModelNo = null;
        }

        return response;
    }

    /// <summary>Maps a configuration response and protects the parent device Tenant identifier.</summary>
    protected TenantDeviceConfigurationResponseDTO MapConfigurationResponse(IMapper mapper, TenantDeviceConfiguration entity, TenantDeviceAccessScope scope)
        => MapConfigurationResponse(mapper, entity, scope.TenantEncryptionKey, scope.UserType == LoginUserType.Host);

    /// <summary>Maps a device configuration response while protecting its parent Tenant identifier with the trusted request key.</summary>
    protected TenantDeviceConfigurationResponseDTO MapConfigurationResponse(
        IMapper mapper,
        TenantDeviceConfiguration entity,
        string tenantEncryptionKey,
        bool includeDeviceMasterMetadata)
    {
        var response = mapper.Map<TenantDeviceConfigurationResponseDTO>(entity);
        response.TenantId = EncryptTenantId(entity.TenantDevice.TenantId, tenantEncryptionKey);
        response.Id = EncryptIdentifier(entity.Id, tenantEncryptionKey);
        response.TenantDeviceId = EncryptIdentifier(entity.TenantDeviceId, tenantEncryptionKey);
        if (!includeDeviceMasterMetadata)
        {
            response.DeviceMasterName = null;
            response.DeviceMasterSNo = null;
        }

        return response;
    }

    /// <summary>
    /// Resolves a list scope. Host Admin can omit TenantId to list every live Tenant record;
    /// all other callers remain restricted to an authorized Tenant.
    /// </summary>
    protected async Task<TenantDeviceListAccessScope> ResolveTenantListScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        if (principal.UserType == LoginUserType.Host)
        {
            var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
                CommonRequestService,
                UnitOfWork.StoreProcedureRepository,
                accessRequest.ModuleId,
                accessRequest.OperationId,
                cancellationToken);

            if (hostContext.CurrentHostRoleId == AppConstants.SuperAdminHostRoleId &&
                string.IsNullOrWhiteSpace(accessRequest.TenantId))
            {
                return new TenantDeviceListAccessScope(null, hostContext.TenantEncryptionKey, LoginUserType.Host);
            }

            var tenantId = HostTenantIdentifierProtector.Decrypt(
                accessRequest.TenantId,
                hostContext.TenantEncryptionKey,
                _idEncoderService);
            return new TenantDeviceListAccessScope(tenantId, hostContext.TenantEncryptionKey, LoginUserType.Host);
        }

        if (principal.UserType == LoginUserType.TenantEmployee)
        {
            var tenantScope = await ResolveTenantEmployeeScopeAsync(accessRequest, cancellationToken);
            return new TenantDeviceListAccessScope(tenantScope.TenantId, tenantScope.TenantEncryptionKey, LoginUserType.TenantEmployee);
        }

        throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
    }

    private string EncryptTenantId(long tenantId, string tenantEncryptionKey) =>
        HostTenantIdentifierProtector.Encrypt(tenantId, tenantEncryptionKey, _idEncoderService);

    /// <summary>
    /// Encodes a Tenant-device aggregate identifier for every external response.
    /// This follows the same IIdEncoderService pattern used by Employee handlers.
    /// </summary>
    protected string EncryptIdentifier(long identifier, string tenantEncryptionKey) =>
        _idEncoderService.EncodeId_long(identifier, tenantEncryptionKey);

    /// <summary>Decodes and validates an identifier only after the caller Tenant scope is trusted.</summary>
    protected long DecodeIdentifier(string? encryptedIdentifier, TenantDeviceAccessScope scope, string identifierName)
        => DecodeIdentifier(encryptedIdentifier, scope.TenantEncryptionKey, identifierName);

    /// <summary>Decodes and validates an identifier using a trusted Tenant encryption key.</summary>
    protected long DecodeIdentifier(string? encryptedIdentifier, string tenantEncryptionKey, string identifierName)
    {
        if (string.IsNullOrWhiteSpace(encryptedIdentifier))
        {
            throw new ValidationErrorException($"A valid {identifierName} is required.");
        }

        var token = encryptedIdentifier.Trim();
        var identifier = _idEncoderService.DecodeId_long(token, tenantEncryptionKey);
        if (identifier <= 0 ||
            !string.Equals(EncryptIdentifier(identifier, tenantEncryptionKey), token, StringComparison.Ordinal))
        {
            throw new ValidationErrorException($"A valid {identifierName} is required.");
        }

        return identifier;
    }

    private async Task<TenantDeviceAccessScope> ResolveHostTenantScopeAsync(TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            CommonRequestService,
            UnitOfWork.StoreProcedureRepository,
            accessRequest.ModuleId,
            accessRequest.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(accessRequest.TenantId, hostContext.TenantEncryptionKey, _idEncoderService);
        return new TenantDeviceAccessScope(tenantId, hostContext.HostUserId, hostContext.TenantEncryptionKey, LoginUserType.Host);
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

        return new TenantDeviceAccessScope(tenantId, actorId, tenantValidation.Claims.TenantEncriptionKey, LoginUserType.TenantEmployee);
    }

    /// <summary>Resolves Tenant runtime scope after the pipeline behavior has already checked the module and operation permission.</summary>
    protected async Task<TenantDeviceAccessScope> ResolveTenantConfigurationScopeAsync(
        CancellationToken cancellationToken)
    {
        var tenantValidation = await ValidateTenantDataAccessContextAsync();
        if (string.IsNullOrWhiteSpace(tenantValidation.Claims.TenantEncriptionKey))
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        return new TenantDeviceAccessScope(
            tenantValidation.TenantId,
            tenantValidation.LoggedInEmployeeId,
            tenantValidation.Claims.TenantEncriptionKey,
            LoginUserType.TenantEmployee);
    }

    /// <summary>
    /// Resolves a configuration scope after the device-configuration permission behavior has authorized the request.
    /// A Host user acts on the encrypted Tenant selected in the request; a Tenant employee remains scoped to their token Tenant.
    /// </summary>
    protected async Task<TenantDeviceAccessScope> ResolveAuthorizedTenantConfigurationScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        if (principal.UserType == LoginUserType.Host)
        {
            var hostContext = await CommonRequestService.ValidateHostUserPermissionRequestAsync();
            var tenantId = HostTenantIdentifierProtector.Decrypt(
                accessRequest.TenantId,
                hostContext.TenantEncryptionKey,
                _idEncoderService);
            return new TenantDeviceAccessScope(
                tenantId,
                hostContext.HostUserId,
                hostContext.TenantEncryptionKey,
                LoginUserType.Host);
        }

        if (principal.UserType == LoginUserType.TenantEmployee)
        {
            return await ResolveTenantConfigurationScopeAsync(cancellationToken);
        }

        throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
    }

    /// <summary>
    /// Resolves the configuration list scope after the device-configuration permission behavior has authorized the request.
    /// Super Admin may omit TenantId to list every Tenant configuration; another Host user must select a Tenant.
    /// </summary>
    protected async Task<TenantDeviceListAccessScope> ResolveAuthorizedTenantConfigurationListScopeAsync(
        TenantDeviceAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        if (principal.UserType == LoginUserType.Host)
        {
            var hostContext = await CommonRequestService.ValidateHostUserPermissionRequestAsync();
            if (hostContext.CurrentHostRoleId == AppConstants.SuperAdminHostRoleId &&
                string.IsNullOrWhiteSpace(accessRequest.TenantId))
            {
                return new TenantDeviceListAccessScope(null, hostContext.TenantEncryptionKey, LoginUserType.Host);
            }

            var tenantId = HostTenantIdentifierProtector.Decrypt(
                accessRequest.TenantId,
                hostContext.TenantEncryptionKey,
                _idEncoderService);
            return new TenantDeviceListAccessScope(tenantId, hostContext.TenantEncryptionKey, LoginUserType.Host);
        }

        if (principal.UserType == LoginUserType.TenantEmployee)
        {
            var scope = await ResolveTenantConfigurationScopeAsync(cancellationToken);
            return new TenantDeviceListAccessScope(scope.TenantId, scope.TenantEncryptionKey, LoginUserType.TenantEmployee);
        }

        throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
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
        var scope = await ResolveHostDeviceAssignmentScopeAsync(request.DTO, cancellationToken);
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
        var scope = await ResolveHostDeviceAssignmentScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        TenantDeviceValidation.Validate(request.DTO);
        var tenantDeviceId = DecodeIdentifier(request.DTO.Id, scope, "TenantDeviceId");

        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
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
        var scope = await ResolveHostDeviceAssignmentScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var tenantDeviceId = DecodeIdentifier(request.DTO.Id, scope, "TenantDeviceId");

        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
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
        var scope = await ResolveHostDeviceAssignmentScopeAsync(request.AccessRequest, cancellationToken);
        var tenantDeviceId = DecodeIdentifier(request.EncryptedId, scope, "TenantDeviceId");
        var entity = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
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
        var tenantDeviceId = DecodeIdentifier(request.EncryptedId, scope, "TenantDeviceId");
        var entity = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, tenantDeviceId, cancellationToken)
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
        var scope = await ResolveTenantListScopeAsync(filter, cancellationToken);
        var page = scope.TenantId.HasValue
            ? await UnitOfWork.TenantDeviceRepository.GetPagedAsync(scope.TenantId.Value, filter, cancellationToken)
            : await UnitOfWork.TenantDeviceRepository.GetHostPagedAsync(filter, cancellationToken);
        return ApiResponse<List<TenantDeviceResponseDTO>>.SuccessPaginated(
            page.Data.Select(entity => MapDeviceResponse(
                _mapper,
                entity,
                scope.TenantEncryptionKey,
                scope.UserType == LoginUserType.Host)).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            AppConstants.SuccessMessages.TenantDeviceRetrieved);
    }
}

#endregion

#region Tenant Device Configuration and Runtime Handlers

/// <summary>Handles creation of a separate TenantDeviceConfiguration record.</summary>
public sealed class CreateTenantDeviceConfigurationCommandHandler : TenantDeviceAccessHandlerBase, IRequestHandler<CreateTenantDeviceConfigurationCommand, ApiResponse<TenantDeviceConfigurationResponseDTO>>
{
    private readonly IMapper _mapper;

    public CreateTenantDeviceConfigurationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) => _mapper = mapper;

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceConfigurationResponseDTO>> Handle(CreateTenantDeviceConfigurationCommand request, CancellationToken cancellationToken)
    {
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.DTO, cancellationToken);
        TenantDeviceConfigurationValidation.Validate(request.DTO);
        var tenantDeviceId = DecodeIdentifier(request.DTO.TenantDeviceId, scope, "TenantDeviceId");
        var tenantDevice = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (!tenantDevice.IsActive)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        }

        var deviceMaster = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(tenantDevice.DeviceMasterId, cancellationToken)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        TenantDeviceConfigurationValidation.ValidateDeviceTransport(deviceMaster, request.DTO);
        if (await UnitOfWork.TenantDeviceConfigurationRepository.ExistsForTenantDeviceAsync(scope.TenantId, tenantDeviceId, null, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceConfigurationAlreadyExists);

        var entity = _mapper.Map<TenantDeviceConfiguration>(request.DTO);
        entity.TenantDeviceId = tenantDeviceId;
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
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        TenantDeviceConfigurationValidation.Validate(request.DTO);
        var tenantDeviceConfigurationId = DecodeIdentifier(request.DTO.Id, scope, "TenantDeviceConfigurationId");
        var tenantDeviceId = DecodeIdentifier(request.DTO.TenantDeviceId, scope, "TenantDeviceId");

        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceConfigurationId, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        var tenantDevice = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        if (!tenantDevice.IsActive)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        }

        var deviceMaster = await UnitOfWork.DeviceMasterRepository.GetForUpdateAsync(tenantDevice.DeviceMasterId, cancellationToken)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        TenantDeviceConfigurationValidation.ValidateDeviceTransport(deviceMaster, request.DTO);
        if (await UnitOfWork.TenantDeviceConfigurationRepository.ExistsForTenantDeviceAsync(scope.TenantId, tenantDeviceId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantDeviceConfigurationAlreadyExists);

        _mapper.Map(request.DTO, entity);
        entity.TenantDeviceId = tenantDeviceId;
        TenantDeviceConfigurationValidation.ApplyNormalizedValues(entity, request.DTO);
        if (entity.CommandTransport != (short)DeviceCommunicationProtocol.Https)
        {
            // A stale token must never remain usable after a transport change.
            entity.HttpsIngressTokenHash = null;
        }
        entity.UpdatedById = scope.ActorId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceConfigurationRepository.GetByIdAsync(scope.TenantId, entity.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        return ApiResponse<TenantDeviceConfigurationResponseDTO>.Success(MapConfigurationResponse(_mapper, stored, scope), AppConstants.SuccessMessages.TenantDeviceConfigurationUpdated);
    }
}

/// <summary>
/// Generates a new opaque device-gateway URL exactly once. Only its SHA-256 hash
/// is retained, so a later configuration read cannot disclose the bearer token.
/// </summary>
public sealed class RotateTenantDeviceHttpsIngressTokenCommandHandler : TenantDeviceAccessHandlerBase,
    IRequestHandler<RotateTenantDeviceHttpsIngressTokenCommand, ApiResponse<TenantDeviceHttpsIngressEndpointResponseDTO>>
{
    public RotateTenantDeviceHttpsIngressTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService,
        ILogger<TenantConfigurationHandlerBase> tenantLogger)
        : base(unitOfWork, commonRequestService, idEncoderService, tenantLogger) { }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceHttpsIngressEndpointResponseDTO>> Handle(
        RotateTenantDeviceHttpsIngressTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.DTO, cancellationToken);
        var hasConfigurationId = !string.IsNullOrWhiteSpace(request.DTO.TenantDeviceConfigurationId);
        var hasTenantDeviceId = !string.IsNullOrWhiteSpace(request.DTO.TenantDeviceId);
        if (hasConfigurationId == hasTenantDeviceId)
        {
            throw new ValidationErrorException(
                "Provide exactly one encrypted TenantDeviceConfigurationId or TenantDeviceId.");
        }

        if (scope.UserType == LoginUserType.Host && !hasTenantDeviceId)
        {
            throw new ValidationErrorException("Host URL issuance requires an encrypted TenantDeviceId.");
        }

        if (scope.UserType == LoginUserType.TenantEmployee && !hasConfigurationId)
        {
            throw new ValidationErrorException("Tenant URL rotation requires an encrypted TenantDeviceConfigurationId.");
        }

        TenantDeviceConfiguration? configuration;
        if (hasConfigurationId)
        {
            var tenantDeviceConfigurationId = DecodeIdentifier(
                request.DTO.TenantDeviceConfigurationId,
                scope,
                "TenantDeviceConfigurationId");
            configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateAsync(
                scope.TenantId,
                tenantDeviceConfigurationId,
                cancellationToken);
        }
        else
        {
            var tenantDeviceId = DecodeIdentifier(request.DTO.TenantDeviceId, scope, "TenantDeviceId");
            configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken);
        }

        if (configuration is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        }

        var transport = configuration.CommandTransport ?? configuration.MqttTransport;
        if (transport != (short)DeviceCommunicationProtocol.Https ||
            !DeviceHttpsGatewaySecurity.IsValidGatewayPath(configuration.ServerPath) ||
            !Uri.TryCreate(configuration.ServerUrl, UriKind.Absolute, out var serverUri) ||
            !string.Equals(serverUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationErrorException("Configure this device for HTTPS polling before generating its gateway URL.");
        }

        var deviceMaster = await UnitOfWork.DeviceMasterRepository.GetByIdAsync(
            configuration.TenantDevice.DeviceMasterId,
            cancellationToken);
        if (deviceMaster is null || !deviceMaster.IsActive || deviceMaster.IsSoftDeleted || !deviceMaster.SupportsHttps)
        {
            throw new ValidationErrorException("The selected device model does not support HTTPS polling.");
        }

        var gatewayUrl = TenantDeviceConfigurationValidation.IssueHttpsGatewayUrl(configuration);
        configuration.UpdatedById = scope.ActorId;
        configuration.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        // Never log the generated URL or token. The caller receives it once and
        // must install it into the device's Server Domain Name field immediately.
        return ApiResponse<TenantDeviceHttpsIngressEndpointResponseDTO>.Success(
            new TenantDeviceHttpsIngressEndpointResponseDTO { GatewayUrl = gatewayUrl },
            "HTTPS device gateway URL generated. It will not be shown again.");
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
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.AccessRequest, cancellationToken);
        var tenantDeviceConfigurationId = DecodeIdentifier(request.EncryptedId, scope, "TenantDeviceConfigurationId");
        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceConfigurationId, cancellationToken)
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
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.AccessRequest, cancellationToken);
        var tenantDeviceConfigurationId = DecodeIdentifier(request.EncryptedId, scope, "TenantDeviceConfigurationId");
        var entity = await UnitOfWork.TenantDeviceConfigurationRepository.GetByIdAsync(scope.TenantId, tenantDeviceConfigurationId, cancellationToken)
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
        var scope = await ResolveAuthorizedTenantConfigurationListScopeAsync(filter, cancellationToken);
        filter.ResolvedTenantDeviceId = string.IsNullOrWhiteSpace(filter.TenantDeviceId)
            ? null
            : DecodeIdentifier(filter.TenantDeviceId, scope.TenantEncryptionKey, "TenantDeviceId");
        var page = scope.TenantId.HasValue
            ? await UnitOfWork.TenantDeviceConfigurationRepository.GetPagedAsync(scope.TenantId.Value, filter, cancellationToken)
            : await UnitOfWork.TenantDeviceConfigurationRepository.GetHostPagedAsync(filter, cancellationToken);
        return ApiResponse<List<TenantDeviceConfigurationResponseDTO>>.SuccessPaginated(
            page.Data.Select(entity => MapConfigurationResponse(
                _mapper,
                entity,
                scope.TenantEncryptionKey,
                scope.UserType == LoginUserType.Host)).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            AppConstants.SuccessMessages.TenantDeviceConfigurationRetrieved);
    }
}

/// <summary>Handles Host issuance of an initial device bootstrap URL.</summary>
public sealed class IssueInitialDeviceBootstrapCommandHandler(
    ICommonRequestService commonRequestService,
    IDeviceInitialProvisioningService initialProvisioningService)
    : IRequestHandler<IssueInitialDeviceBootstrapCommand, ApiResponse<InitialDeviceBootstrapResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<InitialDeviceBootstrapResponseDTO>> Handle(
        IssueInitialDeviceBootstrapCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (dto.DeviceMasterId <= 0)
        {
            throw new ValidationErrorException("A valid physical device is required.");
        }

        var hostContext = await commonRequestService.ValidateHostUserPermissionRequestAsync();
        var issue = await initialProvisioningService.IssueAsync(
            dto.DeviceMasterId,
            dto.LifetimeMinutes,
            hostContext.HostUserId,
            cancellationToken);

        return ApiResponse<InitialDeviceBootstrapResponseDTO>.Success(
            new InitialDeviceBootstrapResponseDTO
            {
                DeviceSerialNumber = issue.DeviceSerialNumber,
                InitialGatewayUrl = issue.InitialGatewayUrl,
                HeartbeatIntervalSeconds = issue.HeartbeatIntervalSeconds,
                ExpiresDateTime = issue.ExpiresDateTime
            },
            "Initial device gateway URL generated. It will not be shown again.");
    }
}

/// <summary>Handles strongly typed Tenant-admin runtime configuration for an HTTPS device.</summary>
public sealed class ApplyTenantDeviceRuntimeConfigurationCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<ApplyTenantDeviceRuntimeConfigurationCommand, ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>> Handle(
        ApplyTenantDeviceRuntimeConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.DTO, cancellationToken);
        TenantDeviceConfigurationValidation.ValidateRuntimeConfiguration(dto);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");

        var configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        TenantDeviceConfigurationValidation.EnsureHttpsGateway(configuration);
        var deviceMaster = await UnitOfWork.DeviceMasterRepository.GetByIdAsync(
            configuration.TenantDevice.DeviceMasterId,
            cancellationToken);
        if (deviceMaster is null || !deviceMaster.IsActive || deviceMaster.IsSoftDeleted || !deviceMaster.SupportsHttps ||
            !configuration.TenantDevice.IsActive || configuration.TenantDevice.IsSoftDeleted)
        {
            throw new ValidationErrorException("The Tenant device must be active and support HTTPS before runtime settings can be queued.");
        }

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            string? initialNormalGatewayUrl = null;
            if (string.IsNullOrWhiteSpace(configuration.HttpsIngressTokenHash))
            {
                initialNormalGatewayUrl = TenantDeviceConfigurationValidation.IssueHttpsGatewayUrl(configuration);
            }

            configuration.HeartbeatIntervalSeconds = dto.HeartbeatIntervalSeconds;
            configuration.Configuration = JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                heartbeatIntervalSeconds = dto.HeartbeatIntervalSeconds,
                volume = dto.Volume,
                localWebServerEnabled = false,
                managedBy = "TenantAdmin"
            });
            configuration.UpdatedById = scope.ActorId;
            configuration.UpdatedDateTime = DateTime.UtcNow;
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            var configurationCommand = await deviceCommandSubmissionService.SubmitAsync(
                new DeviceCommandSubmission(
                    scope.TenantId,
                    tenantDeviceId,
                    DeviceCommands.SetDeviceInfo,
                    TenantDeviceConfigurationValidation.BuildSetDeviceInfoPayload(dto, initialNormalGatewayUrl),
                    scope.ActorId,
                    ProtectPayload: true),
                cancellationToken);

            DeviceCommandSubmissionResult? rebootCommand = null;
            if (dto.RebootAfterApply)
            {
                rebootCommand = await deviceCommandSubmissionService.SubmitAsync(
                    new DeviceCommandSubmission(
                        scope.TenantId,
                        tenantDeviceId,
                        DeviceCommands.Reboot,
                        JsonSerializer.Serialize(new { cmd = DeviceCommands.Reboot }),
                        scope.ActorId),
                    cancellationToken);
            }

            await UnitOfWork.CommitTransactionAsync(cancellationToken);
            return ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>.Success(
                new TenantDeviceRuntimeConfigurationResponseDTO
                {
                    ConfigurationCommandId = configurationCommand.DeviceCommandId,
                    ConfigurationTrackingId = configurationCommand.InternalTrackingId,
                    RebootCommandId = rebootCommand?.DeviceCommandId,
                    RebootTrackingId = rebootCommand?.InternalTrackingId,
                    Status = configurationCommand.Status.ToString()
                },
                "Tenant device configuration has been queued securely.");
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>Handles a Tenant-admin reboot without the generic raw command endpoint.</summary>
public sealed class RebootTenantDeviceCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<RebootTenantDeviceCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        RebootTenantDeviceCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.DTO, cancellationToken);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");
        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                scope.TenantId,
                tenantDeviceId,
                DeviceCommands.Reboot,
                JsonSerializer.Serialize(new { cmd = DeviceCommands.Reboot }),
                scope.ActorId),
            cancellationToken);

        return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
            new DeviceCommandSubmissionResponseDTO
            {
                DeviceCommandId = result.DeviceCommandId,
                InternalTrackingId = result.InternalTrackingId,
                Status = result.Status.ToString()
            },
            "Tenant device reboot has been queued securely.");
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
        var transport = ResolveTransport(dto);
        if (dto is null ||
            string.IsNullOrWhiteSpace(dto.TenantDeviceId) ||
            !transport.HasValue ||
            !Enum.IsDefined(transport.Value) ||
            (dto.DevicePort.HasValue && dto.DevicePort <= 0) ||
            (dto.ServerPort.HasValue && dto.ServerPort <= 0) ||
            (dto.HeartbeatIntervalSeconds.HasValue && dto.HeartbeatIntervalSeconds <= 0))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        if (!string.IsNullOrWhiteSpace(dto.Configuration))
        {
            try
            {
                using var document = JsonDocument.Parse(dto.Configuration);
                EnsureNoPlainCredentialMaterial(document.RootElement);
            }
            catch (JsonException)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }
        }

        if (transport == DeviceCommunicationProtocol.Https)
        {
            if (!Uri.TryCreate(dto.ServerUrl, UriKind.Absolute, out var serverUri) ||
                !string.Equals(serverUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
                !DeviceHttpsGatewaySecurity.IsValidGatewayPath(dto.ServerPath) ||
                dto.ServerPort is not null and not 443 ||
                dto.HeartbeatIntervalSeconds is < 10 or > 3600)
            {
                throw new ValidationErrorException(
                    "HTTPS polling requires an absolute HTTPS ServerUrl, the /device-gateway path, port 443, and a 10–3600 second heartbeat.");
            }
        }
    }

    /// <summary>
    /// Ensures a selected transport is both catalog-confirmed for the physical
    /// device and backed by an enabled AxionPro device adapter.
    /// </summary>
    internal static void ValidateDeviceTransport(DeviceMaster deviceMaster, TenantDeviceConfigurationRequestDTO dto)
    {
        ArgumentNullException.ThrowIfNull(deviceMaster);
        var transport = ResolveTransport(dto)
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);

        var deviceSupportsTransport = transport switch
        {
            DeviceCommunicationProtocol.Mqtt => deviceMaster.SupportsMqtt,
            DeviceCommunicationProtocol.Mqtts => deviceMaster.SupportsMqtts,
            DeviceCommunicationProtocol.Https => deviceMaster.SupportsHttps,
            DeviceCommunicationProtocol.Http => deviceMaster.SupportsHttp,
            DeviceCommunicationProtocol.WebSocket or DeviceCommunicationProtocol.WebSocketSecure => deviceMaster.SupportsWebSocket,
            _ => false
        };
        if (!deviceMaster.IsActive || deviceMaster.IsSoftDeleted || !deviceSupportsTransport)
        {
            throw new ValidationErrorException("The selected physical device does not support the requested command transport.");
        }

        if (transport is not (DeviceCommunicationProtocol.Mqtt or DeviceCommunicationProtocol.Mqtts or DeviceCommunicationProtocol.Https))
        {
            throw new ValidationErrorException("The requested device transport does not yet have an enabled AxionPro transport adapter.");
        }

        if (transport is DeviceCommunicationProtocol.Mqtt or DeviceCommunicationProtocol.Mqtts &&
            (string.IsNullOrWhiteSpace(dto.ServerHost) || dto.ServerPort is null or < 1 or > 65535))
        {
            throw new ValidationErrorException("MQTT and MQTTS configurations require the broker host and a valid broker port.");
        }
    }

    private static DeviceCommunicationProtocol? ResolveTransport(TenantDeviceConfigurationRequestDTO? dto)
    {
        if (dto is null)
        {
            return null;
        }

        if (dto.CommandTransport.HasValue && dto.MqttTransport.HasValue &&
            dto.CommandTransport.Value != dto.MqttTransport.Value)
        {
            throw new ValidationErrorException("Send either CommandTransport or the legacy MqttTransport value, not conflicting values.");
        }

        if (dto.MqttTransport.HasValue &&
            dto.MqttTransport.Value is not DeviceCommunicationProtocol.Mqtt and not DeviceCommunicationProtocol.Mqtts)
        {
            throw new ValidationErrorException("The legacy MqttTransport field accepts only MQTT or MQTTS. Use CommandTransport for HTTPS or WebSocket.");
        }

        return dto.CommandTransport ?? dto.MqttTransport;
    }

    private static void EnsureNoPlainCredentialMaterial(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (property.Name.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Contains("credential", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Contains("apikey", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Contains("token", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationErrorException("Device credentials must be stored only in the encrypted DeviceCredential record.");
                }

                EnsureNoPlainCredentialMaterial(property.Value);
            }

            return;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                EnsureNoPlainCredentialMaterial(item);
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

    /// <summary>Validates the approved runtime fields before a protected vendor command is queued.</summary>
    internal static void ValidateRuntimeConfiguration(ApplyTenantDeviceRuntimeConfigurationRequestDTO dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.NewWebServerPassword))
        {
            throw new ValidationErrorException(
                "Use the typed settings/web-access endpoint to change the local Web UI/API password.");
        }

        if (string.IsNullOrWhiteSpace(dto.TenantDeviceId) ||
            string.IsNullOrWhiteSpace(dto.CurrentWebServerPassword) ||
            dto.CurrentWebServerPassword.Length is < 4 or > 128 ||
            dto.HeartbeatIntervalSeconds is < 10 or > 3600 ||
            dto.Volume is < 0 or > 15)
        {
            throw new ValidationErrorException(
                "A valid device password, 10–3600 second heartbeat, and volume 0–15 are required.");
        }
    }

    /// <summary>Ensures the stored connection is an HTTPS device gateway before runtime commands are allowed.</summary>
    internal static void EnsureHttpsGateway(TenantDeviceConfiguration configuration)
    {
        var transport = configuration.CommandTransport ?? configuration.MqttTransport;
        if (transport != (short)DeviceCommunicationProtocol.Https ||
            !Uri.TryCreate(configuration.ServerUrl, UriKind.Absolute, out var serverUri) ||
            !string.Equals(serverUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            !DeviceHttpsGatewaySecurity.IsValidGatewayPath(configuration.ServerPath) ||
            configuration.ServerPort is not null and not 443)
        {
            throw new ValidationErrorException(
                "Configure this Tenant device for HTTPS, port 443, and the /device-gateway server path before applying runtime settings.");
        }
    }

    /// <summary>
    /// Issues a one-time normal HTTPS gateway URL and persists only its token hash
    /// on the supplied configuration. The caller returns the raw URL exactly once.
    /// </summary>
    internal static string IssueHttpsGatewayUrl(TenantDeviceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        EnsureHttpsGateway(configuration);

        var ingressToken = DeviceHttpsGatewaySecurity.GenerateIngressToken();
        configuration.HttpsIngressTokenHash = DeviceHttpsGatewaySecurity.HashIngressToken(ingressToken);
        return $"{configuration.ServerUrl!.TrimEnd('/')}{DeviceHttpsGatewaySecurity.RoutePrefix}/{ingressToken}";
    }

    /// <summary>Builds the approved vendor request without persisting plaintext credentials.</summary>
    internal static string BuildSetDeviceInfoPayload(
        ApplyTenantDeviceRuntimeConfigurationRequestDTO dto,
        string? initialNormalGatewayUrl)
    {
        var payload = new Dictionary<string, object?>
        {
            ["cmd"] = DeviceCommands.SetDeviceInfo,
            ["password"] = dto.CurrentWebServerPassword,
            ["nowtime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ["server_response_time"] = dto.HeartbeatIntervalSeconds
        };

        if (dto.Volume.HasValue)
        {
            payload["volume"] = dto.Volume.Value;
        }

        if (!string.IsNullOrWhiteSpace(initialNormalGatewayUrl))
        {
            payload["use_bs"] = 1;
            payload["use_domain_name"] = 1;
            payload["bs_domain_name"] = initialNormalGatewayUrl;
            payload["serverport"] = 443;
        }

        return JsonSerializer.Serialize(payload);
    }
}

#endregion
