using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetAllInsuranceQuery  : IRequest<ApiResponse<List<GetAlllnsurancePolicyResponseDTO>>>
        {
        public GetAllInsurancePolicyRequestDTO DTO { get; }

    

        public GetAllInsuranceQuery(GetAllInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
           
        }
    }
    public class GetAllInsuranceQueryHandler
      : IRequestHandler<
          GetAllInsuranceQuery,
          ApiResponse<List<GetAlllnsurancePolicyResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllInsuranceQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllInsuranceQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllInsuranceQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }
        public async Task<ApiResponse<List<GetAlllnsurancePolicyResponseDTO>>> Handle(
    GetAllInsuranceQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetAllInsurance started");

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
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var result = await _unitOfWork.InsuranceRepository
                    .GetAllListAsync(request.DTO.PolicyId, request.DTO.IsActive);

                var data = result?.Data ?? new List<GetAlllnsurancePolicyResponseDTO>();

                _logger.LogInformation("✅ Retrieved {Count} insurance policies", data.Count);

                // ===============================
                // 5️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetAlllnsurancePolicyResponseDTO>>
                    .Success(data, "Insurance policies fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllInsurance");

                throw; // ✅ CRITICAL
            }
        }

    }







}
