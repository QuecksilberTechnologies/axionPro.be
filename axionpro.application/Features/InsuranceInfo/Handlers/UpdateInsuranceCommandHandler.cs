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
    public class UpdateInsurancePolicyCommand
      : IRequest<ApiResponse<bool>>
    {
        public UpdateInsurancePolicyRequestDTO DTO { get; }

        public UpdateInsurancePolicyCommand(UpdateInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class UpdateInsurancePolicyCommandHandler
     : IRequestHandler<UpdateInsurancePolicyCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateInsurancePolicyCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateInsurancePolicyCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateInsurancePolicyCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(  UpdateInsurancePolicyCommand request,   CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate tenant/user
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // ⚠️ TEMP as you said (ignore IsActive filter)
                bool IsActive = true;

                // 2️⃣ Get existing policy
                var policy = await _unitOfWork.InsuranceRepository
                    .GetByIdAsync(
                        request.DTO.Id,
                        validation.TenantId,
                        IsActive
                    );

                if (policy == null)
                    return ApiResponse<bool>.Fail("Insurance policy not found.");

                // ===============================
                // 3️⃣ NULL-SAFE FIELD UPDATES
                // ===============================

                // 🔹 Classification
                if (request.DTO.PolicyTypeId.HasValue)
                {
                    policy.PolicyTypeId = request.DTO.PolicyTypeId.Value;
                }


                policy.CountryId = request.DTO.CountryId ?? policy.CountryId;

                // 🔹 Basic Info
                if (!string.IsNullOrWhiteSpace(request.DTO.InsurancePolicyName))
                    policy.InsurancePolicyName = request.DTO.InsurancePolicyName;

                if (!string.IsNullOrWhiteSpace(request.DTO.InsurancePolicyNumber))
                    policy.InsurancePolicyNumber = request.DTO.InsurancePolicyNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.ProviderName))
                    policy.ProviderName = request.DTO.ProviderName;

                // 🔹 Dates
                policy.StartDate = request.DTO.StartDate ?? policy.StartDate;
                policy.EndDate = request.DTO.EndDate ?? policy.EndDate;

                // 🔹 Agent
                if (!string.IsNullOrWhiteSpace(request.DTO.AgentName))
                    policy.AgentName = request.DTO.AgentName;

                if (!string.IsNullOrWhiteSpace(request.DTO.AgentContactNumber))
                    policy.AgentContactNumber = request.DTO.AgentContactNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.AgentOfficeNumber))
                    policy.AgentOfficeNumber = request.DTO.AgentOfficeNumber;

                // 🔹 Coverage Rules (NON-NULLABLE → always update)
                if (request.DTO.EmployeeAllowed.HasValue)
                    policy.EmployeeAllowed = request.DTO.EmployeeAllowed.Value;

                if (request.DTO.MaxSpouseAllowed.HasValue)
                    policy.MaxSpouseAllowed = request.DTO.MaxSpouseAllowed.Value;

                if (request.DTO.MaxChildAllowed.HasValue)
                    policy.MaxChildAllowed = request.DTO.MaxChildAllowed.Value;

                if (request.DTO.ParentsAllowed.HasValue)
                    policy.ParentsAllowed = request.DTO.ParentsAllowed.Value;

                if (request.DTO.InLawsAllowed.HasValue)
                    policy.InLawsAllowed = request.DTO.InLawsAllowed.Value;


                // 🔹 Status
                policy.IsActive = request.DTO.IsActive;

                // 🔹 Additional
                if (!string.IsNullOrWhiteSpace(request.DTO.Remark))
                    policy.Remark = request.DTO.Remark;

                if (!string.IsNullOrWhiteSpace(request.DTO.Description))
                    policy.Description = request.DTO.Description;

                // 🔹 Audit
                policy.UpdatedById = validation.UserEmployeeId;
                policy.UpdatedDateTime = DateTime.UtcNow;

                // 4️⃣ Save
                bool updated = await _unitOfWork.InsuranceRepository.UpdateAsync(policy);
                if (!updated)
                    return ApiResponse<bool>.Fail("Failed to update insurance policy.");

                return ApiResponse<bool>.Success(true, "Insurance policy updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateInsurancePolicy failed");
                return ApiResponse<bool>.Fail("Failed to update insurance policy.");
            }
        }

    }

}
