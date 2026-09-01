// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant-owned assets and optional asset images.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers;

#region Command

/// <summary>Represents the request to create an asset.</summary>
public class AddAssetCommand : IRequest<ApiResponse<GetAssetResponseDTO>>
{
    /// <summary>Initializes a new instance of the <see cref="AddAssetCommand"/> class.</summary>
    public AddAssetCommand(AddAssetRequestDTO dto) => DTO = dto;

    /// <summary>Gets the client-supplied asset values.</summary>
    public AddAssetRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>Handles creation of tenant-owned assets.</summary>
public class AddAssetCommandHandler : IRequestHandler<AddAssetCommand, ApiResponse<GetAssetResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddAssetCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IConfiguration _configuration;

    #endregion

    #region Constructor

    /// <summary>Initializes a new instance of the <see cref="AddAssetCommandHandler"/> class.</summary>
    public AddAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddAssetCommandHandler> logger,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
        _configuration = configuration;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<GetAssetResponseDTO>> Handle(AddAssetCommand request, CancellationToken cancellationToken)
    {
        string? uploadedFileKey = null;
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (request.DTO is null)
            {
                throw new ValidationErrorException("Invalid request data.");
            }

            #region Tenant Request Validation

            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(
                    validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Trusted Request Context

            long userEmployeeId = validation.LoggedInEmployeeId;
            long tenantId = validation.TenantId;
            int tokenRoleId = validation.RoleId;

            if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
            {
                _logger.LogWarning(
                    "Invalid Tenant authorization context while creating Asset. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Runtime Permission Validation

            var permissionResult = await _unitOfWork.StoreProcedureRepository
                .CheckTenantEmployeePermissionAsync(
                    tenantId,
                    userEmployeeId,
                    tokenRoleId,
                    request.DTO.ModuleId,
                    request.DTO.OperationId,
                    cancellationToken);

            TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

            #endregion

            // Map client-editable values and apply server-controlled context.
            var asset = _mapper.Map<Asset>(request.DTO);
            asset.TenantId = tenantId;
            asset.AddedById = userEmployeeId;
            asset.AddedDateTime = DateTime.UtcNow;
            asset.IsActive = true;
            asset.IsSoftDeleted = false;
            asset.UpdatedDateTime = null;
            asset.DeletedDateTime = null;

            var assetStatus = await _unitOfWork.AssetStatusRepository.GetByIdForTenantAsync(
                asset.AssetStatusId,
                tenantId,
                cancellationToken);
            if (assetStatus is null)
            {
                throw new ValidationErrorException("Invalid AssetStatusId.");
            }

            string? assetImagePath = null;
            if (request.DTO.AssetImageFile is { Length: > 0 })
            {
                var cleanName = EncryptionSanitizer.CleanEncodedInput(request.DTO.AssetName ?? "asset")
                    .ToLowerInvariant()
                    .Replace(" ", "_");
                var fileName = $"asset-{cleanName}-{DateTime.UtcNow:yyyyMMddHHmmss}";
                var folderPath = $"{ConstantValues.TenantFolder}-{tenantId}/{ConstantValues.AssetsFolder}";
                uploadedFileKey = await _fileStorageService.UploadFileAsync(
                    request.DTO.AssetImageFile,
                    folderPath,
                    fileName);
                assetImagePath = uploadedFileKey;
            }

            var insertedAsset = await _unitOfWork.AssetRepository.AddAsync(asset, assetImagePath!);
            if (insertedAsset is null)
            {
                throw new ApiException("Asset creation failed.", 500);
            }

            var qrJson = JsonConvert.SerializeObject(new
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
            });
            await _unitOfWork.AssetRepository.UpdateQrCodeAsync(insertedAsset.AssetId, qrJson);

            var baseUrl = _configuration["FileSettings:BaseUrl"] ?? string.Empty;
            insertedAsset.AssetImagePath = string.IsNullOrEmpty(insertedAsset.AssetImagePath)
                ? null
                : $"{baseUrl}{insertedAsset.AssetImagePath}";

            await _unitOfWork.CommitTransactionAsync();
            return ApiResponse<GetAssetResponseDTO>.Success(insertedAsset, "Asset created successfully.");
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

            _logger.LogError(ex, "Asset creation failed.");
            throw;
        }
    }

    #endregion
}

#endregion
