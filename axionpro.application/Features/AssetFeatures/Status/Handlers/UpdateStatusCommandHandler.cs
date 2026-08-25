// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-owned asset statuses from authenticated requests.
// ================================================================

using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers;

#region Command

/// <summary>
/// Represents the request to update an asset status.
/// </summary>
public class UpdateStatusCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommand"/> class.
    /// </summary>
    public UpdateStatusCommand(UpdateStatusRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the client-supplied update values.
    /// </summary>
    public UpdateStatusRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles updates to tenant-owned asset statuses.
/// </summary>
public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommandHandler"/> class.
    /// </summary>
    public UpdateStatusCommandHandler(
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
    public async Task<ApiResponse<bool>> Handle(
        UpdateStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(
                "Invalid Status Id.",
                new List<string> { "Status Id must be greater than 0." });
        }

        // Resolve the trusted tenant-user context.
        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        // Load the tenant-owned entity before applying client changes.
        var entity = await _unitOfWork.AssetStatusRepository.GetByIdForTenantAsync(
            request.DTO.Id,
            validation.TenantId,
            cancellationToken);
        if (entity is null)
        {
            throw new ApiException("Asset Status not found or update failed.", 404);
        }

        _mapper.Map(request.DTO, entity);
        entity.UpdatedById = validation.LoggedInEmployeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;

        var updated = await _unitOfWork.AssetStatusRepository.UpdateAsync(entity, cancellationToken);
        if (!updated)
        {
            throw new ApiException("Asset Status not found or update failed.", 404);
        }

        return ApiResponse<bool>.Success(true, "Asset Status updated successfully.");
    }

    #endregion
}

#endregion
