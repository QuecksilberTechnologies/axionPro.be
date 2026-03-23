using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{

    public class DeletePolicyTypeInsuranceQuery : IRequest<ApiResponse<bool>>
    {
        public DeletePolicyTypeInsuranceMappingRequestDTO DTO { get; set; }

        public DeletePolicyTypeInsuranceQuery(DeletePolicyTypeInsuranceMappingRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeletePolicyTypeInsuranceMappingQueryHandler : IRequestHandler<DeletePolicyTypeInsuranceQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletePolicyTypeInsuranceMappingQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public DeletePolicyTypeInsuranceMappingQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeletePolicyTypeInsuranceMappingQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
      DeletePolicyTypeInsuranceQuery request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 DeletePolicyTypeInsurance started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.PolicyTypeInsuranceMapping,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 4️⃣ FETCH ENTITY
                // ===============================
                var policy = await _unitOfWork
                    .PolicyTypeInsuranceMappingRepository
                    .GetByIdAsync(request.DTO.Id, true);

                if (policy == null)
                    throw new ApiException("Insurance policy not found.", 404);

                // ===============================
                // 5️⃣ SOFT DELETE
                // ===============================
                policy.IsSoftDeleted = true;
                policy.SoftDeleteById = validation.UserEmployeeId;
                policy.SoftDeleteDateTime = DateTime.UtcNow;
                policy.IsActive = false;

                var deleted = await _unitOfWork
                    .PolicyTypeInsuranceMappingRepository
                    .SoftDeleteAsync(policy);

                if (!deleted)
                    throw new ApiException(
                        "Failed to delete insurance policy.", 500);

                _logger.LogInformation("✅ PolicyTypeInsuranceMapping deleted successfully");

                // ===============================
                // 6️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Insurance policy deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ DeletePolicyTypeInsurance failed");

                throw; // ✅ CRITICAL
            }
        }
    }
}
