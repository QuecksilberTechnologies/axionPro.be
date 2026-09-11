// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates, retrieves, updates, and soft deletes tenant EmployeeTypes.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeTypeCmd.Handlers;

#region Commands and Queries

/// <summary>
/// Represents a request to create a tenant-owned EmployeeType.
/// </summary>
public sealed class CreateEmployeeTypeCommand : IRequest<ApiResponse<GetEmployeeTypeResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEmployeeTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The EmployeeType values to create.</param>
    public CreateEmployeeTypeCommand(CreateEmployeeTypeDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the EmployeeType values to create.
    /// </summary>
    public CreateEmployeeTypeDTO DTO { get; }
}

/// <summary>
/// Represents a request to retrieve tenant-owned EmployeeTypes.
/// </summary>
public sealed class GetEmployeeTypesQuery : IRequest<ApiResponse<List<GetEmployeeTypeResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeTypesQuery"/> class.
    /// </summary>
    /// <param name="dto">The paging and permission request values.</param>
    public GetEmployeeTypesQuery(GetEmployeeTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the paging and permission request values.
    /// </summary>
    public GetEmployeeTypeRequestDTO DTO { get; }
}

/// <summary>
/// Represents a request to update a tenant-owned EmployeeType.
/// </summary>
public sealed class UpdateEmployeeTypeCommand : IRequest<ApiResponse<GetEmployeeTypeResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEmployeeTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The EmployeeType values to update.</param>
    public UpdateEmployeeTypeCommand(UpdateEmployeeTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the EmployeeType values to update.
    /// </summary>
    public UpdateEmployeeTypeRequestDTO DTO { get; }
}

/// <summary>
/// Represents a request to soft delete a tenant-owned EmployeeType.
/// </summary>
public sealed class DeleteEmployeeTypeCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteEmployeeTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The EmployeeType deletion request.</param>
    public DeleteEmployeeTypeCommand(DeleteEmployeeTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the EmployeeType deletion request.
    /// </summary>
    public DeleteEmployeeTypeRequestDTO DTO { get; }
}

#endregion

#region Handlers

/// <summary>
/// Handles creation of tenant-owned EmployeeTypes.
/// </summary>
public sealed class CreateEmployeeTypeCommandHandler : IRequestHandler<CreateEmployeeTypeCommand, ApiResponse<GetEmployeeTypeResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEmployeeTypeCommandHandler"/> class.
    /// </summary>
    public CreateEmployeeTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Creates one EmployeeType for the authenticated Tenant.
    /// </summary>
    public async Task<ApiResponse<GetEmployeeTypeResponseDTO>> Handle(
        CreateEmployeeTypeCommand request,
        CancellationToken cancellationToken)
    {
        var actor = await EmployeeTypeHandlerRules.GetTrustedTenantActorAsync(_commonRequestService);
        var created = await _unitOfWork.EmployeeTypeRepository.CreateAsync(
            actor.TenantId,
            actor.EmployeeId,
            request.DTO,
            cancellationToken);

        return ApiResponse<GetEmployeeTypeResponseDTO>.Success(created, "EmployeeType created successfully.");
    }

    #endregion
}

/// <summary>
/// Handles paged retrieval of tenant-owned EmployeeTypes.
/// </summary>
public sealed class GetEmployeeTypesQueryHandler : IRequestHandler<GetEmployeeTypesQuery, ApiResponse<List<GetEmployeeTypeResponseDTO>>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeTypesQueryHandler"/> class.
    /// </summary>
    public GetEmployeeTypesQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Retrieves a paged EmployeeType list for the authenticated Tenant.
    /// </summary>
    public async Task<ApiResponse<List<GetEmployeeTypeResponseDTO>>>
        Handle(GetEmployeeTypesQuery request, CancellationToken cancellationToken)
    {
        if (request.DTO.PageNumber < 1 || request.DTO.PageSize is < 1 or > 100)
        {
            throw new ValidationErrorException("PageNumber must be positive; PageSize must be 1 to 100.");
        }

        var actor = await EmployeeTypeHandlerRules.GetTrustedTenantActorAsync(_commonRequestService);
        var all = await _unitOfWork.EmployeeTypeRepository.GetAllAsync(actor.TenantId, cancellationToken);
        var offset = (long)(request.DTO.PageNumber - 1) * request.DTO.PageSize;
        var page = offset >= all.Count
            ? new List<GetEmployeeTypeResponseDTO>()
            : all.Skip((int)offset).Take(request.DTO.PageSize).ToList();
        var response = ApiResponse<List<GetEmployeeTypeResponseDTO>>.Success(
            page,
            AppConstants.SuccessMessages.EmployeeTypesRetrieved);

        response.PageNumber = request.DTO.PageNumber;
        response.PageSize = request.DTO.PageSize;
        response.TotalRecords = all.Count;
        response.TotalPages = (int)Math.Ceiling((double)all.Count / request.DTO.PageSize);
        return response;
    }

    #endregion
}

