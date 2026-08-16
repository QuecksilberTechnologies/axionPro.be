// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves tenant-owned assets from authenticated requests.
// ================================================================

using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers;

#region Query

/// <summary>Represents the request to retrieve assets.</summary>
public class GetAllAssetCommand : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
{
    /// <summary>Initializes a new instance of the <see cref="GetAllAssetCommand"/> class.</summary>
    public GetAllAssetCommand(GetAssetRequestDTO dto) => DTO = dto;

    /// <summary>Gets the client-supplied asset filters.</summary>
    public GetAssetRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>Handles retrieval of tenant-owned assets.</summary>
public class GetAllAssetCommandHandler
    : IRequestHandler<GetAllAssetCommand, ApiResponse<List<GetAssetResponseDTO>>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IConfiguration _configuration;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>Initializes a new instance of the <see cref="GetAllAssetCommandHandler"/> class.</summary>
    public GetAllAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IIdEncoderService idEncoderService,
        IFileStorageService fileStorageService,
        IConfiguration configuration,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _idEncoderService = idEncoderService;
        _fileStorageService = fileStorageService;
        _configuration = configuration;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(
        GetAllAssetCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null)
        {
            throw new ValidationErrorException("Invalid request.");
        }

        // Resolve the trusted tenant context separately from client filters.
        var validation = await _commonRequestService.ValidateRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        var pagedAssets = await _unitOfWork.AssetRepository.GetAssetsByFilterAsync(
            validation.TenantId,
            request.DTO,
            cancellationToken);
        var encryptedList = ProjectionHelper.ToGetAssetResponseDTOs(
            pagedAssets.Data,
            _idEncoderService,
            validation.Claims.TenantEncriptionKey,
            _configuration,
            _fileStorageService);

        return ApiResponse<List<GetAssetResponseDTO>>.SuccessPaginatedOnly(
            Data: encryptedList,
            PageNumber: pagedAssets.PageNumber,
            PageSize: pagedAssets.PageSize,
            TotalRecords: pagedAssets.TotalCount,
            TotalPages: pagedAssets.TotalPages,
            Message: "Assets fetched successfully.",
            HasUploadedAll: pagedAssets.HasUploadedAll);
    }

    #endregion
}

#endregion
