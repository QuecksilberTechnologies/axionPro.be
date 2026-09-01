// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant-owned executable attendance policies.
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

/// <summary>Creates a Tenant-owned attendance policy.</summary>
public sealed class CreateAttendancePolicyCommand : IRequest<ApiResponse<AttendancePolicyResponseDTO>>
{
    /// <summary>Initializes the command with client-editable policy values.</summary>
    public CreateAttendancePolicyCommand(CreateAttendancePolicyRequestDTO dto) => DTO = dto;
    /// <summary>Gets the policy values to create.</summary>
    public CreateAttendancePolicyRequestDTO DTO { get; }
}

/// <summary>Updates a Tenant-owned attendance policy.</summary>
public sealed class UpdateAttendancePolicyCommand : IRequest<ApiResponse<AttendancePolicyResponseDTO>>
{
    /// <summary>Initializes the command with client-editable policy values.</summary>
    public UpdateAttendancePolicyCommand(UpdateAttendancePolicyRequestDTO dto) => DTO = dto;
    /// <summary>Gets the policy values to update.</summary>
    public UpdateAttendancePolicyRequestDTO DTO { get; }
}

/// <summary>Soft deletes a Tenant-owned attendance policy.</summary>
public sealed class DeleteAttendancePolicyCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>Initializes the command with the target policy identifier.</summary>
    public DeleteAttendancePolicyCommand(int id) => Id = id;
    /// <summary>Gets the target policy identifier.</summary>
    public int Id { get; }
}

/// <summary>Changes a Tenant-owned attendance policy active state.</summary>
public sealed class UpdateAttendancePolicyStatusCommand : IRequest<ApiResponse<AttendancePolicyResponseDTO>>
{
    /// <summary>Initializes the command with the desired policy status.</summary>
    public UpdateAttendancePolicyStatusCommand(UpdateAttendancePolicyStatusRequestDTO dto) => DTO = dto;
    /// <summary>Gets the requested policy status change.</summary>
    public UpdateAttendancePolicyStatusRequestDTO DTO { get; }
}

#endregion

#region Query

/// <summary>Retrieves one Tenant-owned attendance policy.</summary>
public sealed class GetAttendancePolicyByIdQuery : IRequest<ApiResponse<AttendancePolicyResponseDTO>>
{
    /// <summary>Initializes the query with the policy identifier.</summary>
    public GetAttendancePolicyByIdQuery(int id) => Id = id;
    /// <summary>Gets the requested policy identifier.</summary>
    public int Id { get; }
}

/// <summary>Retrieves filtered and paginated Tenant-owned attendance policies.</summary>
public sealed class GetAttendancePoliciesQuery : IRequest<ApiResponse<List<AttendancePolicyResponseDTO>>>
{
    /// <summary>Initializes the query with policy filters.</summary>
    public GetAttendancePoliciesQuery(AttendancePolicyFilterRequestDTO filter) => Filter = filter;
    /// <summary>Gets the filters and paging request.</summary>
    public AttendancePolicyFilterRequestDTO Filter { get; }
}

#endregion

#region Handler

