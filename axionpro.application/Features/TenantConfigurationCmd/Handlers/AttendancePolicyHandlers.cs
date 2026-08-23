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
        var (tenantId, actorId) = await ValidateTenantAsync();
        Validate(request.DTO);
        if (!await UnitOfWork.AttendancePolicyRepository.IsEligiblePolicyTypeAsync(tenantId, request.DTO.PolicyTypeId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.AttendancePolicyRepository.PolicyNameExistsAsync(tenantId, request.DTO.PolicyName.Trim(), null, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateAttendancePolicyName);
        var entity = _mapper.Map<AttendancePolicy>(request.DTO); entity.PolicyName = request.DTO.PolicyName.Trim(); entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.AttendancePolicyRepository.AddAsync(entity, cancellationToken); await UnitOfWork.SaveChangesAsync(cancellationToken);
        Logger.LogInformation("Attendance policy {AttendancePolicyId} created for Tenant {TenantId} by Employee {EmployeeId}.", entity.Id, tenantId, actorId);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyCreated);
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
        var (tenantId, actorId) = await ValidateTenantAsync();
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        Validate(request.DTO);
        var entity = await UnitOfWork.AttendancePolicyRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.AttendancePolicyRepository.HasActiveWorkArrangementsAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyInUse);
        if (!await UnitOfWork.AttendancePolicyRepository.IsEligiblePolicyTypeAsync(tenantId, request.DTO.PolicyTypeId, cancellationToken)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.AttendancePolicyRepository.PolicyNameExistsAsync(tenantId, request.DTO.PolicyName.Trim(), entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateAttendancePolicyName);
        _mapper.Map(request.DTO, entity); entity.PolicyName = request.DTO.PolicyName.Trim(); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyUpdated);
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
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateAttendancePolicyStatusCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    #endregion
    #region Handle
    /// <summary>Changes policy state after dependency validation.</summary>
    /// <param name="request">The status command.</param><param name="cancellationToken">Cancellation token.</param><returns>The policy after the change.</returns>
    public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(UpdateAttendancePolicyStatusCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantAsync();
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var entity = await UnitOfWork.AttendancePolicyRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        if (!request.DTO.IsActive && entity.IsActive && await UnitOfWork.AttendancePolicyRepository.HasActiveWorkArrangementsAsync(tenantId, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyInUse);
        entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse((await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken))!), AppConstants.SuccessMessages.AttendancePolicyStatusUpdated);
    }
    #endregion
}

/// <summary>Handles retrieval of a Tenant-owned attendance policy.</summary>
public sealed class GetAttendancePolicyByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetAttendancePolicyByIdQuery, ApiResponse<AttendancePolicyResponseDTO>>
{
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetAttendancePolicyByIdQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    #endregion
    #region Handle
    /// <summary>Retrieves one Tenant-owned attendance policy.</summary>
    /// <param name="request">The identifier query.</param><param name="cancellationToken">Cancellation token.</param><returns>The policy.</returns>
    public async Task<ApiResponse<AttendancePolicyResponseDTO>> Handle(GetAttendancePolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ValidateTenantAsync();
        var entity = await UnitOfWork.AttendancePolicyRepository.GetByIdAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.AttendancePolicyNotFound);
        return ApiResponse<AttendancePolicyResponseDTO>.Success(TenantConfigurationResponseMapper.ToResponse(entity), AppConstants.SuccessMessages.AttendancePolicyRetrieved);
    }
    #endregion
}

/// <summary>Handles filtered retrieval of Tenant-owned attendance policies.</summary>
public sealed class GetAttendancePoliciesQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetAttendancePoliciesQuery, ApiResponse<List<AttendancePolicyResponseDTO>>>
{
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public GetAttendancePoliciesQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    #endregion
    #region Handle
    /// <summary>Retrieves a database-paged attendance-policy list.</summary>
    /// <param name="request">The filter query.</param><param name="cancellationToken">Cancellation token.</param><returns>A flattened paginated policy response.</returns>
    public async Task<ApiResponse<List<AttendancePolicyResponseDTO>>> Handle(GetAttendancePoliciesQuery request, CancellationToken cancellationToken)
    {
        var (tenantId, _) = await ValidateTenantAsync();
        var page = await UnitOfWork.AttendancePolicyRepository.GetPagedAsync(tenantId, request.Filter ?? new AttendancePolicyFilterRequestDTO(), cancellationToken);
        return Paged(page.Data.Select(TenantConfigurationResponseMapper.ToResponse).ToList(), page.PageNumber, page.PageSize, page.TotalCount, AppConstants.SuccessMessages.AttendancePolicyRetrieved);
    }
    #endregion
}

#endregion
