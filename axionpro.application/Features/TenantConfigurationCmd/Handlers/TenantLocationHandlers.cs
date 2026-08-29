// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant-owned work-location configuration.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Handlers;

#region Command

/// <summary>Creates a Tenant-owned work location.</summary>
public sealed class CreateTenantLocationCommand : IRequest<ApiResponse<TenantLocationResponseDTO>>
{
    /// <summary>Initializes a command with client-editable location values.</summary>
    public CreateTenantLocationCommand(CreateTenantLocationRequestDTO dto) => DTO = dto;
    /// <summary>Gets the location values to create.</summary>
    public CreateTenantLocationRequestDTO DTO { get; }
}

/// <summary>Updates a Tenant-owned work location.</summary>
public sealed class UpdateTenantLocationCommand : IRequest<ApiResponse<TenantLocationResponseDTO>>
{
    /// <summary>Initializes a command with client-editable location values.</summary>
    public UpdateTenantLocationCommand(UpdateTenantLocationRequestDTO dto) => DTO = dto;
    /// <summary>Gets the location values to update.</summary>
    public UpdateTenantLocationRequestDTO DTO { get; }
}

/// <summary>Soft deletes a Tenant-owned work location.</summary>
public sealed class DeleteTenantLocationCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>Initializes a command with the target location identifier.</summary>
    public DeleteTenantLocationCommand(long id, TenantLocationAccessRequestDTO accessRequest)
    {
        Id = id;
        AccessRequest = accessRequest;
    }
    /// <summary>Gets the target location identifier.</summary>
    public long Id { get; }
    /// <summary>Gets the encrypted Host Tenant scope and permission context.</summary>
    public TenantLocationAccessRequestDTO AccessRequest { get; }
}

/// <summary>Changes the active state of a Tenant-owned work location.</summary>
public sealed class UpdateTenantLocationStatusCommand : IRequest<ApiResponse<TenantLocationResponseDTO>>
{
    /// <summary>Initializes a command with the desired status.</summary>
    public UpdateTenantLocationStatusCommand(UpdateTenantLocationStatusRequestDTO dto) => DTO = dto;
    /// <summary>Gets the requested status change.</summary>
    public UpdateTenantLocationStatusRequestDTO DTO { get; }
}

#endregion

#region Query

/// <summary>Retrieves one Tenant-owned work location.</summary>
public sealed class GetTenantLocationByIdQuery : IRequest<ApiResponse<TenantLocationResponseDTO>>
{
    /// <summary>Initializes a query with the target location identifier.</summary>
    public GetTenantLocationByIdQuery(long id, TenantLocationAccessRequestDTO accessRequest)
    {
        Id = id;
        AccessRequest = accessRequest;
    }
    /// <summary>Gets the target location identifier.</summary>
    public long Id { get; }
    /// <summary>Gets the encrypted Host Tenant scope and permission context.</summary>
    public TenantLocationAccessRequestDTO AccessRequest { get; }
}

/// <summary>Retrieves filtered and paginated Tenant-owned work locations.</summary>
public sealed class GetTenantLocationsQuery : IRequest<ApiResponse<List<TenantLocationResponseDTO>>>
{
    /// <summary>Initializes a query with database-side filters.</summary>
    public GetTenantLocationsQuery(TenantLocationFilterRequestDTO filter) => Filter = filter;
    /// <summary>Gets the location filters and paging request.</summary>
    public TenantLocationFilterRequestDTO Filter { get; }
}

#endregion

#region Handler

/// <summary>
/// Resolves the trusted Tenant scope for TenantLocation endpoints. Host requests
/// must supply an encrypted Tenant identifier, while Tenant Employee requests
/// are constrained to the Tenant in their authenticated token.
/// </summary>
public abstract class TenantLocationAccessHandlerBase : TenantConfigurationHandlerBase
{
    private readonly IIdEncoderService _idEncoderService;

    protected TenantLocationAccessHandlerBase(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService,
        ILogger<TenantConfigurationHandlerBase> logger)
        : base(unitOfWork, commonRequestService, logger)
    {
        _idEncoderService = idEncoderService;
    }

