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
    public class DeleteAssetCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteAssetReqestDTO DTO { get; }

        public DeleteAssetCommand(DeleteAssetReqestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles soft deletion of Asset (IDEAL PATTERN)
    /// </summary>
    public class DeleteAssetCommandHandler
        : IRequestHandler<DeleteAssetCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteAssetCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public DeleteAssetCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteAssetCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteAssetCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting Asset");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();
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

                // if (!permissions.Contains("DeleteAsset"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                // ===============================
                // 4️⃣ DELETE (REPO DECIDES)
                // ===============================
                bool deleted =
                    await _unitOfWork.AssetRepository
                        .DeleteAssetAsync(request.DTO);

                if (!deleted)
                    return ApiResponse<bool>.Fail(
                        "Asset not found or already deleted.");

                _logger.LogInformation(
                    "Asset deleted successfully | AssetId={AssetId} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while deleting Asset | AssetId={AssetId}",
                    request?.DTO?.Id);

                return ApiResponse<bool>
                    .Fail("Unexpected error while deleting asset.");
            }
        }
    }
}
