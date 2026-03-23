using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using MediatR;
using Microsoft.Extensions.Logging;

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
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw (middleware handle karega)
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY + INPUT VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException(
                        "Invalid Category Id.",
                        new List<string> { "Category Id must be greater than 0." }
                    );

                if (string.IsNullOrWhiteSpace(request.DTO.CategoryName))
                    throw new ValidationErrorException(
                        "Category name cannot be empty.",
                        new List<string> { "CategoryName is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Assign values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetCategory",   // 🔹 Module
                //    "Update"           // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to update asset category.");

                // ===============================
                // 4️⃣ UPDATE RECORD
                // ===============================
                bool updated = await _unitOfWork.AssetCategoryRepository
                    .UpdateAsync(request.DTO);

                if (!updated)
                    throw new ApiException(
                        "Category not found or update failed.",
                        404
                    );

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "Asset Category updated successfully. CategoryId: {Id}, TenantId: {TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Category updated successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(ex, "Update Asset Category failed");

                throw;
            }
        }
    }
}
