using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
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
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // ===============================
            // 1️⃣ COMMON VALIDATION
            // ===============================
            // ===============================
            // 1️⃣ COMMON VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                return ApiResponse<GetAssetResponseDTO>.Fail(validation.ErrorMessage);

            // Null safety
            if (request.DTO.Prop == null)
                request.DTO.Prop = new();

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;


            // ===============================
            // 2️⃣ DTO → ENTITY (AutoMapper)
            // ===============================
            var asset = _mapper.Map<Asset>(request.DTO);

            // Mandatory system fields
            asset.TenantId = validation.TenantId;
            asset.AddedById = validation.UserEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsActive = true;
            asset.IsSoftDeleted = false;

            // Safe IsAssigned logic
          //  asset.IsAssigned = request.DTO.IsAssigned ?? false;


            // ===============================
            // 3️⃣ VALIDATE ASSET STATUS (Optional but safe)
            // ===============================
            var assetStatus = await _unitOfWork.AssetStatusRepository
                .GetByIdAsync(asset.AssetStatusId);

            if (assetStatus == null)
                return ApiResponse<GetAssetResponseDTO>.Fail("Invalid AssetStatusId.");


            // ===============================
            // 4️⃣ QR JSON
            // ===============================
           
            // ===============================
            // 4️⃣ IMAGE UPLOAD
            // ===============================
            string? assetImagePath = null;

            if (request.DTO.AssetImageFile != null &&
                request.DTO.AssetImageFile.Length > 0)
            {
                try
                {
                    string cleanName =
                        EncryptionSanitizer.CleanEncodedInput(
                            request.DTO.AssetName ?? "asset");

                    using var ms = new MemoryStream();
                    await request.DTO.AssetImageFile.CopyToAsync(ms);

                    string fileName =
                        $"ASSET-{cleanName}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                    string folder =
                        _fileStorageService.GetTenantFolderPath(
                            validation.TenantId, "assets");

                    string fullPath =
                        await _fileStorageService.SaveFileAsync(
                            ms.ToArray(), fileName, folder);

                    assetImagePath =
                        _fileStorageService.GetRelativePath(fullPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Asset image upload failed");
                }
            }

            // ===============================
            // 5️⃣ SAVE & RETURN OBJECT
            // ===============================
            var insertedAsset =
                await _unitOfWork.AssetRepository
                    .AddAsync(asset, assetImagePath);

            if (insertedAsset == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<GetAssetResponseDTO>
                    .Fail("Asset creation failed.");
            }
            // ===============================
            // 6️⃣ QR GENERATION (AFTER INSERT)
            // ===============================
            var qrPayload = new
            {
                insertedAsset.AssetId,          // ✅ NOW AVAILABLE
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

            // 🔁 Update QR in DB
            await _unitOfWork.AssetRepository
                .UpdateQrCodeAsync(insertedAsset.AssetId, qrJson);

            string baseUrl =
              configuration["FileSettings:BaseUrl"] ?? string.Empty;
            insertedAsset.AssetImagePath = $"{baseUrl}{insertedAsset.AssetImagePath}";
            await _unitOfWork.CommitTransactionAsync();

            return ApiResponse<GetAssetResponseDTO>
                .Success(insertedAsset, "Asset created successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "AddAsset failed");

            return ApiResponse<GetAssetResponseDTO>
                .Fail("Unexpected error while creating asset.");
        }
    }
}
