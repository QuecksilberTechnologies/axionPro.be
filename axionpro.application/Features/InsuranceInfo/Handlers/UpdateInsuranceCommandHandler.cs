using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
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

        public async Task<ApiResponse<bool>> Handle(
           UpdateInsurancePolicyCommand request,
           CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 UpdateInsurancePolicy started");

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
                //    Operations.Update);

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
                var policy = await _unitOfWork.InsuranceRepository
                    .GetByIdAsync(
                        request.DTO.Id,
                        validation.TenantId,
                        true);

                if (policy == null)
                    throw new ApiException("Insurance policy not found.", 404);

                // ===============================
                // 5️⃣ UPDATE FIELDS (SAFE)
                // ===============================

                if (request.DTO.PolicyTypeId.HasValue)
                    policy.PolicyTypeId = request.DTO.PolicyTypeId.Value;

                policy.CountryId = request.DTO.CountryId ?? policy.CountryId;

                if (!string.IsNullOrWhiteSpace(request.DTO.InsurancePolicyName))
                    policy.InsurancePolicyName = request.DTO.InsurancePolicyName;

                if (!string.IsNullOrWhiteSpace(request.DTO.InsurancePolicyNumber))
                    policy.InsurancePolicyNumber = request.DTO.InsurancePolicyNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.ProviderName))
                    policy.ProviderName = request.DTO.ProviderName;

                policy.StartDate = request.DTO.StartDate ?? policy.StartDate;
                policy.EndDate = request.DTO.EndDate ?? policy.EndDate;

                if (!string.IsNullOrWhiteSpace(request.DTO.AgentName))
                    policy.AgentName = request.DTO.AgentName;

                if (!string.IsNullOrWhiteSpace(request.DTO.AgentContactNumber))
                    policy.AgentContactNumber = request.DTO.AgentContactNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.AgentOfficeNumber))
                    policy.AgentOfficeNumber = request.DTO.AgentOfficeNumber;

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

                policy.IsActive = request.DTO.IsActive;

                if (!string.IsNullOrWhiteSpace(request.DTO.Remark))
                    policy.Remark = request.DTO.Remark;

                if (!string.IsNullOrWhiteSpace(request.DTO.Description))
                    policy.Description = request.DTO.Description;

                policy.UpdatedById = validation.UserEmployeeId;
                policy.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 6️⃣ SAVE
                // ===============================
                var updated = await _unitOfWork.InsuranceRepository.UpdateAsync(policy);

                if (!updated)
                    throw new ApiException("Failed to update insurance policy.", 401);

                _logger.LogInformation("✅ Insurance policy updated successfully");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Insurance policy updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UpdateInsurancePolicy failed");

                throw; // ✅ CRITICAL
            }
        }
    }

}
