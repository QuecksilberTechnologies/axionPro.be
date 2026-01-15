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

public class AddAssetCommand : IRequest<ApiResponse<bool>>
{
    public AddAssetRequestDTO DTO { get; }

    public AddAssetCommand(AddAssetRequestDTO dto)
    {
        DTO = dto;
    }
}


public class AddAssetCommandHandler
    : IRequestHandler<AddAssetCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddAssetCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IPermissionService _permissionService;

    public AddAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddAssetCommandHandler> logger,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService,
        IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
        _permissionService = permissionService;
    }

    public async Task<ApiResponse<bool>> Handle(
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
                return ApiResponse<bool>.Fail(validation.ErrorMessage);

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 2️⃣ PERMISSION (OPTIONAL)
            // ===============================
            var permissions =
                await _permissionService.GetPermissionsAsync(validation.RoleId);

            // if (!permissions.Contains("AddAsset"))
            //     return ApiResponse<bool>.Fail("Permission denied.");

            // ===============================
            // 3️⃣ DTO → ENTITY
            // ===============================
            var asset = _mapper.Map<Asset>(request.DTO);
            asset.TenantId = validation.TenantId;
            asset.AddedById = validation.UserEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsSoftDeleted = false;
            asset.IsActive = true;

            // ===============================
            // 4️⃣ QR JSON
            // ===============================
            asset.Qrcode = JsonConvert.SerializeObject(new
            {
                asset.AssetName,
                asset.AssetTypeId
            });

            // ===============================
            // 5️⃣ IMAGE UPLOAD (SAFE)
            // ===============================
            string? assetImagePath = null;

            if (request.DTO.AssetImageFile != null &&
                request.DTO.AssetImageFile.Length > 0)
            {
                try
                {
                    string cleanName =
                        EncryptionSanitizer.CleanEncodedInput(
                            request.DTO.AssetName?.Trim()
                                .Replace(" ", "")
                                .ToLower() ?? "asset");

                    using var ms = new MemoryStream();
                    await request.DTO.AssetImageFile.CopyToAsync(ms);

                    string fileName =
                        $"ASSET-{cleanName}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                    string folderPath =
                        _fileStorageService.GetTenantFolderPath(
                            validation.TenantId,
                            "assets");

                    string fullPath =
                        await _fileStorageService.SaveFileAsync(
                            ms.ToArray(),
                            fileName,
                            folderPath);

                    if (!string.IsNullOrWhiteSpace(fullPath))
                        assetImagePath =
                            _fileStorageService.GetRelativePath(fullPath);
                }
                catch (Exception imgEx)
                {
                    _logger.LogError(imgEx, "⚠️ Asset image upload failed.");
                    assetImagePath = null;
                }
            }

            // ===============================
            // 6️⃣ SAVE (REPO RETURNS INT)
            // ===============================
            int result =
                await _unitOfWork.AssetRepository
                    .AddAsync(asset, assetImagePath);

            if (result <= 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.Fail("Asset creation failed.");
            }

            // ===============================
            // 7️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            // ===============================
            // 8️⃣ SUCCESS
            // ===============================
            return ApiResponse<bool>.Success(
                true,
                result == 2
                    ? "Asset created successfully with image."
                    : "Asset created successfully."
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "❌ AddAsset failed");

            return ApiResponse<bool>.Fail("Unexpected error while creating asset.");
        }
    }
}
