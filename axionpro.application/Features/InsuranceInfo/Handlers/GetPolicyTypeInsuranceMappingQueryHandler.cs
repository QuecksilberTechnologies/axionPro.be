using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetPolicyInsuranceRequestCommand : IRequest<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
    {
        public GetPolicyTypeInsuranceMappingRequestDTO DTO { get; }    

        public GetPolicyInsuranceRequestCommand(GetPolicyTypeInsuranceMappingRequestDTO dto)
        {
            DTO = dto;
           
        }
    }
    public class GetPolicyTypeInsuranceMappingQueryHandler  : IRequestHandler<GetPolicyInsuranceRequestCommand, ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPolicyTypeInsuranceMappingQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetPolicyTypeInsuranceMappingQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetPolicyTypeInsuranceMappingQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>> Handle(
     GetPolicyInsuranceRequestCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetPolicyInsuranceMapping started");

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
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                request.DTO.Props ??= new();

                request.DTO.Props.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Props.TenantId = validation.TenantId;

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
                var result = await _unitOfWork
                    .PolicyTypeInsuranceMappingRepository
                    .GetListAsync(request.DTO);

                var data = result?.Items ?? new List<GetPolicyTypeInsuranceMappingResponseDTO>();

                _logger.LogInformation("✅ Retrieved {Count} mapping records", data.Count);

                // ===============================
                // 6️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: data,
                        Message: "PolicyInsurance mapping info retrieved successfully.",
                        PageNumber: result?.PageNumber ?? 1,
                        PageSize: result?.PageSize ?? 10,
                        TotalRecords: result?.TotalCount ?? 0,
                        TotalPages: result?.TotalPages ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error in GetPolicyInsuranceMapping");

                throw; // ✅ CRITICAL
            }
        }
    }



}
