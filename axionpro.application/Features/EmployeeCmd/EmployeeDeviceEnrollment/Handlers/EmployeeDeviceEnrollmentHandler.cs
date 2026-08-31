// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Validates and manages Tenant employee enrollments on Host-managed devices.
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
using EmployeeDeviceEnrollmentEntity = axionpro.domain.Entity.EmployeeDeviceEnrollment;

namespace axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment.Handlers;

#region Command
/// <summary>Creates an employee device enrollment.</summary>
public sealed class CreateEmployeeDeviceEnrollmentCommand(CreateEmployeeDeviceEnrollmentRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    /// <summary>Gets enrollment values.</summary>
    public CreateEmployeeDeviceEnrollmentRequestDTO DTO { get; } = dto;
}
/// <summary>Updates an employee device enrollment.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentCommand(UpdateEmployeeDeviceEnrollmentRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    /// <summary>Gets enrollment values.</summary>
    public UpdateEmployeeDeviceEnrollmentRequestDTO DTO { get; } = dto;
}
/// <summary>Soft deletes an employee device enrollment.</summary>
public sealed class DeleteEmployeeDeviceEnrollmentCommand(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<bool>>
{
    /// <summary>Gets identifier.</summary>
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Changes an employee device enrollment active state.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentStatusCommand(UpdateEmployeeDeviceEnrollmentStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    /// <summary>Gets status values.</summary>
    public UpdateEmployeeDeviceEnrollmentStatusRequestDTO DTO { get; } = dto;
}
#endregion
#region Query
/// <summary>Retrieves one employee device enrollment.</summary>
public sealed class GetEmployeeDeviceEnrollmentByIdQuery(long id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    /// <summary>Gets identifier.</summary>
    public long Id { get; } = id;
    /// <summary>Gets the module and operation required for tenant-role authorization.</summary>
    public PermissionRequestDTO PermissionRequest { get; } = permissionRequest;
}
/// <summary>Retrieves filtered employee device enrollments.</summary>
public sealed class GetEmployeeDeviceEnrollmentsQuery(EmployeeDeviceEnrollmentFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>>
{
    /// <summary>Gets filters.</summary>
    public EmployeeDeviceEnrollmentFilterRequestDTO Filter { get; } = filter;
}
#endregion
#region Handler
/// <summary>Handles employee device enrollment creation.</summary>
public sealed class CreateEmployeeDeviceEnrollmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<CreateEmployeeDeviceEnrollmentCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes handler.</summary>
    public CreateEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = m;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(CreateEmployeeDeviceEnrollmentCommand request, CancellationToken ct)
    {
        if (request.DTO is null) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO); await ValidateRefs(tenantId, employeeId, request.DTO, null, ct);
        var entity = _mapper.Map<EmployeeDeviceEnrollmentEntity>(request.DTO); entity.EmployeeId = employeeId; entity.EnrollId = request.DTO.EnrollId.Trim(); entity.TenantId = tenantId; entity.IsSoftDeleted = false; entity.AddedById = actorId; entity.AddedDateTime = DateTime.UtcNow;
        await UnitOfWork.EmployeeDeviceEnrollmentRepository.AddAsync(entity, ct); await UnitOfWork.SaveChangesAsync(ct);
        Logger.LogInformation("Employee {EmployeeId} enrolled on TenantDevice {TenantDeviceId} for Tenant {TenantId}.", entity.EmployeeId, entity.TenantDeviceId, tenantId);
        return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(_mapper.Map<EmployeeDeviceEnrollmentResponseDTO>((await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(tenantId, entity.Id, ct))!), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentCreated);
    }
    #endregion
    private async Task ValidateRefs(long tenantId, long employeeId, CreateEmployeeDeviceEnrollmentRequestDTO dto, long? excludeId, CancellationToken ct)
    {
        if (!await UnitOfWork.EmployeeDeviceEnrollmentRepository.IsEligibleEmployeeAsync(tenantId, employeeId, ct) || !await UnitOfWork.EmployeeDeviceEnrollmentRepository.IsEligibleTenantDeviceAsync(tenantId, dto.TenantDeviceId, ct)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.EmployeeDeviceEnrollmentRepository.EnrollIdExistsAsync(tenantId, dto.TenantDeviceId, dto.EnrollId.Trim(), excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateDeviceEnrollId);
    }
    private static void Validate(CreateEmployeeDeviceEnrollmentRequestDTO dto) { if (dto is null || string.IsNullOrWhiteSpace(dto.EmployeeId) || dto.TenantDeviceId <= 0 || string.IsNullOrWhiteSpace(dto.EnrollId)) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing); }
}

/// <summary>Handles employee device enrollment updates.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeDeviceEnrollmentCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    #region Fields
    private readonly IMapper _mapper;
    #endregion
    #region Constructor
    /// <summary>Initializes handler.</summary>
    public UpdateEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = m;
    #endregion
    #region Handle
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpdateEmployeeDeviceEnrollmentCommand request, CancellationToken ct)
    {
        if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var (tenantId, actorId, employeeId) = await ValidateTenantAndDecodeEmployeeIdAsync(request.DTO.EmployeeId); Validate(request.DTO);
        var entity = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound);
        if (entity.EmployeeId != employeeId) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        await ValidateRefs(tenantId, employeeId, request.DTO, entity.Id, ct);
        _mapper.Map(request.DTO, entity); entity.EmployeeId = employeeId; entity.EnrollId = request.DTO.EnrollId.Trim(); entity.UpdatedById = actorId; entity.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct);
        return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(_mapper.Map<EmployeeDeviceEnrollmentResponseDTO>((await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(tenantId, entity.Id, ct))!), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentUpdated);
    }
    #endregion
    private async Task ValidateRefs(long tenantId, long employeeId, CreateEmployeeDeviceEnrollmentRequestDTO dto, long? excludeId, CancellationToken ct)
    {
        if (!await UnitOfWork.EmployeeDeviceEnrollmentRepository.IsEligibleEmployeeAsync(tenantId, employeeId, ct) || !await UnitOfWork.EmployeeDeviceEnrollmentRepository.IsEligibleTenantDeviceAsync(tenantId, dto.TenantDeviceId, ct)) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (await UnitOfWork.EmployeeDeviceEnrollmentRepository.EnrollIdExistsAsync(tenantId, dto.TenantDeviceId, dto.EnrollId.Trim(), excludeId, ct)) throw new ConflictException(AppConstants.ErrorMessages.DuplicateDeviceEnrollId);
    }
    private static void Validate(CreateEmployeeDeviceEnrollmentRequestDTO dto) { if (string.IsNullOrWhiteSpace(dto.EmployeeId) || dto.TenantDeviceId <= 0 || string.IsNullOrWhiteSpace(dto.EnrollId)) throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing); }
}

