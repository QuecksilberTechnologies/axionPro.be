// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant-owned asset types from authenticated requests.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers;

#region Command

/// <summary>
/// Represents the request to create an asset type.
/// </summary>
public class AddTypeCommand : IRequest<ApiResponse<List<GetTypeResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The client-supplied asset type values.</param>
    public AddTypeCommand(AddTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the client-supplied asset type values.
    /// </summary>
    public AddTypeRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles creation of tenant-owned asset types.
/// </summary>
public class AddTypeCommandHandler : IRequestHandler<AddTypeCommand, ApiResponse<List<GetTypeResponseDTO>>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddTypeCommandHandler> _logger;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AddTypeCommandHandler"/> class.
    /// </summary>
    public AddTypeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddTypeCommandHandler> logger,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<List<GetTypeResponseDTO>>> Handle(
        AddTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null)
        {
            throw new ValidationErrorException(
                "Invalid request data.",
                new List<string> { "Request DTO is required." });
        }

        if (string.IsNullOrWhiteSpace(request.DTO.TypeName))
        {
            throw new ValidationErrorException(
                "Type Name is required.",
                new List<string> { "TypeName cannot be empty." });
        }

        #region Tenant Request Validation

        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long userEmployeeId = validation.LoggedInEmployeeId;
        long tenantId = validation.TenantId;
        int tokenRoleId = validation.RoleId;

        if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
        {
            _logger.LogWarning(
                "Invalid Tenant authorization context while creating Asset Type. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, userEmployeeId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Runtime Permission Validation

        var permissionResult = await _unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                userEmployeeId,
                tokenRoleId,
                request.DTO.ModuleId,
                request.DTO.OperationId,
                cancellationToken);

        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

        #endregion

        // Map client-editable values and apply server-controlled context.
        var entity = _mapper.Map<AssetType>(request.DTO);
        entity.TenantId = tenantId;
        entity.AddedById = userEmployeeId;
        entity.AddedDateTime = DateTime.UtcNow;
        entity.IsSoftDeleted = false;

        var createdEntity = await _unitOfWork.AssetTypeRepository.CreateAsync(entity, cancellationToken);
        if (createdEntity is null)
        {
            _logger.LogWarning("Asset type creation returned no entity for tenant {TenantId}.", tenantId);
            throw new ApiException("Failed to add asset type.", 500);
        }

        var response = _mapper.Map<GetTypeResponseDTO>(createdEntity);
        return ApiResponse<List<GetTypeResponseDTO>>.Success(
            new List<GetTypeResponseDTO> { response },
            "Asset Type added successfully.");
    }

    #endregion
}

#endregion
