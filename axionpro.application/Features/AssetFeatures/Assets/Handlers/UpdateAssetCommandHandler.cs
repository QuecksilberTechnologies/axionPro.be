// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-owned assets and optional asset images.
// ================================================================

using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers;

#region Command

/// <summary>Represents the request to update an asset.</summary>
public class UpdateAssetCommand : IRequest<ApiResponse<GetAssetResponseDTO>>
{
    /// <summary>Initializes a new instance of the <see cref="UpdateAssetCommand"/> class.</summary>
    public UpdateAssetCommand(UpdateAssetRequestDTO dto) => DTO = dto;

    /// <summary>Gets the client-supplied asset update values.</summary>
    public UpdateAssetRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>Handles updates to tenant-owned assets.</summary>
public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, ApiResponse<GetAssetResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAssetCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>Initializes a new instance of the <see cref="UpdateAssetCommandHandler"/> class.</summary>
    public UpdateAssetCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateAssetCommandHandler> logger,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<GetAssetResponseDTO>> Handle(
        UpdateAssetCommand request,
        CancellationToken cancellationToken)
    {
        string? uploadedFileKey = null;
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (request.DTO is null || request.DTO.Id <= 0)
            {
                throw new ValidationErrorException(
                    "Invalid request.",
                    new List<string> { "Asset Id is required." });
            }

            // Resolve the trusted tenant-user context.
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            // Load the tenant-owned entity before applying client changes.
            var asset = await _unitOfWork.AssetRepository.GetSingleRecordForTenantAsync(
                request.DTO.Id,
                validation.TenantId,
                cancellationToken);
            if (asset is null)
            {
                throw new KeyNotFoundException("Asset not found.");
            }

            asset.AssetName = request.DTO.AssetName ?? asset.AssetName;
            asset.AssetTypeId = request.DTO.AssetTypeId;
            asset.Company = request.DTO.Company ?? asset.Company;
            asset.ModelNo = request.DTO.ModelNo ?? asset.ModelNo;
            asset.Size = request.DTO.Size ?? asset.Size;
            asset.Weight = request.DTO.Weight ?? asset.Weight;
            asset.Color = request.DTO.Color ?? asset.Color;
            asset.IsRepairable = request.DTO.IsRepairable;
            asset.Price = request.DTO.Price;
            asset.SerialNumber = request.DTO.SerialNumber ?? asset.SerialNumber;
            asset.Barcode = request.DTO.Barcode ?? asset.Barcode;
            asset.AssetStatusId = request.DTO.AssetStatusId;
            asset.IsAssigned = request.DTO.IsAssigned;
            asset.IsActive = request.DTO.IsActive;
            asset.PurchaseDate = request.DTO.PurchaseDate;
            asset.WarrantyExpiryDate = request.DTO.WarrantyExpiryDate;
            asset.UpdatedById = validation.LoggedInEmployeeId;
            asset.UpdatedDateTime = DateTime.UtcNow;
            asset.Qrcode = JsonConvert.SerializeObject(new
            {
                asset.Id,
                asset.AssetName,
                asset.AssetTypeId,
                asset.Company,
                asset.ModelNo,
                asset.SerialNumber,
                asset.Barcode,
                asset.AssetStatusId,
                asset.IsAssigned,
                asset.PurchaseDate,
                asset.WarrantyExpiryDate,
                asset.IsRepairable
            });

            string? assetImagePath = null;
            if (request.DTO.AssetImageFile is { Length: > 0 })
            {
                var cleanName = EncryptionSanitizer.CleanEncodedInput(request.DTO.AssetName ?? "asset")
                    .ToLowerInvariant()
                    .Replace(" ", "_");
                var fileName = $"asset-{cleanName}-{DateTime.UtcNow:yyyyMMddHHmmss}";
                var folderPath = $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.AssetsFolder}";
                uploadedFileKey = await _fileStorageService.UploadFileAsync(
                    request.DTO.AssetImageFile,
                    folderPath,
                    fileName);
                assetImagePath = uploadedFileKey;
            }

            var updatedAsset = await _unitOfWork.AssetRepository.UpdateAsync(asset, assetImagePath);
            if (updatedAsset is null)
            {
                throw new ApiException("Asset update failed.", 500);
            }

            await _unitOfWork.CommitTransactionAsync();
            return ApiResponse<GetAssetResponseDTO>.Success(updatedAsset, "Asset updated successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            if (!string.IsNullOrEmpty(uploadedFileKey))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                }
                catch (Exception deleteException)
                {
                    _logger.LogError(deleteException, "Failed to delete the rolled-back asset image.");
                }
            }

            _logger.LogError(ex, "Asset update failed.");
            throw;
        }
    }

    #endregion
}

#endregion
