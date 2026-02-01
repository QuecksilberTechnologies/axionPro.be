using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Token;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class CreateInsuranceCommand : IRequest<ApiResponse<GetInsurancePolicyResponseDTO>>
    {
        public CreateInsurancePolicyRequestDTO DTO { get; set; }

        public CreateInsuranceCommand(CreateInsurancePolicyRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateInsuranceCommandHandler
       : IRequestHandler<CreateInsuranceCommand, ApiResponse<GetInsurancePolicyResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateInsuranceCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateInsuranceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateInsuranceCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetInsurancePolicyResponseDTO>> Handle(
        CreateInsuranceCommand request,
        CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                {
                    return ApiResponse<GetInsurancePolicyResponseDTO>
                        .Fail(validation.ErrorMessage);
                }

                // 2️⃣ Map DTO → Entity
                var policy = _mapper.Map<InsurancePolicy>(request.DTO);

                policy.TenantId = validation.TenantId;
                policy.AddedById = validation.UserEmployeeId;
                policy.AddedDateTime = DateTime.UtcNow;
                policy.IsActive = true;
                policy.IsSoftDeleted = false;

                // 3️⃣ SAVE + JOINED RESULT (IMPORTANT)
                var responseDto =
                    await _unitOfWork.InsuranceRepository.AddAsync(policy);

                // 🔥 RETURN CHECK
                if (responseDto == null)
                {
                    _logger.LogWarning(
                        "InsurancePolicy creation failed. PolicyName: {PolicyName}",
                        policy.InsurancePolicyName);

                    return ApiResponse<GetInsurancePolicyResponseDTO>
                        .Fail("Failed to create insurance policy.");
                }

                // ❌ NO CommitTransactionAsync() here
                // Repository already did SaveChanges()

                // 4️⃣ SUCCESS
                return ApiResponse<GetInsurancePolicyResponseDTO>
                    .Success(responseDto, "Insurance policy created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateInsurance failed");

                return ApiResponse<GetInsurancePolicyResponseDTO>
                    .Fail("Failed to create insurance policy.");
            }
        }

    }


}

