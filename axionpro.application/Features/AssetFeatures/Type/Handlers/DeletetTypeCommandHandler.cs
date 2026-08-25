// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft-deletes tenant-owned asset types from authenticated requests.
// ================================================================

using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers;

#region Command

/// <summary>
/// Represents the request to delete an asset type.
/// </summary>
public class DeletetTypeCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeletetTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The asset type selected for deletion.</param>
    public DeletetTypeCommand(DeleteTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the asset type selected for deletion.
    /// </summary>
    public DeleteTypeRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles soft deletion of tenant-owned asset types.
/// </summary>
public class DeletetTypeCommandHandler : IRequestHandler<DeletetTypeCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletetTypeCommandHandler> _logger;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletetTypeCommandHandler"/> class.
    /// </summary>
    public DeletetTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeletetTypeCommandHandler> logger,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(
        DeletetTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(
                "Invalid Type Id.",
                new List<string> { "Type Id must be greater than 0." });
        }

        // Resolve the trusted tenant-user context.
        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        var deleted = await _unitOfWork.AssetTypeRepository.DeleteAsync(
            request.DTO.Id,
            validation.TenantId,
            validation.LoggedInEmployeeId,
            cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning(
                "Asset Type {AssetTypeId} was not found for tenant {TenantId}.",
                request.DTO.Id,
                validation.TenantId);
            throw new ApiException("Asset Type not found or already deleted.", 404);
        }

        return ApiResponse<bool>.Success(true, "Asset Type deleted successfully.");
    }

    #endregion
}

#endregion
