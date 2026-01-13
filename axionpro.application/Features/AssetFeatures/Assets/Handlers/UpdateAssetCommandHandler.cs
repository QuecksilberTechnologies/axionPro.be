using MediatR;
using Microsoft.Extensions.Logging;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class UpdateAssetCommand : IRequest<ApiResponse<bool>>
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
        : IRequestHandler<UpdateAssetCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAssetCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateAssetCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateAssetCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateAssetCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating Asset");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // ===============================
                // 2️⃣ BASIC INPUT VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Asset Id.");

                // Inject decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("UpdateAsset"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                // ===============================
                // 4️⃣ UPDATE (REPO DECIDES RESULT)
                // ===============================
                bool updated =
                    await _unitOfWork.AssetRepository
                        .UpdateAssetInfoAsync(request.DTO);

                if (!updated)
                    return ApiResponse<bool>
                        .Fail("Asset not found or update failed.");

                _logger.LogInformation(
                    "Asset updated successfully | AssetId={AssetId} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while updating Asset | AssetId={AssetId}",
                    request?.DTO?.Id);

                return ApiResponse<bool>
                    .Fail("Unexpected error while updating asset.");
            }
        }
    }
}
