using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IQRService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class AddAssetCommand : IRequest<ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>>
{
    public AddAssetRequestDTO DTO { get; }

    public AddAssetCommand(AddAssetRequestDTO dto)
    {
        DTO = dto;
    }
}

public class AddAssetCommandHandler
    : IRequestHandler<AddAssetCommand, ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddAssetCommandHandler> _logger;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IPermissionService _permissionService;
    private readonly IQRService _qrService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IConfiguration _config;
    private readonly IIdEncoderService _idEncoderService;
    public AddAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddAssetCommandHandler> logger,
        ICommonRequestService commonRequestService,
        IPermissionService permissionService,
        IQRService qrService,
        IFileStorageService fileStorageService, IConfiguration configuration, IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _commonRequestService = commonRequestService;
        _permissionService = permissionService;
        _qrService = qrService;
        _fileStorageService = fileStorageService;
        _config = configuration;
        _idEncoderService = idEncoderService;
    }

    public async Task<ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>> Handle(
        AddAssetCommand request,
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // =========================
            // 1️⃣ COMMON VALIDATION
            // =========================
            var validation =
                await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>
                    .Fail(validation.ErrorMessage);

            // =========================
            // 2️⃣ DTO → ENTITY
            // =========================
            var asset = _mapper.Map<Asset>(request.DTO);
            asset.TenantId = validation.TenantId;
            asset.AddedById = validation.UserEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsSoftDeleted = false;

            // =========================
            // 3️⃣ SAVE ASSET (DB ONLY)
            // =========================
            await _unitOfWork.AssetRepository.AddAsync(asset);

            // =========================
            // 4️⃣ QR JSON (DATA ONLY)
            // =========================
            var qrPayload = new
            {
                asset.Id,
                asset.AssetName,
                asset.AssetTypeId,
                asset.TenantId
            };

            string qrJson = JsonConvert.SerializeObject(qrPayload);
            asset.Qrcode = qrJson;

            // =========================
            // 5️⃣ QR IMAGE SAVE  
            // =========================
            byte[] qrBytes = _qrService.GenerateQrCode(qrJson, 20);

            string qrFileName =
                $"ASSET-QR-{qrPayload.AssetName}-{DateTime.UtcNow:yyMMddHHmmss}.png";

            string qrFolderPath =
                _fileStorageService.GetTenantFolderPath(
                    asset.TenantId,
                    "qrcodes");

            string qrFullPath =
                await _fileStorageService.SaveFileAsync(
                    qrBytes,
                    qrFileName,
                    qrFolderPath);

            asset.Qrcode = _fileStorageService.GetRelativePath(qrFullPath);

            // =========================
            // 6️⃣ ASSET IMAGE SAVE  
            // =========================
            if (!string.IsNullOrWhiteSpace(request.DTO.AssetImagePath))
            {
                string assetFileName =
                    $"ASSET-{asset.Id}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                string assetFolderPath =
                    _fileStorageService.GetTenantFolderPath(
                        asset.TenantId,
                        "assets");

                string destinationPath =
                    Path.Combine(assetFolderPath, assetFileName);

                string savedAssetPath =
                    await FileHelper.SaveAssetImageAsync(
                        request.DTO.AssetImagePath,
                        destinationPath,
                        _fileStorageService,
                        _logger);

                asset.Qrcode =  _fileStorageService.GetRelativePath(savedAssetPath);
            }

            // =========================
            // 7️⃣ UPDATE ASSET (QR + IMAGE PATH)
            // =========================
            await _unitOfWork.AssetRepository.UpdateAsync(asset);

            // =========================
            // 8️⃣ COMMIT
            // =========================
            await _unitOfWork.CommitTransactionAsync();

            // =========================
            // 9️⃣ RETURN PAGED DATA
            // =========================
         
            var pagedAssets =   await _unitOfWork.AssetRepository.GetAllAssetAsync(request.DTO.Prop.TenantId, request.DTO.IsActive);

            // 4. Pre-map projection + encrypt Ids (fast)
            // If pagedResult.Items are entities:
       //     var encryptedList = ProjectionHelper.ToGetAssetResponseDTOs(pagedAssets, _idEncoderService, validation.Claims.TenantEncriptionKey, _config);


            return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>
                .Success(null, "Asset created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "AddAsset failed");

            return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>
                .Fail("Asset creation failed");
        }
    }
}
 