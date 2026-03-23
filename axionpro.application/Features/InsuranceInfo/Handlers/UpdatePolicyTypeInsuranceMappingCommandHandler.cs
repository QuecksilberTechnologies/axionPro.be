using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

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
                _logger.LogInformation("🔹 UpdatePolicyTypeInsuranceMapping started");

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
                var mapping = await _unitOfWork
                    .PolicyTypeInsuranceMappingRepository
                    .GetByIdAsync(request.DTO.Id, true);

                if (mapping == null)
                    throw new ApiException(
                        "Policy type insurance mapping not found.", 404);

                // ===============================
                // 5️⃣ UPDATE FIELDS
                // ===============================
                if (request.DTO.PolicyTypeId.HasValue)
                    mapping.PolicyTypeId = request.DTO.PolicyTypeId.Value;

                if (request.DTO.InsurancePolicyId.HasValue)
                    mapping.InsurancePolicyId = request.DTO.InsurancePolicyId.Value;

                if (request.DTO.IsActive.HasValue)
                    mapping.IsActive = request.DTO.IsActive.Value;

                // ===============================
                // 6️⃣ AUDIT
                // ===============================
                mapping.UpdatedById = validation.UserEmployeeId;
                mapping.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 7️⃣ SAVE
                // ===============================
                var updated = await _unitOfWork
                    .PolicyTypeInsuranceMappingRepository
                    .UpdateAsync(mapping);

                if (!updated)
                    throw new ApiException(
                        "Failed to update policy type insurance mapping.", 500);

                _logger.LogInformation("✅ PolicyTypeInsuranceMapping updated successfully");

                // ===============================
                // 8️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>.Success(
                    true,
                    "Policy type insurance mapping updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ UpdatePolicyTypeInsuranceMapping failed");

                throw; // ✅ CRITICAL
            }
        }
    }


}


