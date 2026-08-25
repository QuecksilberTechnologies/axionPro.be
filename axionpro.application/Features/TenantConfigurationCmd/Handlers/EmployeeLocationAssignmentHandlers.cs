// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant employee-to-location assignments.
// ================================================================

using AutoMapper;
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
/// <summary>Creates an employee-location assignment.</summary>
public sealed class CreateEmployeeLocationAssignmentCommand(CreateEmployeeLocationAssignmentRequestDTO dto) : IRequest<ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    /// <summary>Gets assignment values.</summary>
    public CreateEmployeeLocationAssignmentRequestDTO DTO { get; } = dto;
}
/// <summary>Updates an employee-location assignment.</summary>
public sealed class UpdateEmployeeLocationAssignmentCommand(UpdateEmployeeLocationAssignmentRequestDTO dto) : IRequest<ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    /// <summary>Gets assignment values.</summary>
    public UpdateEmployeeLocationAssignmentRequestDTO DTO { get; } = dto;
}
/// <summary>Soft deletes an employee-location assignment.</summary>
public sealed class DeleteEmployeeLocationAssignmentCommand(long id) : IRequest<ApiResponse<bool>>
{
    /// <summary>Gets the assignment identifier.</summary>
    public long Id { get; } = id;
}
/// <summary>Changes an employee-location-assignment active state.</summary>
public sealed class UpdateEmployeeLocationAssignmentStatusCommand(UpdateEmployeeLocationAssignmentStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    /// <summary>Gets status values.</summary>
    public UpdateEmployeeLocationAssignmentStatusRequestDTO DTO { get; } = dto;
}
#endregion
#region Query
/// <summary>Retrieves one employee-location assignment.</summary>
public sealed class GetEmployeeLocationAssignmentByIdQuery(long id) : IRequest<ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    /// <summary>Gets the identifier.</summary>
    public long Id { get; } = id;
}
/// <summary>Retrieves filtered employee-location assignments.</summary>
public sealed class GetEmployeeLocationAssignmentsQuery(EmployeeLocationAssignmentFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeLocationAssignmentResponseDTO>>>
{
    /// <summary>Gets filters.</summary>
    public EmployeeLocationAssignmentFilterRequestDTO Filter { get; } = filter;
}
#endregion
#region Handler
/// <summary>Handles employee-location-assignment creation.</summary>
public sealed class CreateEmployeeLocationAssignmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeLocationAssignmentCommand, ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public CreateEmployeeLocationAssignmentCommandHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeLocationAssignmentResponseDTO>> Handle(CreateEmployeeLocationAssignmentCommand request, CancellationToken ct)
    {
        var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, ct); Validate(request.DTO);
        await ValidateReferences(tenantId, request.DTO, null, ct);
        var entity = _mapper.Map<EmployeeLocationAssignment>(request.DTO); entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.EmployeeLocationAssignmentRepository.AddAsync(entity, ct); await UnitOfWork.SaveChangesAsync(ct);
        Logger.LogInformation("Employee location assignment {AssignmentId} created for Tenant {TenantId}.", entity.Id, tenantId);
        return ApiResponse<EmployeeLocationAssignmentResponseDTO>.Success(_mapper.Map<EmployeeLocationAssignmentResponseDTO>((await UnitOfWork.EmployeeLocationAssignmentRepository.GetByIdAsync(tenantId, entity.Id, ct))!), AppConstants.SuccessMessages.EmployeeLocationAssignmentCreated);
    }
    #endregion
    private async Task ValidateReferences(long tenantId, CreateEmployeeLocationAssignmentRequestDTO dto, long? excludeId, CancellationToken ct)
    {
        if (!await UnitOfWork.EmployeeLocationAssignmentRepository.IsEligibleEmployeeAsync(tenantId, dto.EmployeeId, ct) || !await UnitOfWork.EmployeeLocationAssignmentRepository.IsEligibleLocationAsync(tenantId, dto.TenantLocationId, ct)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (dto.IsActive && await UnitOfWork.EmployeeLocationAssignmentRepository.AssignmentExistsAsync(tenantId, dto.EmployeeId, dto.TenantLocationId, excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeLocationAssignment);
        if (dto.IsActive && dto.IsPrimary && await UnitOfWork.EmployeeLocationAssignmentRepository.PrimaryAssignmentExistsAsync(tenantId, dto.EmployeeId, excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasPrimaryLocation);
    }
    private static void Validate(CreateEmployeeLocationAssignmentRequestDTO dto) { if (dto is null || dto.EmployeeId <= 0 || dto.TenantLocationId <= 0 || dto.EffectiveFrom == default || (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidEffectiveDateRange); }
}

/// <summary>Handles employee-location-assignment updates.</summary>
public sealed class UpdateEmployeeLocationAssignmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeLocationAssignmentCommand, ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateEmployeeLocationAssignmentCommandHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeLocationAssignmentResponseDTO>> Handle(UpdateEmployeeLocationAssignmentCommand request, CancellationToken ct)
    {
        var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, ct); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); Validate(request.DTO);
        var entity = await UnitOfWork.EmployeeLocationAssignmentRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeLocationAssignmentNotFound);
        if (!await UnitOfWork.EmployeeLocationAssignmentRepository.IsEligibleEmployeeAsync(tenantId, request.DTO.EmployeeId, ct) || !await UnitOfWork.EmployeeLocationAssignmentRepository.IsEligibleLocationAsync(tenantId, request.DTO.TenantLocationId, ct)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (request.DTO.IsActive && await UnitOfWork.EmployeeLocationAssignmentRepository.AssignmentExistsAsync(tenantId, request.DTO.EmployeeId, request.DTO.TenantLocationId, entity.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeLocationAssignment);
        if (request.DTO.IsActive && request.DTO.IsPrimary && await UnitOfWork.EmployeeLocationAssignmentRepository.PrimaryAssignmentExistsAsync(tenantId, request.DTO.EmployeeId, entity.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasPrimaryLocation);
        _mapper.Map(request.DTO, entity); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct);
        return ApiResponse<EmployeeLocationAssignmentResponseDTO>.Success(_mapper.Map<EmployeeLocationAssignmentResponseDTO>((await UnitOfWork.EmployeeLocationAssignmentRepository.GetByIdAsync(tenantId, entity.Id, ct))!), AppConstants.SuccessMessages.EmployeeLocationAssignmentUpdated);
    }
    #endregion
    private static void Validate(CreateEmployeeLocationAssignmentRequestDTO dto) { if (dto.EmployeeId <= 0 || dto.TenantLocationId <= 0 || dto.EffectiveFrom == default || (dto.EffectiveTo.HasValue && dto.EffectiveTo < dto.EffectiveFrom)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidEffectiveDateRange); }
}

/// <summary>Handles employee-location-assignment soft deletion.</summary>
public sealed class DeleteEmployeeLocationAssignmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeLocationAssignmentCommand, ApiResponse<bool>>
{
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public DeleteEmployeeLocationAssignmentCommandHandler(IUnitOfWork u, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) { }
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeLocationAssignmentCommand request, CancellationToken ct) { var (tenantId, actorId) = await ValidateTenantAsync(); var entity = await UnitOfWork.EmployeeLocationAssignmentRepository.GetForUpdateAsync(tenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeLocationAssignmentNotFound); entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeLocationAssignmentDeleted); }
    #endregion
}

