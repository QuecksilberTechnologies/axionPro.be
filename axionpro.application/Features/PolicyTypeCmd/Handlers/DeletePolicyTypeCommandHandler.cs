using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; 
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ================================
    // 🔹 COMMAND
    // ================================
    public class DeletePolicyTypeCommand : IRequest<ApiResponse<bool>>
    {
        public DeletePolicyTypeDTO DTO { get; set; }

        public DeletePolicyTypeCommand(DeletePolicyTypeDTO dto)
        {
            DTO = dto;
        }
    }

    // ================================
    // 🔹 HANDLER
    // ================================
    public class DeletePolicyTypeCommandHandler
        : IRequestHandler<DeletePolicyTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreatePolicyTypeCommandHandler> _logger;
        IPermissionService permissionService;
        public DeletePolicyTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService, ILogger<CreatePolicyTypeCommandHandler> logger, IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
            this.permissionService = permissionService; 
        }

        public async Task<ApiResponse<bool>> Handle(
      DeletePolicyTypeCommand request,
      CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🔹 DeletePolicyType started");

                // ===============================
                // 1️⃣ VALIDATION (NULL + ID)
                // ===============================
                if (request?.DTO == null || request.DTO.PolicyId <= 0)
                    throw new ValidationErrorException("Invalid request. PolicyId is required.");

                // ===============================
                // 2️⃣ AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 3️⃣ PERMISSION CHECK
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
                var policyType =
                    await _unitOfWork.PolicyTypeRepository
                        .GetPolicyTypeByIdAsync(request.DTO.PolicyId, null);

                if (policyType == null)
                    throw new ApiException("Policy type not found.", 404);

                // ===============================
                // 5️⃣ SOFT DELETE POLICY TYPE
                // ===============================
                policyType.IsSoftDelete = true;
                policyType.IsActive = false;
                policyType.SoftDeleteById = validation.UserEmployeeId;
                policyType.SoftDeleteDateTime = DateTime.UtcNow;

                var policyDeleted =
                    await _unitOfWork.PolicyTypeRepository
                        .SoftDeletePolicyTypeAsync(policyType);

                if (!policyDeleted)
                    throw new ApiException("Policy type deletion failed.", 500);

                // ===============================
                // 6️⃣ SOFT DELETE RELATED DOCS (OPTIONAL)
                // ===============================
                var docsDeleted =
                    await _unitOfWork.CompanyPolicyDocumentRepository
                        .SoftDeleteByPolicyTypeIdAsync(
                            policyType.Id,
                            validation.UserEmployeeId);

                if (!docsDeleted)
                {
                    _logger.LogWarning(
                        "⚠️ PolicyType deleted but related documents not deleted. PolicyTypeId: {Id}",
                        policyType.Id);
                }

                // ===============================
                // 7️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ PolicyType deleted successfully");

                return ApiResponse<bool>
                    .Success(true, "Policy type deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ DeletePolicyType failed");

                throw; // ✅ CRITICAL
            }
        }

    }

}


