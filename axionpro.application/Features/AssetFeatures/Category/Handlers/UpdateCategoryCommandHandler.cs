using MediatR;
using Microsoft.Extensions.Logging;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class UpdateCategoryCommand
        : IRequest<ApiResponse<bool>>
    {
        public UpdateCategoryReqestDTO DTO { get; set; }

        public UpdateCategoryCommand(UpdateCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles update of Asset Category (IDEAL PATTERN – BOOL RESPONSE)
    /// </summary>
    public class UpdateCategoryCommandHandler
        : IRequestHandler<UpdateCategoryCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateCategoryCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating Asset Category");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ INPUT VALIDATION
                // ===============================
                if (request.DTO == null || request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Category Id.");

                if (string.IsNullOrWhiteSpace(request.DTO.CategoryName))
                    return ApiResponse<bool>.Fail("Category name cannot be empty.");

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("UpdateAssetCategory"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

          
                // ===============================
                // 5️⃣ UPDATE RECORD
                // ===============================
                bool updated =
                    await _unitOfWork.AssetCategoryRepository.UpdateAsync(request.DTO);

                if (!updated)
                    return ApiResponse<bool>.Fail("Category not found or update failed.");

                _logger.LogInformation(
                    "Asset Category updated successfully. CategoryId: {Id}, TenantId: {TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>.Success(true, "Asset Category updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Asset Category failed");

                return ApiResponse<bool>.Fail(
                    "Unexpected error while updating asset category.");
            }
        }
    }
}
