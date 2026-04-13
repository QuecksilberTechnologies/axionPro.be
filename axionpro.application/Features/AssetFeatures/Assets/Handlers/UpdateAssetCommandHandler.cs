using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


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
            // 🔹 Track uploaded file (VERY IMPORTANT for rollback cleanup)
            string? uploadedFileKey = null;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Asset Id is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "Asset",
                //    "Update"
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException("No permission to update asset.");

                // ===============================
                // 4️⃣ FETCH EXISTING ASSET
                // ===============================
                var existingAsset = await _unitOfWork.AssetRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

                if (existingAsset == null)
                    throw new KeyNotFoundException("Asset not found.");

                // ===============================
                // 5️⃣ UPDATE FIELDS (SAFE MERGE)
                // ===============================
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
                
                existingAsset.AssetStatusId = request.DTO.AssetStatusId ?? existingAsset.AssetStatusId;
                existingAsset.IsAssigned = request.DTO.IsAssigned ?? existingAsset.IsAssigned;
                existingAsset.IsActive = request.DTO.IsActive ?? existingAsset.IsActive;
                existingAsset.IsSoftDeleted = false;
                existingAsset.UpdatedDateTime = null;
                existingAsset.DeletedDateTime = null;
                existingAsset.PurchaseDate = request.DTO.PurchaseDate.HasValue ? DateTime.SpecifyKind(request.DTO.PurchaseDate.Value, DateTimeKind.Utc) : null;
                existingAsset.WarrantyExpiryDate = request.DTO.WarrantyExpiryDate.HasValue ? DateTime.SpecifyKind(request.DTO.WarrantyExpiryDate.Value, DateTimeKind.Utc) : null;

                // ===============================
                // 6️⃣ QR UPDATE
                // ===============================
                existingAsset.QRCode = JsonConvert.SerializeObject(new
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

                // ===============================
                // 7️⃣ IMAGE UPLOAD (CRITICAL SAFE BLOCK)
                // ===============================
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

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.AssetImageFile,
                            folderPath,
                            fileName);

                        assetImagePath = uploadedFileKey;
                    }
                    catch (Exception ex)
                    {
                        // ❗ Image failure should NOT break update
                        _logger.LogError(ex, "Asset image upload failed");
                    }
                }

                // ===============================
                // 8️⃣ UPDATE DATABASE
                // ===============================
                var updatedAsset = await _unitOfWork.AssetRepository
                    .UpdateAsync(existingAsset, assetImagePath);

                if (updatedAsset == null)
                    throw new ApiException("Asset update failed.", 500);

                // ===============================
                // 9️⃣ COMMIT TRANSACTION
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<GetAssetResponseDTO>
                    .Success(updatedAsset, "Asset updated successfully.");
            }
            catch (Exception ex)
            {
                // ===============================
                // 🔁 ROLLBACK DB
                // ===============================
                await _unitOfWork.RollbackTransactionAsync();

                // ===============================
                // 🧨 CRITICAL: DELETE UPLOADED IMAGE
                // ===============================
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                        _logger.LogWarning("Rollback: Uploaded image deleted from storage.");
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, "Failed to delete uploaded image after rollback.");
                    }
                }

                _logger.LogError(ex, "UpdateAsset failed");

                // ❗ IMPORTANT: Middleware handle karega
                throw;
            }
        }
    }


}
