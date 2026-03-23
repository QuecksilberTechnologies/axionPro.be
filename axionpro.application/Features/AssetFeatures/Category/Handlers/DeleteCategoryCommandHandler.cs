using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

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

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Inject values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetCategory",   // 🔹 Module
                //    "Delete"           // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to delete asset category.");

                // ===============================
                // 4️⃣ DELETE (REPOSITORY)
                // ===============================
                bool deleted = await _unitOfWork.AssetCategoryRepository
                    .DeleteAsync(request.DTO);

                if (!deleted)
                    throw new ApiException(
                        "Delete failed. Record may not exist or already deleted.",
                        404
                    );

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "Asset Category deleted successfully | CategoryId={Id} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Category deleted successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(ex, "Delete Asset Category failed");

                throw;
            }
        }
    }


}