    /// <summary>Validates the principal and resolves its authoritative Tenant and audit actor.</summary>
    protected async Task<(long TenantId, long ActorId)> ResolveTenantScopeAsync(
        TenantLocationAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accessRequest);

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        return principal.UserType switch
        {
            LoginUserType.Host => await ResolveHostTenantScopeAsync(accessRequest, cancellationToken),
            LoginUserType.TenantEmployee => await ValidateTenantPermissionAsync(accessRequest, cancellationToken),
            _ => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized)
        };
    }

    private async Task<(long TenantId, long ActorId)> ResolveHostTenantScopeAsync(
        TenantLocationAccessRequestDTO accessRequest,
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

        return (tenantId, hostContext.HostUserId);
    }

    /// <summary>
    /// Resolves the list scope. The current Host Admin role is permitted to retrieve
    /// all live Tenant locations, while normal Host and Tenant users remain Tenant-scoped.
    /// </summary>
    /// <param name="accessRequest">The request containing Host or Tenant permission context.</param>
    /// <param name="cancellationToken">The token used to cancel authorization work.</param>
    /// <returns>The selected Tenant identifier, or <see langword="null"/> for a Host Admin global list.</returns>
    protected async Task<long?> ResolveLocationListTenantScopeAsync(
        TenantLocationAccessRequestDTO accessRequest,
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

            if (hostContext.CurrentHostRoleId == AppConstants.SuperAdminHostRoleId)
            {
                return null;
            }

            return HostTenantIdentifierProtector.Decrypt(
                accessRequest.TenantId,
                hostContext.TenantEncryptionKey,
                _idEncoderService);
        }

        if (principal.UserType == LoginUserType.TenantEmployee)
        {
            var (tenantId, _) = await ValidateTenantPermissionAsync(accessRequest, cancellationToken);
            return tenantId;
        }

        throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
    }
}

