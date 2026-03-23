using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetPolicyInsuranceDetailRequestCommand
       : IRequest<ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>>
    {
        public GetPolicyTypeInsuranceMapDetailsRequestDTO DTO { get; }

        public GetPolicyInsuranceDetailRequestCommand(
            GetPolicyTypeInsuranceMapDetailsRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class GetPolicyTypeInsuranceMappingDetailQueryHandler
        : IRequestHandler<
            GetPolicyInsuranceDetailRequestCommand,
            ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPolicyTypeInsuranceMappingDetailQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _configuration;

        public GetPolicyTypeInsuranceMappingDetailQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetPolicyTypeInsuranceMappingDetailQueryHandler> logger,
            ICommonRequestService commonRequestService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>> Handle(
   GetPolicyInsuranceDetailRequestCommand request,
   CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetPolicyInsuranceDetail started");

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

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var list =
                    await _unitOfWork
                        .PolicyTypeInsuranceMappingRepository
                        .GetMapInsuranceDetailAsync(
                            request.DTO.PolicyTypeId,
                            request.DTO.IsActive);

                var data = list ?? new List<GetPolicyTypeInsuranceMapDetailsResponseDTO>();

                // ===============================
                // 5️⃣ BUILD FILE URL
                // ===============================
                string baseUrl =
                    _configuration["FileSettings:BaseUrl"] ?? string.Empty;

                foreach (var item in data)
                {
                    if (!string.IsNullOrWhiteSpace(item.FilePath))
                    {
                        item.Url =
                            $"{baseUrl.TrimEnd('/')}/{item.FilePath.TrimStart('/')}";
                    }
                }

                _logger.LogInformation("✅ Retrieved {Count} mapping records", data.Count);

                // ===============================
                // 6️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>
                    .Success(data, "Insurance policy mapping fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error in GetPolicyInsuranceDetail");

                throw; // ✅ CRITICAL
            }
        }
    }
}