/// <summary>Handles employee device enrollment soft deletion.</summary>
public sealed class DeleteEmployeeDeviceEnrollmentCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<DeleteEmployeeDeviceEnrollmentCommand, ApiResponse<bool>>
{
    /// <summary>Initializes handler.</summary>
    public DeleteEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) { }
    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeDeviceEnrollmentCommand request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(validation.TenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); e.IsSoftDeleted = true; e.IsActive = false; e.SoftDeletedById = validation.LoggedInEmployeeId; e.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeDeviceEnrollmentDeleted); }
}

/// <summary>Handles employee device enrollment status changes.</summary>
public sealed class UpdateEmployeeDeviceEnrollmentStatusCommandHandler : TenantConfigurationHandlerBase, IRequestHandler<UpdateEmployeeDeviceEnrollmentStatusCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public UpdateEmployeeDeviceEnrollmentStatusCommandHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpdateEmployeeDeviceEnrollmentStatusCommand request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var tenantId = validation.TenantId; if (request.DTO is null || request.DTO.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(tenantId, request.DTO.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); e.IsActive = request.DTO.IsActive; e.UpdatedById = validation.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(_mapper.Map<EmployeeDeviceEnrollmentResponseDTO>((await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(tenantId, e.Id, ct))!), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentStatusUpdated); }
}

/// <summary>Handles employee device enrollment retrieval.</summary>
public sealed class GetEmployeeDeviceEnrollmentByIdQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeDeviceEnrollmentByIdQuery, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public GetEmployeeDeviceEnrollmentByIdQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l) : base(u, c, l) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(GetEmployeeDeviceEnrollmentByIdQuery request, CancellationToken ct) { var validation = await ValidateTenantDataAccessContextAsync(); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(validation.TenantId, request.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(validation, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(_mapper.Map<EmployeeDeviceEnrollmentResponseDTO>(e)); }
}

/// <summary>Handles filtered employee device enrollment retrieval.</summary>
public sealed class GetEmployeeDeviceEnrollmentsQueryHandler : TenantConfigurationHandlerBase, IRequestHandler<GetEmployeeDeviceEnrollmentsQuery, ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>>
{
    private readonly IMapper _mapper;
    /// <summary>Initializes handler.</summary>
    public GetEmployeeDeviceEnrollmentsQueryHandler(IUnitOfWork u, IMapper mapper, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService idEncoderService) : base(u, c, l, idEncoderService) => _mapper = mapper;
    /// <inheritdoc />
    public async Task<ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>> Handle(GetEmployeeDeviceEnrollmentsQuery request, CancellationToken ct) { var filter = request.Filter ?? new EmployeeDeviceEnrollmentFilterRequestDTO(); var validation = await ValidateTenantDataAccessContextAsync(); long? employeeId = null; if (!string.IsNullOrWhiteSpace(filter.EmployeeId)) { var context = await ValidateTenantAndDecodeOptionalEmployeeIdAsync(filter.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); employeeId = context.EmployeeId; } filter.ResolvedEmployeeId = employeeId; var p = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetPagedAsync(validation.TenantId, filter, validation.LoggedInEmployeeId, validation.RoleTypeId, ct); return Paged(p.Data.Select(entity => _mapper.Map<EmployeeDeviceEnrollmentResponseDTO>(entity)).ToList(), p.PageNumber, p.PageSize, p.TotalCount, "Employee device enrollments retrieved successfully."); }
}
#endregion
