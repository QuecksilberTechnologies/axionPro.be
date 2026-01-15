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

public class AddAssetCommand : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
{
    public AddAssetRequestDTO DTO { get; }

    public AddAssetCommand(AddAssetRequestDTO dto)
    {
        DTO = dto;
    }
}

public class AddAssetCommandHandler
    : IRequestHandler<AddAssetCommand, ApiResponse<List<GetAssetResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddAssetCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IConfiguration _config;
    private readonly IPermissionService _permissionService;


    public AddAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddAssetCommandHandler> logger,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService,
        IConfiguration config,
        IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
        _config = config;
        _permissionService = permissionService;
    }

    public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(
        AddAssetCommand request,
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // ===============================
            // 1️⃣ COMMON VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                return ApiResponse<List<GetAssetResponseDTO>>
                    .Fail(validation.ErrorMessage);

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 2️⃣ PERMISSION CHECK (OPTIONAL)
            // ===============================
            var permissions =
                await _permissionService.GetPermissionsAsync(validation.RoleId);

            // if (!permissions.Contains("ViewAsset"))
            // {
            //     return ApiResponse<List<GetAssetResponseDTO>>
            //         .Fail("You do not have permission to view assets.");
            // }
            // ===============================
            // 2️⃣ DTO → ENTITY
            // ===============================
            var asset = _mapper.Map<Asset>(request.DTO);
            asset.TenantId = validation.TenantId;
            asset.AddedById = validation.UserEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsSoftDeleted = false;
            asset.IsActive = true;

            // ===============================
            // 3️⃣ QR JSON (DATA ONLY)
            // ===============================
            asset.Qrcode = JsonConvert.SerializeObject(new
            {
                asset.AssetName,
                asset.AssetTypeId
            });

            // ===============================
            // 4️⃣ ASSET IMAGE UPLOAD (SAFE)
            // ===============================
            string? assetImagePath = null;

            try
            {
                if (request.DTO.AssetImageFile != null &&
                    request.DTO.AssetImageFile.Length > 0)
                {
                    string cleanName =
                        EncryptionSanitizer.CleanEncodedInput(
                            request.DTO.AssetName.Trim()
                                .Replace(" ", "")
                                .ToLower());

                    using var ms = new MemoryStream();
                    await request.DTO.AssetImageFile.CopyToAsync(ms);

                    string fileName =
                        $"ASSET-{cleanName}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                    string folderPath =
                        _fileStorageService.GetTenantFolderPath(
                            request.DTO.Prop.TenantId,
                            "assets");

                    string fullPath =
                        await _fileStorageService.SaveFileAsync(
                            ms.ToArray(),
                            fileName,
                            folderPath);

                    if (!string.IsNullOrEmpty(fullPath))
                        assetImagePath =
                            _fileStorageService.GetRelativePath(fullPath);
                }
            }
            catch (Exception imgEx)
            {
                _logger.LogError(imgEx, "⚠️ Asset image upload failed.");
                assetImagePath = null; // continue without image
            }

            // ===============================
            // 5️⃣ SAVE ASSET + IMAGE (REPO)
            // ===============================
            await _unitOfWork.AssetRepository.AddAsync(asset, assetImagePath);

            // ===============================
            // 6️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            var pagedAssets = await _unitOfWork.AssetRepository.GetInsertedAssetAsync(request.DTO.Prop.TenantId, request.DTO.IsActive);

             var encryptedList = ProjectionHelper.ToGetAssetResponseDTOs(pagedAssets, _idEncoderService, validation.Claims.TenantEncriptionKey, _config);



            // ===============================
            // 7️⃣ RETURN RESPONSE
            // ===============================
            return ApiResponse<List<GetAssetResponseDTO>>.Success(pagedAssets, "Asset created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "❌ AddAsset failed");

            return ApiResponse<List<GetAssetResponseDTO>>
                .Fail("Asset creation failed");
        }
    }
}
 