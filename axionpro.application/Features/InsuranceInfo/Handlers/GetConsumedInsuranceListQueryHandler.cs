// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles employee insurance-policy consumption query requests.
// ================================================================

using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using static axionpro.application.DTOS.InsurancePolicy.GetAlllnsurancePolicyResponseDTO;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request for insurance policies with employee consumption details.
    /// </summary>
    public class GetConsumedInsuranceListQuery : IRequest<ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>>
    {
        public GetAllInsurancePolicyRequestWithEmployeeIdDTO DTO { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetConsumedInsuranceListQuery"/> class.
        /// </summary>
        public GetConsumedInsuranceListQuery(GetAllInsurancePolicyRequestWithEmployeeIdDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles employee insurance-policy consumption query requests.
    /// </summary>
    public class GetConsumedInsuranceListQueryHandler :
        IRequestHandler<GetConsumedInsuranceListQuery, ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ILogger<GetConsumedInsuranceListQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetConsumedInsuranceListQueryHandler"/> class.
        /// </summary>
        public GetConsumedInsuranceListQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            ILogger<GetConsumedInsuranceListQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves consumed insurance-policy data and constructs the successful API response.
        /// </summary>
        public async Task<ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>> Handle(
            GetConsumedInsuranceListQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.DTO == null)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            if (request.DTO.PolicyId <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                request.DTO.EmployeeId,
                validation.Claims.TenantEncriptionKey,
                _idEncoderService);

            if (employeeId <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            var policies = await _unitOfWork.InsuranceRepository
                .GetAllPolicyListWithConsumedDetailsAsync(
                    employeeId,
                    request.DTO.PolicyId,
                    request.DTO.IsActive);

            _logger.LogInformation(
                "Retrieved {Count} consumed insurance policy records.",
                policies.Count);

            // Build the application response in the handler layer.
            return ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>.Success(
                policies,
                AppConstants.SuccessMessages.ConsumedInsurancePoliciesRetrieved);
        }

        #endregion
    }

    #endregion
}
