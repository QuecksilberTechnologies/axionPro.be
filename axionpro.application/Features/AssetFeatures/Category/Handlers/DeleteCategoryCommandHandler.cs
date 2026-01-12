using MediatR;
using Microsoft.Extensions.Logging;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class DeleteCategoryCommand
        : IRequest<ApiResponse<bool>>
    {
        public DeleteCategoryReqestDTO DTO { get; set; }

        public DeleteCategoryCommand(DeleteCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles delete of Asset Category (IDEAL PATTERN)
    /// Repo is single source of truth
    /// </summary>
    public class DeleteCategoryCommandHandler
        : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public DeleteCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteCategoryCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting Asset Category");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ BASIC INPUT CHECK
                // ===============================
                if (request.DTO == null || request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Category Id.");

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("DeleteAssetCategory"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                // ===============================
                // 4️⃣ DELETE (REPO DECIDES RESULT)
                // ===============================
                bool deleted =
                    await _unitOfWork.AssetCategoryRepository
                        .DeleteAsync(request.DTO);

                if (!deleted)
                    return ApiResponse<bool>.Fail(
                        "Delete failed. Record may not exist or already deleted.");

                _logger.LogInformation(
                    "Asset Category deleted successfully | CategoryId={Id} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Category deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete Asset Category failed");

                return ApiResponse<bool>
                    .Fail("Unexpected error while deleting asset category.");
            }
        }
    }
}