/// <summary>Handles Tenant-owned attendance-policy creation.</summary>
public sealed class CreateAttendancePolicyCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateAttendancePolicyCommand, ApiResponse<AttendancePolicyResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public CreateAttendancePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Creates a validated attendance policy.</summary>
    /// <param name="request">The creation command.</param><param name="cancellationToken">Cancellation token.</param><returns>The created attendance policy.</returns>
    public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(CreateAttendancePolicyCommand request, CancellationToken cancellationToken)
    {
        #region Tenant Request Validation

        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context while creating Attendance Policy. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, actorId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        Validate(request.DTO);

        #region Runtime Permission Validation

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                request.DTO.ModuleId,
                request.DTO.OperationId,
                cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        #endregion
        if (!await UnitOfWork.AttendancePolicyRepository.IsEligiblePolicyTypeAsync(tenantId, request.DTO.PolicyTypeId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.AttendancePolicyRepository.PolicyNameExistsAsync(tenantId, request.DTO.PolicyName.Trim(), null, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateAttendancePolicyName);
        var entity = _mapper.Map<AttendancePolicy>(request.DTO); entity.PolicyName = request.DTO.PolicyName.Trim(); entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.AttendancePolicyRepository.AddAsync(entity, cancellationToken); await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Attendance policy {AttendancePolicyId} created for Tenant {TenantId} by Employee {EmployeeId}.", entity.Id, tenantId, actorId);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(_mapper.Map<AttendancePolicyResponseDTO>((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyCreated);
    }
    #endregion
    private static void Validate(CreateAttendancePolicyRequestDTO dto)
    {
        if (dto is null || dto.PolicyTypeId <= 0 || string.IsNullOrWhiteSpace(dto.PolicyName) || !Enum.IsDefined(dto.AttendanceLocationScope)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles Tenant-owned attendance-policy updates.</summary>
public sealed class UpdateAttendancePolicyCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateAttendancePolicyCommand, ApiResponse<AttendancePolicyResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateAttendancePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Updates a validated attendance policy.</summary>
    /// <param name="request">The update command.</param><param name="cancellationToken">Cancellation token.</param><returns>The updated attendance policy.</returns>
public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(UpdateAttendancePolicyCommand request, CancellationToken cancellationToken)
    {
        #region Tenant Request Validation

        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context while updating Attendance Policy. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, actorId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);

        #region Runtime Permission Validation

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                request.DTO.ModuleId,
                request.DTO.OperationId,
                cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        #endregion

        Validate(request.DTO);
        var entity = await UnitOfWork.AttendancePolicyRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.AttendancePolicyRepository.HasActiveWorkArrangementsAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyInUse);
        if (!await UnitOfWork.AttendancePolicyRepository.IsEligiblePolicyTypeAsync(tenantId, request.DTO.PolicyTypeId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.AttendancePolicyRepository.PolicyNameExistsAsync(tenantId, request.DTO.PolicyName.Trim(), entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateAttendancePolicyName);
        _mapper.Map(request.DTO, entity); entity.PolicyName = request.DTO.PolicyName.Trim(); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(_mapper.Map<AttendancePolicyResponseDTO>((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyUpdated);
    }
    #endregion
    private static void Validate(CreateAttendancePolicyRequestDTO dto)
    {
        if (dto.PolicyTypeId <= 0 || string.IsNullOrWhiteSpace(dto.PolicyName) || !Enum.IsDefined(dto.AttendanceLocationScope)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles safe soft deletion of Tenant-owned attendance policies.</summary>
public sealed class DeleteAttendancePolicyCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteAttendancePolicyCommand, ApiResponse<bool>>
{
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public DeleteAttendancePolicyCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    #endregion
    #region Handle
    /// <summary>Soft deletes an unreferenced attendance policy.</summary>
    /// <param name="request">The deletion command.</param><param name="cancellationToken">Cancellation token.</param><returns>A deletion acknowledgement.</returns>
    public async Task<ApiResponse<bool>> Handle(DeleteAttendancePolicyCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantAsync();
        var entity = await UnitOfWork.AttendancePolicyRepository.GetForUpdateAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        if (await UnitOfWork.AttendancePolicyRepository.HasAnyWorkArrangementsAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyInUse);
        entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.AttendancePolicyDeleted);
    }
    #endregion
}

/// <summary>Handles active-state changes to Tenant-owned attendance policies.</summary>
public sealed class UpdateAttendancePolicyStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateAttendancePolicyStatusCommand, ApiResponse<AttendancePolicyResponseDTO>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateAttendancePolicyStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Changes policy state after dependency validation.</summary>
    /// <param name="request">The status command.</param><param name="cancellationToken">Cancellation token.</param><returns>The policy after the change.</returns>
public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(UpdateAttendancePolicyStatusCommand request, CancellationToken cancellationToken)
    {
        #region Tenant Request Validation

        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context while changing Attendance Policy status. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, actorId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);

        #region Runtime Permission Validation

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                request.DTO.ModuleId,
                request.DTO.OperationId,
                cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        #endregion

        var entity = await UnitOfWork.AttendancePolicyRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.AttendancePolicyRepository.HasActiveWorkArrangementsAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyInUse);
        entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(_mapper.Map<AttendancePolicyResponseDTO>((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyStatusUpdated);
    }
    #endregion
}

/// <summary>Handles retrieval of a Tenant-owned attendance policy.</summary>
public sealed class GetAttendancePolicyByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetAttendancePolicyByIdQuery, ApiResponse<AttendancePolicyResponseDTO>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetAttendancePolicyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Retrieves one Tenant-owned attendance policy.</summary>
    /// <param name="request">The identifier query.</param><param name="cancellationToken">Cancellation token.</param><returns>The policy.</returns>
    public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(GetAttendancePolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ValidateTenantAsync();
        var entity = await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(_mapper.Map<AttendancePolicyResponseDTO>(entity), AppConstants.SuccessMessages.AttendancePolicyRetrieved);
    }
    #endregion
}

/// <summary>Handles filtered retrieval of Tenant-owned attendance policies.</summary>
public sealed class GetAttendancePoliciesQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetAttendancePoliciesQuery, ApiResponse<List<AttendancePolicyResponseDTO>>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetAttendancePoliciesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    #endregion
    #region Handle
    /// <summary>Retrieves a database-paged attendance-policy list.</summary>
    /// <param name="request">The filter query.</param><param name="cancellationToken">Cancellation token.</param><returns>A flattened paginated policy response.</returns>
public async Task<ApiResponse<List<AttendancePolicyResponseDTO>>> Handle(GetAttendancePoliciesQuery request, CancellationToken cancellationToken)
    {
        #region Tenant Request Validation

        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context while retrieving Attendance Policies. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, actorId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        var filter = request.Filter ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);

        #region Runtime Permission Validation

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                filter.ModuleId,
                filter.OperationId,
                cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        #endregion

        var page = await UnitOfWork.AttendancePolicyRepository.GetPagedAsync(tenantId, filter, cancellationToken);
        return Paged(page.Data.Select(entity => _mapper.Map<AttendancePolicyResponseDTO>(entity)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, AppConstants.SuccessMessages.AttendancePolicyRetrieved);
    }
    #endregion
}

#endregion
