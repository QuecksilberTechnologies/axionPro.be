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
                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // 2️⃣ Get existing policy (active only)
                var policy = await _unitOfWork.InsuranceRepository
                    .GetByIdAsync(request.DTO.Id, validation.TenantId, true);

                if (policy == null)
                    return ApiResponse<bool>.Fail("Insurance policy not found.");

                // 3️⃣ Soft delete
                policy.IsSoftDeleted = true;
                policy.SoftDeletedById = validation.UserEmployeeId;
                policy.DeletedDateTime = DateTime.UtcNow;
                policy.IsActive =false;

                bool deleted = await _unitOfWork.InsuranceRepository.UpdateAsync(policy);

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
