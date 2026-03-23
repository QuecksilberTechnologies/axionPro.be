using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class AddAssetCommand : IRequest<ApiResponse<GetAssetResponseDTO>>
{
    public AddAssetRequestDTO DTO { get; }

    public AddAssetCommand(AddAssetRequestDTO dto)
    {
        DTO = dto;
    }
}


public class AddAssetCommandHandler
   : IRequestHandler<AddAssetCommand, ApiResponse<GetAssetResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddAssetCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IPermissionService _permissionService;
    private readonly IConfiguration configuration;
     
    public AddAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddAssetCommandHandler> logger,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService,
        IPermissionService permissionService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
        _permissionService = permissionService;
        this.configuration = configuration;
    }

    public async Task<ApiResponse<GetAssetResponseDTO>> Handle(
    AddAssetCommand request,
    CancellationToken ct)
    {
        // 🔹 Transaction start
        await _unitOfWork.BeginTransactionAsync();
        // 🔹 Track uploaded file (VERY IMPORTANT for rollback cleanup)
        string? uploadedFileKey = null;
        try
        {
            // ===============================
            // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();

            // ❌ Pehle: return Fail
            // ✅ Ab: throw (middleware handle karega)
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 2️⃣ NULL SAFETY (IMPORTANT)
            // ===============================
            if (request?.DTO == null)
                throw new ValidationErrorException("Invalid request data.");

            if (request.DTO.Prop == null)
                request.DTO.Prop = new();

            // Assign context values
            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 3️⃣ DTO → ENTITY (AutoMapper)
            // ===============================
            var asset = _mapper.Map<Asset>(request.DTO);

            // System fields
            asset.TenantId = validation.TenantId;
            asset.AddedById = validation.UserEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsActive = true;
            asset.IsSoftDeleted = false;

            // ===============================
            // 4️⃣ VALIDATE FOREIGN KEYS
            // ===============================
            var assetStatus = await _unitOfWork.AssetStatusRepository
                .GetByIdAsync(asset.AssetStatusId);

            if (assetStatus == null)
                throw new ValidationErrorException("Invalid AssetStatusId.");

            // ===============================
            // 5️⃣ IMAGE UPLOAD (SAFE BLOCK)
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

                    if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                        assetImagePath = uploadedFileKey;
                }
                catch (Exception ex)
                {
                    // ⚠️ Image failure should NOT break main flow
                    _logger.LogError(ex, "Asset image upload failed");
                }
            }

            // ===============================
            // 6️⃣ SAVE DATA
            // ===============================
            var insertedAsset = await _unitOfWork.AssetRepository
                .AddAsync(asset, assetImagePath);

            // ❌ Pehle: return Fail
            // ✅ Ab: throw (middleware handle karega)
            if (insertedAsset == null)
                throw new ApiException("Asset creation failed.", 500);

            // ===============================
            // 7️⃣ QR GENERATION
            // ===============================
            var qrPayload = new
            {
                insertedAsset.AssetId,
                insertedAsset.AssetName,
                insertedAsset.AssetTypeId,
                insertedAsset.Company,
                insertedAsset.ModelNo,
                insertedAsset.SerialNumber,
                insertedAsset.Barcode,
                insertedAsset.AssetStatusId,
                insertedAsset.StatusName,
                insertedAsset.PurchaseDate,
                insertedAsset.WarrantyExpiryDate,
                insertedAsset.IsRepairable,
                insertedAsset.IsAssigned
            };

            string qrJson = JsonConvert.SerializeObject(qrPayload);

            await _unitOfWork.AssetRepository
                .UpdateQrCodeAsync(insertedAsset.AssetId, qrJson);

            // ===============================
            // 8️⃣ FINAL RESPONSE BUILD
            // ===============================
            string baseUrl =
                configuration["FileSettings:BaseUrl"] ?? string.Empty;

            insertedAsset.AssetImagePath =
                string.IsNullOrEmpty(insertedAsset.AssetImagePath)
                ? null
                : $"{baseUrl}{insertedAsset.AssetImagePath}";

            // ===============================
            // 9️⃣ COMMIT TRANSACTION
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            return ApiResponse<GetAssetResponseDTO>
                .Success(insertedAsset, "Asset created successfully.");
        }
        catch (Exception ex)
        {
            // 🔁 Rollback ALWAYS on error
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "AddAsset failed");
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

            // ❗ IMPORTANT: throw (middleware handle karega)
            throw;
        }
    }
}