/// <summary>
/// Handles updates to tenant-owned EmployeeTypes.
/// </summary>
public sealed class UpdateEmployeeTypeCommandHandler : IRequestHandler<UpdateEmployeeTypeCommand, ApiResponse<GetEmployeeTypeResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateEmployeeTypeCommandHandler> _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEmployeeTypeCommandHandler"/> class.
    /// </summary>
    public UpdateEmployeeTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IMapper mapper,
        ILogger<UpdateEmployeeTypeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _mapper = mapper;
        _logger = logger;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Updates an EmployeeType after enforcing authenticated Tenant ownership.
    /// </summary>
    public async Task<ApiResponse<GetEmployeeTypeResponseDTO>> Handle(
        UpdateEmployeeTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var actor = await EmployeeTypeHandlerRules.GetTrustedTenantActorAsync(_commonRequestService);
        var typeName = EmployeeTypeHandlerRules.ValidateAndNormalize(request.DTO);
        var employeeType = await _unitOfWork.EmployeeTypeRepository.GetForUpdateAsync(
            actor.TenantId,
            request.DTO.Id,
            cancellationToken);

        if (employeeType is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        if (await _unitOfWork.EmployeeTypeRepository.NameExistsAsync(
                actor.TenantId,
                typeName,
                employeeType.Id,
                cancellationToken))
        {
            throw new ConflictException("EmployeeType already exists in this tenant.");
        }

        _mapper.Map(request.DTO, employeeType);
        employeeType.TypeName = typeName;
        employeeType.UpdatedById = actor.EmployeeId;
        employeeType.UpdatedDateTime = DateTime.UtcNow;

        var updated = await _unitOfWork.EmployeeTypeRepository.UpdateAsync(employeeType, cancellationToken);
        _logger.LogInformation(
            "EmployeeType updated. EmployeeTypeId: {EmployeeTypeId}, TenantId: {TenantId}, EmployeeId: {EmployeeId}",
            employeeType.Id,
            actor.TenantId,
            actor.EmployeeId);

        return ApiResponse<GetEmployeeTypeResponseDTO>.Success(updated, "EmployeeType updated successfully.");
    }

    #endregion
}

/// <summary>
/// Handles safe soft deletion of tenant-owned EmployeeTypes.
/// </summary>
public sealed class DeleteEmployeeTypeCommandHandler : IRequestHandler<DeleteEmployeeTypeCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly ILogger<DeleteEmployeeTypeCommandHandler> _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteEmployeeTypeCommandHandler"/> class.
    /// </summary>
    public DeleteEmployeeTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        ILogger<DeleteEmployeeTypeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _logger = logger;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Soft deletes an EmployeeType only when no protected dependency exists.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(
        DeleteEmployeeTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var actor = await EmployeeTypeHandlerRules.GetTrustedTenantActorAsync(_commonRequestService);
        var employeeType = await _unitOfWork.EmployeeTypeRepository.GetForUpdateAsync(
            actor.TenantId,
            request.DTO.Id,
            cancellationToken);

        if (employeeType is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        if (await _unitOfWork.EmployeeTypeRepository.HasDeletionDependenciesAsync(
                actor.TenantId,
                employeeType.Id,
                cancellationToken))
        {
            _logger.LogWarning(
                "EmployeeType deletion blocked by dependencies. EmployeeTypeId: {EmployeeTypeId}, TenantId: {TenantId}",
                employeeType.Id,
                actor.TenantId);
            throw new ConflictException(AppConstants.ErrorMessages.EmployeeTypeHasDependencies);
        }

        employeeType.SoftDeletedById = actor.EmployeeId;
        employeeType.SoftDeletedDateTime = DateTime.UtcNow;
        employeeType.UpdatedById = actor.EmployeeId;
        employeeType.UpdatedDateTime = DateTime.UtcNow;

        await _unitOfWork.EmployeeTypeRepository.SoftDeleteAsync(employeeType, cancellationToken);
        _logger.LogInformation(
            "EmployeeType soft deleted. EmployeeTypeId: {EmployeeTypeId}, TenantId: {TenantId}, EmployeeId: {EmployeeId}",
            employeeType.Id,
            actor.TenantId,
            actor.EmployeeId);

        return ApiResponse<bool>.Success(true, "EmployeeType deleted successfully.");
    }

    #endregion
}

#endregion

#region Helpers

internal static class EmployeeTypeHandlerRules
{
    /// <summary>
    /// Resolves the trusted Tenant and actor identifiers from the established request validation flow.
    /// </summary>
    internal static async Task<(long TenantId, long EmployeeId)> GetTrustedTenantActorAsync(
        ICommonRequestService commonRequestService)
    {
        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success || validation.TenantId <= 0 || validation.LoggedInEmployeeId <= 0 || validation.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        return (validation.TenantId, validation.LoggedInEmployeeId);
    }

    /// <summary>
    /// Validates and normalizes the EmployeeType fields shared by creation and update.
    /// </summary>
    internal static string ValidateAndNormalize(CreateEmployeeTypeDTO request)
    {
        var name = request.TypeName?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 255 ||
            request.Description?.Length > 255 || request.Remark?.Length > 255)
        {
            throw new ValidationErrorException(
                "TypeName is required; type name, description and remark must not exceed 255 characters.");
        }

        return name;
    }
}

#endregion
