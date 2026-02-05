using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // 2️⃣ Get existing policy (active only)
                var policy = await _unitOfWork.PolicyTypeInsuranceMappingRepository.GetByIdAsync(request.DTO.Id, true);

                if (policy == null)
                    return ApiResponse<bool>.Fail("Insurance policy not found.");

                // 3️⃣ Soft delete
                policy.IsSoftDeleted = true;
                policy.SoftDeleteById = validation.UserEmployeeId;
                policy.SoftDeleteDateTime = DateTime.UtcNow;
                policy.IsActive = false;

                bool deleted = await _unitOfWork.PolicyTypeInsuranceMappingRepository.SoftDeleteAsync(policy);

                if (!deleted)
                    return ApiResponse<bool>.Fail("Failed to delete insurance policy.");

                return ApiResponse<bool>.Success(true, "Insurance policy deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteInsurancePolicy failed");
                return ApiResponse<bool>.Fail("Failed to delete insurance policy.");
            }
        }
    }
}
