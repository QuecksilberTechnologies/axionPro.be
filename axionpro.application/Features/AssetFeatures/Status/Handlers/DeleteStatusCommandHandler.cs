using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class DeleteStatusCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteStatusReqestDTO? DTO { get; set; }

        public DeleteStatusCommand(DeleteStatusReqestDTO deleteAssetStatusRequest)
        {
            this.DTO = deleteAssetStatusRequest;
        }
    }
        public class DeleteStatusCommandHandler : IRequestHandler<DeleteStatusCommand, ApiResponse<bool>>
     {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public DeleteStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }


        public async Task<ApiResponse<bool>> Handle(
     DeleteStatusCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting Asset Status");

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
                        "Invalid Status Id.",
                        new List<string> { "Status Id must be greater than 0." }
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
                //    "AssetStatus",   // 🔹 Module
                //    "Delete"         // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to delete asset status.");

                // ===============================
                // 4️⃣ DELETE (REPOSITORY)
                // ===============================
                bool isDeleted = await _unitOfWork.AssetStatusRepository
                    .DeleteAsync(request.DTO);

                if (!isDeleted)
                {
                    _logger.LogWarning(
                        "AssetStatus delete failed. Record not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id,
                        request.DTO.Prop.TenantId);

                    throw new ApiException(
                        "Delete failed. Record not found or already deleted.",
                        404
                    );
                }

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "AssetStatus deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Status deleted successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while deleting Asset Status. TenantId: {TenantId}, Id: {Id}",
                    request?.DTO?.Prop?.TenantId,
                    request?.DTO?.Id);

                throw;
            }
        }

    }

}