/// <summary>Handles employee-location-assignment status changes.</summary>
public sealed class UpdateEmployeeLocationAssignmentStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeLocationAssignmentStatusCommand, ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    private readonly IMapper _mapper;
    #region Constructor
    /// <summary>Initializes the handler.</summary>
    public UpdateEmployeeLocationAssignmentStatusCommandHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeLocationAssignmentResponseDTO>> Handle(UpdateEmployeeLocationAssignmentStatusCommand request, CancellationToken ct)
    {
        var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, ct); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var entity = await UnitOfWork.EmployeeLocationAssignmentRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeLocationAssignmentNotFound);
        if (request.DTO.IsActive && entity.IsPrimary && await UnitOfWork.EmployeeLocationAssignmentRepository.PrimaryAssignmentExistsAsync(tenantId, entity.EmployeeId, entity.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.EmployeeAlreadyHasPrimaryLocation);
        if (request.DTO.IsActive && await UnitOfWork.EmployeeLocationAssignmentRepository.AssignmentExistsAsync(tenantId, entity.EmployeeId, entity.TenantLocationId, entity.Id, ct)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeLocationAssignment);
        entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<EmployeeLocationAssignmentResponseDTO>.Success(_mapper.Map<EmployeeLocationAssignmentResponseDTO>((await UnitOfWork.EmployeeLocationAssignmentRepository.GetByIdAsync(tenantId, entity.Id, ct))!), AppConstants.SuccessMessages.EmployeeLocationAssignmentStatusUpdated);
    }
    #endregion
}

/// <summary>Handles retrieval of employee-location assignments.</summary>
public sealed class GetEmployeeLocationAssignmentByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeLocationAssignmentByIdQuery, ApiResponse<EmployeeLocationAssignmentResponseDTO>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes the handler.</summary>
    public GetEmployeeLocationAssignmentByIdQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeLocationAssignmentResponseDTO>> Handle(GetEmployeeLocationAssignmentByIdQuery request, CancellationToken ct) { var (tenantId, _) = await ValidateTenantAsync(); var entity = await UnitOfWork.EmployeeLocationAssignmentRepository.GetByIdAsync(tenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeLocationAssignmentNotFound); return ApiResponse<EmployeeLocationAssignmentResponseDTO>.Success(_mapper.Map<EmployeeLocationAssignmentResponseDTO>(entity)); }
}

/// <summary>Handles filtered retrieval of employee-location assignments.</summary>
public sealed class GetEmployeeLocationAssignmentsQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeLocationAssignmentsQuery, ApiResponse<List<EmployeeLocationAssignmentResponseDTO>>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes the handler.</summary>
    public GetEmployeeLocationAssignmentsQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<List<EmployeeLocationAssignmentResponseDTO>>> Handle(GetEmployeeLocationAssignmentsQuery request, CancellationToken ct) { var (tenantId, _) = await ValidateTenantPermissionAsync(request.Filter ?? new EmployeeLocationAssignmentFilterRequestDTO(), ct); var page = await UnitOfWork.EmployeeLocationAssignmentRepository.GetPagedAsync(tenantId, request.Filter ?? new EmployeeLocationAssignmentFilterRequestDTO(), ct); return Paged(page.Data.Select(entity => _mapper.Map<EmployeeLocationAssignmentResponseDTO>(entity)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, "Employee location assignments retrieved successfully."); }
}
#endregion
