using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using static axionpro.application.DTOS.InsurancePolicy.GetAlllnsurancePolicyResponseDTO;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetConsumedInsuranceListQuery : IRequest<ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>>
        {
        public GetAllInsurancePolicyRequestWithEmployeeIdDTO DTO { get; }

    

        public GetConsumedInsuranceListQuery(GetAllInsurancePolicyRequestWithEmployeeIdDTO dto)
        {
            DTO = dto;
           
        }
    }

    public class GetConsumedInsuranceListQueryHandler
   : IRequestHandler<GetConsumedInsuranceListQuery, ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetConsumedInsuranceListQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetConsumedInsuranceListQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetConsumedInsuranceListQueryHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>> Handle(
            GetConsumedInsuranceListQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetConsumedInsuranceList started");

                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                var result = await _unitOfWork.InsuranceRepository
                    .GetAllPolicyListWithConsumedDetailsAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.PolicyId,
                        request.DTO.IsActive);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetConsumedInsuranceList");
                throw;
            }
        }
    }
}
 