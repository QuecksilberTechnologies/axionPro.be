// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant employee work-pattern day configuration.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo.Handlers;

#region Command

/// <summary>Creates an employee work-pattern day.</summary>
public sealed class CreateEmployeeWorkPatternCommand(CreateEmployeeWorkPatternRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    /// <summary>Gets the client-editable work-pattern values.</summary>
    public CreateEmployeeWorkPatternRequestDTO DTO { get; } = dto;
}

/// <summary>Updates an employee work-pattern day.</summary>
public sealed class UpdateEmployeeWorkPatternCommand(UpdateEmployeeWorkPatternRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    /// <summary>Gets the client-editable work-pattern values.</summary>
    public UpdateEmployeeWorkPatternRequestDTO DTO { get; } = dto;
}

/// <summary>Soft deletes an employee work-pattern day.</summary>
public sealed class DeleteEmployeeWorkPatternCommand(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<bool>>
{
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}

/// <summary>Changes an employee work-pattern active state.</summary>
public sealed class UpdateEmployeeWorkPatternStatusCommand(UpdateEmployeeWorkPatternStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeWorkPatternResponseDTO>> { public UpdateEmployeeWorkPatternStatusRequestDTO DTO { get; } = dto; }

#endregion

#region Query

/// <summary>Retrieves one employee work-pattern day.</summary>
public sealed class GetEmployeeWorkPatternByIdQuery(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}

/// <summary>Retrieves filtered employee work-pattern days.</summary>
public sealed class GetEmployeeWorkPatternsQuery(EmployeeWorkPatternFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeWorkPatternResponseDTO>>> { public EmployeeWorkPatternFilterRequestDTO Filter { get; } = filter; }

#endregion

#region Handler

/// <summary>Handles employee work-pattern creation.</summary>
public sealed class CreateEmployeeWorkPatternCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeWorkPatternCommand, ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    private readonly IMapper _mapper;
    public CreateEmployeeWorkPatternCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkPatternResponseDTO>> Handle(CreateEmployeeWorkPatternCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, cancellationToken); Validate(request.DTO); await ValidateReferencesAsync(tenantId, request.DTO, null, cancellationToken);
        var entity = _mapper.Map<EmployeeWorkPattern>(request.DTO); entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.EmployeeWorkPatternRepository.AddAsync(entity, cancellationToken); await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkPatternRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        return ApiResponse<EmployeeWorkPatternResponseDTO>.Success(_mapper.Map<EmployeeWorkPatternResponseDTO>(stored!), AppConstants.SuccessMessages.EmployeeWorkPatternCreated);
    }
    private async Task ValidateReferencesAsync(long tenantId, CreateEmployeeWorkPatternRequestDTO dto, long? excludeId, CancellationToken cancellationToken)
    {
        if (!await UnitOfWork.EmployeeWorkPatternRepository.IsEligibleArrangementAsync(tenantId, dto.EmployeeWorkArrangementId, cancellationToken) || (dto.TenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkPatternRepository.IsEligibleLocationAsync(tenantId, dto.TenantLocationId.Value, cancellationToken))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (dto.IsActive && await UnitOfWork.EmployeeWorkPatternRepository.PatternDayExistsAsync(tenantId, dto.EmployeeWorkArrangementId, (short)dto.DayOfWeek, excludeId, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeWorkPatternDay);
    }
    private static void Validate(CreateEmployeeWorkPatternRequestDTO dto)
    {
        if (dto is null || dto.EmployeeWorkArrangementId <= 0 || !Enum.IsDefined(dto.DayOfWeek) || !Enum.IsDefined(dto.WorkMode)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles employee work-pattern updates.</summary>
public sealed class UpdateEmployeeWorkPatternCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkPatternCommand, ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    private readonly IMapper _mapper;
    public UpdateEmployeeWorkPatternCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkPatternResponseDTO>> Handle(UpdateEmployeeWorkPatternCommand request, CancellationToken cancellationToken)
    {
        var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, cancellationToken); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); Validate(request.DTO);
        var entity = await UnitOfWork.EmployeeWorkPatternRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkPatternNotFound);
        if (!await UnitOfWork.EmployeeWorkPatternRepository.IsEligibleArrangementAsync(tenantId, request.DTO.EmployeeWorkArrangementId, cancellationToken) || (request.DTO.TenantLocationId.HasValue && !await UnitOfWork.EmployeeWorkPatternRepository.IsEligibleLocationAsync(tenantId, request.DTO.TenantLocationId.Value, cancellationToken))) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (request.DTO.IsActive && await UnitOfWork.EmployeeWorkPatternRepository.PatternDayExistsAsync(tenantId, request.DTO.EmployeeWorkArrangementId, (short)request.DTO.DayOfWeek, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeWorkPatternDay);
        _mapper.Map(request.DTO, entity); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken);
        var stored = await UnitOfWork.EmployeeWorkPatternRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken);
        return ApiResponse<EmployeeWorkPatternResponseDTO>.Success(_mapper.Map<EmployeeWorkPatternResponseDTO>(stored!), AppConstants.SuccessMessages.EmployeeWorkPatternUpdated);
    }
    private static void Validate(CreateEmployeeWorkPatternRequestDTO dto)
    {
        if (dto.EmployeeWorkArrangementId <= 0 || !Enum.IsDefined(dto.DayOfWeek) || !Enum.IsDefined(dto.WorkMode)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
    }
}

/// <summary>Handles employee work-pattern soft deletion.</summary>
public sealed class DeleteEmployeeWorkPatternCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeWorkPatternCommand, ApiResponse<bool>>
{
    public DeleteEmployeeWorkPatternCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) { }
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeWorkPatternCommand request, CancellationToken cancellationToken)
    { var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.PermissionRequest, cancellationToken); var entity = await UnitOfWork.EmployeeWorkPatternRepository.GetForUpdateAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkPatternNotFound); entity.IsSoftDeleted = true; entity.IsActive = false; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeWorkPatternDeleted); }
}

