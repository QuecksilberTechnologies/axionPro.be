using MediatR;
using Microsoft.Extensions.Logging;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class UpdateAssetTypeCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateTypeRequestDTO DTO { get; set; }

        public UpdateAssetTypeCommand(UpdateTypeRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
    /// <summary>
    /// Handles update of Asset Type (IDEAL PATTERN)
    /// Repo decides existence & update result
    /// </summary>
    public class UpdateAssetTypeCommandHandler
        : IRequestHandler<UpdateAssetTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAssetTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateAssetTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateAssetTypeCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateAssetTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating Asset Type");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ INPUT VALIDATION
                // ===============================
                if (request.DTO == null || request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Asset Type Id.");

                if (string.IsNullOrWhiteSpace(request.DTO.TypeName))
                    return ApiResponse<bool>.Fail("Type Name is required.");

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("UpdateAssetType"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                // ===============================
                // 4️⃣ UPDATE (REPO DECIDES RESULT)
                // ===============================
                bool updated =
                    await _unitOfWork.AssetTypeRepository
                        .UpdateAsync(request.DTO);

                if (!updated)
                    return ApiResponse<bool>
                        .Fail("Asset Type not found or update failed.");

                _logger.LogInformation(
                    "Asset Type updated successfully | TypeId={Id} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Type updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Update Asset Type failed | TypeId={Id}",
                    request?.DTO?.Id);

                return ApiResponse<bool>
                    .Fail("Unexpected error while updating asset type.");
            }
        }
    }
}
