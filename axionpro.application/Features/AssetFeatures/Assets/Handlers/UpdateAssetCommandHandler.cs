using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks; 


namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class UpdateAssetCommand : IRequest<ApiResponse<GetAssetResponseDTO>>
    {
        public UpdateAssetRequestDTO DTO { get; }

        public UpdateAssetCommand(UpdateAssetRequestDTO dto)
        {
            DTO = dto;
        }
    }


    /// <summary>
    /// Handles Asset Update (IDEAL PATTERN)
    /// </summary>
    public class UpdateAssetCommandHandler
      : IRequestHandler<UpdateAssetCommand, ApiResponse<GetAssetResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateAssetCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;

        public UpdateAssetCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateAssetCommandHandler> logger,
            IFileStorageService fileStorageService,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            _config = config;
        }

        public async Task<ApiResponse<GetAssetResponseDTO>> Handle(
     UpdateAssetCommand request,
     CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ===========================
                // 1️⃣ COMMON VALIDATION
                // ===========================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<GetAssetResponseDTO>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===========================
                // 2️⃣ FETCH EXISTING ASSET
                // ===========================
                var existingAsset = await _unitOfWork.AssetRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

                if (existingAsset == null)
                    return ApiResponse<GetAssetResponseDTO>.Fail("Asset not found.");

                // ===========================
                // 3️⃣ UPDATE FIELDS
                // ===========================
                existingAsset.AssetName = request.DTO.AssetName ?? existingAsset.AssetName;
                existingAsset.AssetTypeId = request.DTO.AssetTypeId ?? existingAsset.AssetTypeId;
                existingAsset.Company = request.DTO.Company ?? existingAsset.Company;
                existingAsset.ModelNo = request.DTO.ModelNo ?? existingAsset.ModelNo;
                existingAsset.Size = request.DTO.Size ?? existingAsset.Size;
                existingAsset.Weight = request.DTO.Weight ?? existingAsset.Weight;
                existingAsset.Color = request.DTO.Color ?? existingAsset.Color;
                existingAsset.IsRepairable = request.DTO.IsRepairable ?? existingAsset.IsRepairable;
                existingAsset.Price = request.DTO.Price ?? existingAsset.Price;
                existingAsset.SerialNumber = request.DTO.SerialNumber ?? existingAsset.SerialNumber;
                existingAsset.Barcode = request.DTO.Barcode ?? existingAsset.Barcode;
                existingAsset.PurchaseDate = request.DTO.PurchaseDate ?? existingAsset.PurchaseDate;
                existingAsset.WarrantyExpiryDate = request.DTO.WarrantyExpiryDate ?? existingAsset.WarrantyExpiryDate;
                existingAsset.AssetStatusId = request.DTO.AssetStatusId ?? existingAsset.AssetStatusId;
                existingAsset.IsAssigned = request.DTO.IsAssigned ?? existingAsset.IsAssigned;
                existingAsset.IsActive = request.DTO.IsActive ?? existingAsset.IsActive;

                // ===========================
                // 4️⃣ QR UPDATE
                // ===========================
                existingAsset.Qrcode = JsonConvert.SerializeObject(new
                {
                    existingAsset.Id,
                    existingAsset.AssetName,
                    existingAsset.AssetTypeId,
                    existingAsset.Company,
                    existingAsset.ModelNo,
                    existingAsset.SerialNumber,
                    existingAsset.Barcode,
                    existingAsset.AssetStatusId,
                    existingAsset.AssetStatus?.StatusName,
                    existingAsset.IsAssigned,
                    existingAsset.PurchaseDate,
                    existingAsset.WarrantyExpiryDate,
                    existingAsset.IsRepairable,
                });

                // ===========================
                // 5️⃣ IMAGE UPDATE (S3 - NO DELETE)
                // ===========================
                string? assetImagePath = null;

                if (request.DTO.AssetImageFile != null &&
                    request.DTO.AssetImageFile.Length > 0)
                {
                    try
                    {
                        string cleanName =
                            EncryptionSanitizer.CleanEncodedInput(
                                request.DTO.AssetName ?? "asset")
                            .ToLower()
                            .Replace(" ", "_");

                        string fileName =
                            $"asset-{cleanName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.AssetsFolder}";

                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.AssetImageFile,
                            folderPath,
                            fileName);

                        assetImagePath = fileKey;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Asset image upload failed");
                    }
                }

                // ===========================
                // 6️⃣ UPDATE DATABASE
                // ===========================
                var updatedAsset = await _unitOfWork.AssetRepository
                    .UpdateAsync(existingAsset, assetImagePath);

                if (updatedAsset == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<GetAssetResponseDTO>.Fail("Asset update failed.");
                }

                // ===========================
                // 7️⃣ COMMIT
                // ===========================
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<GetAssetResponseDTO>
                    .Success(updatedAsset, "Asset updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "UpdateAsset failed");

                return ApiResponse<GetAssetResponseDTO>
                    .Fail("Unexpected error while updating asset.");
            }
        }
    }


}
