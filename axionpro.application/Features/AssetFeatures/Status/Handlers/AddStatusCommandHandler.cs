// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant-owned asset statuses from authenticated requests.
// ================================================================

using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers;

#region Command

/// <summary>
/// Represents the request to create an asset status.
/// </summary>
public class AddStatusCommand : IRequest<ApiResponse<GetStatusResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddStatusCommand"/> class.
    /// </summary>
    public AddStatusCommand(CreateStatusRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the client-supplied asset status values.
    /// </summary>
    public CreateStatusRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles creation of tenant-owned asset statuses.
/// </summary>
public class AddStatusCommandHandler : IRequestHandler<AddStatusCommand, ApiResponse<GetStatusResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AddStatusCommandHandler"/> class.
    /// </summary>
    public AddStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<GetStatusResponseDTO>> Handle(
        AddStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null)
        {
            throw new ValidationErrorException(
                "Invalid request data.",
                new List<string> { "Request DTO is required." });
        }

        if (string.IsNullOrWhiteSpace(request.DTO.StatusName)
            || string.IsNullOrWhiteSpace(request.DTO.ColorKey))
        {
            throw new ValidationErrorException(
                "StatusName and ColorKey are required.",
                new List<string> { "StatusName and ColorKey cannot be empty." });
        }

        // Resolve the trusted tenant-user context.
        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        // Map client-editable values and apply server-controlled context.
        var entity = _mapper.Map<AssetStatus>(request.DTO);
        entity.TenantId = validation.TenantId;
        entity.IsActive = true;
        entity.IsSoftDeleted = false;
        entity.AddedById = validation.LoggedInEmployeeId;
        entity.AddedDateTime = DateTime.UtcNow;

        var createdEntity = await _unitOfWork.AssetStatusRepository.CreateAsync(entity, cancellationToken);
        if (createdEntity is null)
        {
            throw new ApiException("Failed to create asset status.", 500);
        }

        return ApiResponse<GetStatusResponseDTO>.Success(
            _mapper.Map<GetStatusResponseDTO>(createdEntity),
            "Asset Status created successfully.");
    }

    #endregion
}

#endregion