/// <summary>Handles creation of Tenant-owned work locations.</summary>
public sealed class CreateTenantLocationCommandHandler : TenantLocationAccessHandlerBase, IRequestHandler<CreateTenantLocationCommand, ApiResponse<TenantLocationResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public CreateTenantLocationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Creates the validated Tenant location.</summary>
    /// <param name="request">The creation command.</param><param name="cancellationToken">Cancellation token.</param><returns>The created location.</returns>
    public async Task<ApiResponse<TenantLocationResponseDTO>> Handle(CreateTenantLocationCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        Validate(request.DTO);
        if (!await UnitOfWork.TenantLocationRepository.IsValidGeographyAsync(request.DTO.CountryId, request.DTO.StateId, request.DTO.CityId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.TenantLocationRepository.LocationCodeExistsAsync(tenantId, request.DTO.LocationCode.Trim(), null, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantLocationCode);
        var entity = _mapper.Map<TenantLocation>(request.DTO);
        entity.LocationCode = request.DTO.LocationCode.Trim(); entity.LocationName = request.DTO.LocationName.Trim(); entity.TimeZoneId = request.DTO.TimeZoneId.Trim();
        entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.TenantLocationRepository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Tenant location {TenantLocationId} created for Tenant {TenantId} by Employee {EmployeeId}.", entity.Id, tenantId, actorId);
        return ApiResponse<TenantLocationResponseDTO>.Success(_mapper.Map<TenantLocationResponseDTO>((await UnitOfWork.TenantLocationRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.TenantLocationCreated);
    }
    #endregion
    private static void Validate(CreateTenantLocationRequestDTO dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.LocationCode) || string.IsNullOrWhiteSpace(dto.LocationName) || string.IsNullOrWhiteSpace(dto.TimeZoneId)) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        if (!Enum.IsDefined(dto.LocationType)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles updates to Tenant-owned work locations.</summary>
public sealed class UpdateTenantLocationCommandHandler : TenantLocationAccessHandlerBase, IRequestHandler<UpdateTenantLocationCommand, ApiResponse<TenantLocationResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateTenantLocationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Updates the validated Tenant location.</summary>
    /// <param name="request">The update command.</param><param name="cancellationToken">Cancellation token.</param><returns>The updated location.</returns>
    public async Task<ApiResponse<TenantLocationResponseDTO>> Handle(UpdateTenantLocationCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        Validate(request.DTO);
        var entity = await UnitOfWork.TenantLocationRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantLocationNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.TenantLocationRepository.HasLiveActiveDependenciesAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantLocationInUse);
        if (!await UnitOfWork.TenantLocationRepository.IsValidGeographyAsync(request.DTO.CountryId, request.DTO.StateId, request.DTO.CityId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.TenantLocationRepository.LocationCodeExistsAsync(tenantId, request.DTO.LocationCode.Trim(), entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantLocationCode);
        _mapper.Map(request.DTO, entity); entity.LocationCode = request.DTO.LocationCode.Trim(); entity.LocationName = request.DTO.LocationName.Trim(); entity.TimeZoneId = request.DTO.TimeZoneId.Trim(); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Tenant location {TenantLocationId} updated for Tenant {TenantId} by Employee {EmployeeId}.", entity.Id, tenantId, actorId);
        return ApiResponse<TenantLocationResponseDTO>.Success(_mapper.Map<TenantLocationResponseDTO>((await UnitOfWork.TenantLocationRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.TenantLocationUpdated);
    }
    #endregion
    private static void Validate(CreateTenantLocationRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.LocationCode) || string.IsNullOrWhiteSpace(dto.LocationName) || string.IsNullOrWhiteSpace(dto.TimeZoneId) || !Enum.IsDefined(dto.LocationType)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles safe soft deletion of Tenant-owned work locations.</summary>
public sealed class DeleteTenantLocationCommandHandler : TenantLocationAccessHandlerBase, IRequestHandler<DeleteTenantLocationCommand, ApiResponse<bool>>
{
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public DeleteTenantLocationCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) { }
    #endregion
    #region Handle
    /// <summary>Soft deletes an unused Tenant location.</summary>
    /// <param name="request">The deletion command.</param><param name="cancellationToken">Cancellation token.</param><returns>A successful deletion acknowledgement.</returns>
    public async Task<ApiResponse<bool>> Handle(DeleteTenantLocationCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantLocationRepository.GetForUpdateAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantLocationNotFound);
        if (await UnitOfWork.TenantLocationRepository.HasAnyDependenciesAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantLocationInUse);
        entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Tenant location {TenantLocationId} soft deleted for Tenant {TenantId} by Employee {EmployeeId}.", entity.Id, tenantId, actorId);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantLocationDeleted);
    }
    #endregion
}

/// <summary>Handles active-state changes to Tenant-owned work locations.</summary>
public sealed class UpdateTenantLocationStatusCommandHandler : TenantLocationAccessHandlerBase, IRequestHandler<UpdateTenantLocationStatusCommand, ApiResponse<TenantLocationResponseDTO>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateTenantLocationStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Changes the active state after dependency validation.</summary>
    /// <param name="request">The status command.</param><param name="cancellationToken">Cancellation token.</param><returns>The location after the status change.</returns>
    public async Task<ApiResponse<TenantLocationResponseDTO>> Handle(UpdateTenantLocationStatusCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var entity = await UnitOfWork.TenantLocationRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantLocationNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.TenantLocationRepository.HasLiveActiveDependenciesAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.TenantLocationInUse);
        entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<TenantLocationResponseDTO>.Success(_mapper.Map<TenantLocationResponseDTO>((await UnitOfWork.TenantLocationRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.TenantLocationStatusUpdated);
    }
    #endregion
}

/// <summary>Handles retrieval of a Tenant-owned work location.</summary>
public sealed class GetTenantLocationByIdQueryHandler : TenantLocationAccessHandlerBase, IRequestHandler<GetTenantLocationByIdQuery, ApiResponse<TenantLocationResponseDTO>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetTenantLocationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Retrieves one Tenant-owned location.</summary>
    /// <param name="request">The identifier query.</param><param name="cancellationToken">Cancellation token.</param><returns>The requested location.</returns>
    public async Task<ApiResponse<TenantLocationResponseDTO>> Handle(GetTenantLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var entity = await UnitOfWork.TenantLocationRepository.GetByIdAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantLocationNotFound);
        return ApiResponse<TenantLocationResponseDTO>.Success(_mapper.Map<TenantLocationResponseDTO>(entity), AppConstants.SuccessMessages.TenantLocationRetrieved);
    }
    #endregion
}

/// <summary>Handles filtered retrieval of Tenant-owned work locations.</summary>
public sealed class GetTenantLocationsQueryHandler : TenantLocationAccessHandlerBase, IRequestHandler<GetTenantLocationsQuery, ApiResponse<List<TenantLocationResponseDTO>>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetTenantLocationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, idEncoderService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Retrieves a database-paged Tenant location list.</summary>
    /// <param name="request">The filter query.</param><param name="cancellationToken">Cancellation token.</param><returns>A flattened paginated location response.</returns>
    public async Task<ApiResponse<List<TenantLocationResponseDTO>>> Handle(GetTenantLocationsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new TenantLocationFilterRequestDTO();
        var tenantId = await ResolveLocationListTenantScopeAsync(filter, cancellationToken);
        var page = tenantId.HasValue
            ? await UnitOfWork.TenantLocationRepository.GetPagedAsync(tenantId.Value, filter, cancellationToken)
            : await UnitOfWork.TenantLocationRepository.GetHostPagedAsync(filter, cancellationToken);

        return Paged(page.Data.Select(entity => _mapper.Map<TenantLocationResponseDTO>(entity)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, AppConstants.SuccessMessages.TenantLocationRetrieved);
    }
    #endregion
}

#endregion
