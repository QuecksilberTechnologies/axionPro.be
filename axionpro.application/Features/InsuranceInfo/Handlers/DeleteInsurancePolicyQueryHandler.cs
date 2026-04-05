using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
namespace axionpro.application.Features.InsuranceInfo.Handlers
{

    public class DeleteInsurancePolicyQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteInsurancePolicyRequestDTO DTO { get; set; }

        public DeleteInsurancePolicyQuery(DeleteInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeleteInsurancePolicyQueryHandler
        : IRequestHandler<DeleteInsurancePolicyQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteInsurancePolicyQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteInsurancePolicyQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteInsurancePolicyQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
       DeleteInsurancePolicyQuery request,
       CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 DeleteInsurancePolicy started");

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
                //    Modules.InsurancePolicy,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ApiException("Invalid request data.", 401);

                // ===============================
                // 4️⃣ FETCH ENTITY
                // ===============================
                var policy = await _unitOfWork.InsuranceRepository
                    .GetByIdAsync(request.DTO.Id, validation.TenantId, true);



                if (policy == null)
                    throw new ApiException("Insurance policy not found.", 401);

                // ===============================
                // 5️⃣ SOFT DELETE
                // ===============================
                policy.IsSoftDeleted = true;
                policy.SoftDeletedById = validation.UserEmployeeId;
                policy.DeletedDateTime = DateTime.UtcNow;
                policy.IsActive = false;

                var mapInsurance = await _unitOfWork.PolicyTypeInsuranceMappingRepository.GetByMappedByInsuranceIdAsync(request.DTO.Id, true);

                if (mapInsurance == null)
                {
                    _logger.LogWarning("⚠️ No mapping found for insurance policy ID {InsuranceId}", request.DTO.Id);
                }
                else
                {
                    // ===============================
                    // 5️⃣ SOFT DELETE
                    // ===============================
                    policy.IsSoftDeleted = true;
                    policy.SoftDeletedById = validation.UserEmployeeId;
                    policy.DeletedDateTime = DateTime.UtcNow;
                    policy.IsActive = false;
                    _logger.LogInformation("✅ Insurance policy deleted successfully");
                    var deletedInsuranceMapped = await _unitOfWork.PolicyTypeInsuranceMappingRepository.SoftDeleteAsync(mapInsurance);

                }

                var deletedInsurance = await _unitOfWork.InsuranceRepository.SoftDeleteAsync(policy);

                if (!deletedInsurance)
                    throw new ApiException("Failed to delete insurance policy.", 401);

               
                // ===============================
                // 6️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Insurance policy deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ DeleteInsurancePolicy failed");

                throw; // ✅ CRITICAL
            }
        }
    }
}
