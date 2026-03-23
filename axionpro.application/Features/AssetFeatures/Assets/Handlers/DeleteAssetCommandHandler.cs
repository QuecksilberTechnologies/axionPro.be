using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
using MediatR;
using Microsoft.Extensions.Logging;
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
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw (middleware handle karega)
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ INPUT VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException(
                        "Invalid Asset Id.",
                        new List<string> { "Asset Id must be greater than 0." }
                    );

                // Null safety (VERY IMPORTANT)
                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Inject decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (ROLE + MODULE + OPERATION)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "Asset",      // 🔹 Module Name (DB se match hona chahiye)
                //    "Delete"      // 🔹 Operation Name
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException("You do not have permission to delete asset.");

                // ===============================
                // 4️⃣ CHECK EXISTING ASSET
                // ===============================
                var existingAsset = await _unitOfWork.AssetRepository
                    .GetSingleRecordAsync(request.DTO.Id, null);

                if (existingAsset == null)
                    throw new KeyNotFoundException("Asset not found.");

                // ===============================
                // 5️⃣ DELETE OPERATION
                // ===============================
                bool deleted = await _unitOfWork.AssetRepository
                    .DeleteAssetAsync(request.DTO);

                if (!deleted)
                    throw new ApiException("Asset not found or already deleted.", 404);

                // ===============================
                // 6️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "Asset deleted successfully | AssetId={AssetId} | TenantId={TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset deleted successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: DO NOT return Fail
                // ❗ Middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while deleting Asset | AssetId={AssetId}",
                    request?.DTO?.Id);

                throw;
            }
        }
    }

}
