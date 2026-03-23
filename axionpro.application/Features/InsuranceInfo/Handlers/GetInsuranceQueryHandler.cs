using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetInsuranceQuery  : IRequest<ApiResponse<List<GetInsurancePolicyResponseDTO>>> 
        {
        public GetInsurancePolicyRequestDTO DTO { get; }

    

        public GetInsuranceQuery(GetInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
           
        }
    }
    public class GetInsuranceListQueryHandler
      : IRequestHandler<
          GetInsuranceQuery,
          ApiResponse<List<GetInsurancePolicyResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetInsuranceListQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetInsuranceListQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetInsuranceListQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetInsurancePolicyResponseDTO>>> Handle(
    GetInsuranceQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetInsurance started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

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

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ PAGING DEFAULTS
                // ===============================
                request.DTO.PageNumber = request.DTO.PageNumber > 0 ? request.DTO.PageNumber : 1;
                request.DTO.PageSize = request.DTO.PageSize > 0 ? request.DTO.PageSize : 10;
                request.DTO.SortOrder = string.IsNullOrWhiteSpace(request.DTO.SortOrder)
                    ? "desc"
                    : request.DTO.SortOrder.ToLower();

                // ===============================
                // 5️⃣ FETCH DATA
                // ===============================
                var result =
                    await _unitOfWork.InsuranceRepository.GetListAsync(request.DTO);

                var data = result?.Items ?? new List<GetInsurancePolicyResponseDTO>();

                _logger.LogInformation(
                    "✅ Retrieved {Count} insurance policies",
                    data.Count);

                // ===============================
                // 6️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetInsurancePolicyResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: data,
                        Message: "Insurance info retrieved successfully.",
                        PageNumber: result?.PageNumber ?? 1,
                        PageSize: result?.PageSize ?? 10,
                        TotalRecords: result?.TotalCount ?? 0,
                        TotalPages: result?.TotalPages ?? 0
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error in GetInsurance");

                throw; // ✅ CRITICAL
            }
        }
    }



}
