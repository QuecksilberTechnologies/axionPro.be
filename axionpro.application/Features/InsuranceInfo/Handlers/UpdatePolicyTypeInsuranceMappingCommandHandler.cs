using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    
    public class UpdatePolicyTypeInsuranceMappingCommand
      : IRequest<ApiResponse<bool>>
    {
        public UpdatePolicyTypeInsuranceMappingRequestDTO DTO { get; }

        public UpdatePolicyTypeInsuranceMappingCommand(
            UpdatePolicyTypeInsuranceMappingRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdatePolicyTypeInsuranceMappingCommandHandler
        : IRequestHandler<UpdatePolicyTypeInsuranceMappingCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePolicyTypeInsuranceMappingCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public UpdatePolicyTypeInsuranceMappingCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdatePolicyTypeInsuranceMappingCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdatePolicyTypeInsuranceMappingCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Tenant/User
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // 2️⃣ Fetch existing mapping
                var mapping =
                    await _unitOfWork.PolicyTypeInsuranceMappingRepository
                        .GetByIdAsync(request.DTO.Id, true);

                if (mapping == null)
                    return ApiResponse<bool>.Fail("Policy type insurance mapping not found.");

                // 3️⃣ Update allowed fields only
                if (request.DTO.PolicyTypeId.HasValue)
                    mapping.PolicyTypeId = request.DTO.PolicyTypeId.Value;

                if (request.DTO.InsurancePolicyId.HasValue)
                    mapping.InsurancePolicyId = request.DTO.InsurancePolicyId.Value;

                if (request.DTO.IsActive.HasValue)
                    mapping.IsActive = request.DTO.IsActive.Value;

                // 4️⃣ Audit
                mapping.UpdatedById = validation.UserEmployeeId;
                mapping.UpdatedDateTime = DateTime.UtcNow;

                // 5️⃣ Save
                var updated =
                    await _unitOfWork.PolicyTypeInsuranceMappingRepository.UpdateAsync(mapping);

                if (!updated)
                    return ApiResponse<bool>.Fail("Failed to update policy type insurance mapping.");

                await _unitOfWork.CommitAsync();

                return ApiResponse<bool>.Success(
                    true,
                    "Policy type insurance mapping updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while updating PolicyTypeInsuranceMapping. Id: {Id}",
                    request.DTO.Id);

                return ApiResponse<bool>.Fail(
                    "An unexpected error occurred while updating mapping.");
            }
        }
    }


}


