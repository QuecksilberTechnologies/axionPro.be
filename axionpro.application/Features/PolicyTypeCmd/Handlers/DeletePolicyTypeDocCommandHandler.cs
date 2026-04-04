using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ================================
    // 🔹 COMMAND
    // ================================
    public class DeletePolicyTypeDocCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteRequestDTO DTO { get; }

        public DeletePolicyTypeDocCommand(DeleteRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ================================
    // 🔹 HANDLER
    // ================================
    public class DeletePolicyTypeDocCommandHandler
        : IRequestHandler<DeletePolicyTypeDocCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<DeletePolicyTypeDocCommandHandler> _logger;
        private readonly IPermissionService _permissionService;

        public DeletePolicyTypeDocCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<DeletePolicyTypeDocCommandHandler> logger,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeletePolicyTypeDocCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 DeletePolicyTypeDoc started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid request. Document Id is required.");

                // ===============================
                // 2️⃣ AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 3️⃣ PERMISSION CHECK (MANDATORY 🔥)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.PolicyType,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 4️⃣ FETCH ENTITY
                // ===============================
                var entity = await _unitOfWork.PolicyTypeDocumentRepository
                    .GetPolicyTypeOnlyDocByIdAsync(request.DTO.Id);

                if (entity == null)
                    throw new ApiException("Policy type document not found.", 404);
               

                // ===============================
                // 5️⃣ SOFT DELETE
                // ===============================
                entity.IsSoftDeleted = true;
                entity.IsActive = false;
                entity.SoftDeletedById = validation.UserEmployeeId;
                entity.SoftDeletedDateTime = DateTime.UtcNow;

                var isDeleted = await _unitOfWork.PolicyTypeDocumentRepository
                    .SoftDeleteOnlyDocAsync(entity);

                if (!isDeleted)
                    throw new ApiException("Failed to delete policy type document.");

                // ===============================
                // 6️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ PolicyTypeDoc deleted successfully. Id: {Id}", entity.Id);

                return ApiResponse<bool>.Success(true, "Policy type document deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ DeletePolicyTypeDoc failed");

                throw; // 🔥 Exception middleware handle karega
            }
        }
    }
}