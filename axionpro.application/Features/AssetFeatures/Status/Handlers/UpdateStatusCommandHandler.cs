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
    public class UpdateStatusCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateStatusRequestDTO DTO { get; set; }

        public UpdateStatusCommand(UpdateStatusRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
      UpdateStatusCommand request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating Asset Status");

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

                // Assign values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetStatus",   // 🔹 Module
                //    "Update"         // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to update asset status.");

                // ===============================
                // 4️⃣ UPDATE RECORD
                // ===============================
                var isUpdated = await _unitOfWork.AssetStatusRepository
                    .UpdateAsync(request.DTO);

                if (!isUpdated)
                    throw new ApiException(
                        "Asset Status not found or update failed.",
                        404
                    );

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "Asset Status updated successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Status updated successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(ex, "An error occurred while updating asset status.");

                throw;
            }
        }

    }

}
