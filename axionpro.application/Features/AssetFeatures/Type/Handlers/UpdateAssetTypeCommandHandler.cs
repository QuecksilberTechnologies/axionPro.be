using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

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
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY + INPUT VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException(
                        "Invalid Asset Type Id.",
                        new List<string> { "Type Id must be greater than 0." }
                    );

                if (string.IsNullOrWhiteSpace(request.DTO.TypeName))
                    throw new ValidationErrorException(
                        "Type Name is required.",
                        new List<string> { "TypeName cannot be empty." }
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
                //    "AssetType",   // 🔹 Module
                //    "Update"       // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to update asset type.");

                // ===============================
                // 4️⃣ UPDATE (REPOSITORY)
                // ===============================
                bool updated = await _unitOfWork.AssetTypeRepository
                    .UpdateAsync(request.DTO);

                if (!updated)
                    throw new ApiException(
                        "Asset Type not found or update failed.",
                        404
                    );

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "Asset Type updated successfully | TypeId={Id} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Type updated successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Update Asset Type failed | TypeId={Id}",
                    request?.DTO?.Id);

                throw;
            }
        }

    }
}
