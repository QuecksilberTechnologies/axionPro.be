using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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
                // ==================================================
                // 1️⃣ Common validation (Tenant / User)
                // ==================================================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                {
                    return ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>
                        .Fail(validation.ErrorMessage);
                }

             

                // ==================================================
                // 3️⃣ Repository Call
                // ==================================================
                var list =
                    await _unitOfWork
                        .PolicyTypeInsuranceMappingRepository
                        .GetMapInsuranceDetailAsync(
                            request.DTO.PolicyTypeId,
                            request.DTO.IsActive);

                // ==================================================
                // 4️⃣ No data found
                // ==================================================
                if (list == null || !list.Any())
                {
                    _logger.LogInformation(
                        "No insurance mapping found. PolicyTypeId: {PolicyTypeId}",
                        request.DTO.PolicyTypeId);

                    return ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>
                        .Fail("No insurance policies mapping found.");
                }

                // ==================================================
                // 5️⃣ Build File URL (BaseUrl from appsettings)
                // ==================================================
                string baseUrl =
                    _configuration["FileSettings:BaseUrl"] ?? string.Empty;

                foreach (var item in list)
                {
                    if (!string.IsNullOrWhiteSpace(item.FilePath))
                    {
                        item.Url =
                            $"{baseUrl.TrimEnd('/')}/{item.FilePath.TrimStart('/')}";
                    }
                }

                // ==================================================
                // 6️⃣ Success Response
                // ==================================================
                return ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>
                    .Success(list, "Insurance policy mapping fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching PolicyType–Insurance mapping");

                return ApiResponse<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>>
                    .Fail("An unexpected error occurred while fetching insurance mappings.");
            }
        }
    }
}