/// <summary>Handles employee work-pattern active-state changes.</summary>
public sealed class UpdateEmployeeWorkPatternStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeWorkPatternStatusCommand, ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    private readonly IMapper _mapper;
    public UpdateEmployeeWorkPatternStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkPatternResponseDTO>> Handle(UpdateEmployeeWorkPatternStatusCommand request, CancellationToken cancellationToken)
    { var (tenantId, actorId) = await ValidateTenantPermissionAsync(request.DTO, cancellationToken); if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var entity = await UnitOfWork.EmployeeWorkPatternRepository.GetForUpdateAsync(tenantId, request.DTO.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkPatternNotFound); if (request.DTO.IsActive && await UnitOfWork.EmployeeWorkPatternRepository.PatternDayExistsAsync(tenantId, entity.EmployeeWorkArrangementId, (short)entity.DayOfWeek, entity.Id, cancellationToken)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmployeeWorkPatternDay); entity.IsActive = request.DTO.IsActive; entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(cancellationToken); var stored = await UnitOfWork.EmployeeWorkPatternRepository.GetByIdAsync(tenantId, entity.Id, cancellationToken); return ApiResponse<EmployeeWorkPatternResponseDTO>.Success(_mapper.Map<EmployeeWorkPatternResponseDTO>(stored!), AppConstants.SuccessMessages.EmployeeWorkPatternStatusUpdated); }
}

/// <summary>Handles employee work-pattern retrieval.</summary>
public sealed class GetEmployeeWorkPatternByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkPatternByIdQuery, ApiResponse<EmployeeWorkPatternResponseDTO>>
{
    private readonly IMapper _mapper;
    public GetEmployeeWorkPatternByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<EmployeeWorkPatternResponseDTO>> Handle(GetEmployeeWorkPatternByIdQuery request, CancellationToken cancellationToken)
    { var (tenantId, _) = await ValidateTenantPermissionAsync(request.PermissionRequest, cancellationToken); var entity = await UnitOfWork.EmployeeWorkPatternRepository.GetByIdAsync(tenantId, request.Id, cancellationToken) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeWorkPatternNotFound); return ApiResponse<EmployeeWorkPatternResponseDTO>.Success(_mapper.Map<EmployeeWorkPatternResponseDTO>(entity)); }
}

/// <summary>Handles paged employee work-pattern retrieval.</summary>
public sealed class GetEmployeeWorkPatternsQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeWorkPatternsQuery, ApiResponse<List<EmployeeWorkPatternResponseDTO>>>
{
    private readonly IMapper _mapper;
    public GetEmployeeWorkPatternsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger) : base(unitOfWork, commonRequestService, logger) => _mapper = mapper;
    public async Task<ApiResponse<List<EmployeeWorkPatternResponseDTO>>> Handle(GetEmployeeWorkPatternsQuery request, CancellationToken cancellationToken)
    { var (tenantId, _) = await ValidateTenantPermissionAsync(request.Filter ?? new EmployeeWorkPatternFilterRequestDTO(), cancellationToken); var page = await UnitOfWork.EmployeeWorkPatternRepository.GetPagedAsync(tenantId, request.Filter ?? new EmployeeWorkPatternFilterRequestDTO(), cancellationToken); return Paged(page.Data.Select(entity => _mapper.Map<EmployeeWorkPatternResponseDTO>(entity)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, "Employee work patterns retrieved successfully."); }
}

#endregion
