using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Token;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
            // 🔹 Transaction start (standard pattern)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🔹 CreateInsurance started");

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
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 4️⃣ DTO → ENTITY
                // ===============================
                var policy = _mapper.Map<InsurancePolicy>(request.DTO);

                policy.TenantId = validation.TenantId;
                policy.AddedById = validation.UserEmployeeId;
                policy.AddedDateTime = DateTime.UtcNow;
                policy.IsActive = true;
                policy.IsSoftDeleted = false;

                // ===============================
                // 5️⃣ SAVE DATA
                // ===============================
                var responseDto =
                    await _unitOfWork.InsuranceRepository.AddAsync(policy);

                if (responseDto == null)
                    throw new ApiException("Failed to create insurance policy.", 500);

                // ===============================
                // 6️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Insurance policy created successfully");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetInsurancePolicyResponseDTO>
                    .Success(responseDto, "Insurance policy created successfully.");
            }
            catch (Exception ex)
            {
                // 🔁 Rollback (only because transaction started)
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ CreateInsurance failed");

                throw; // ✅ CRITICAL
            }
        }
    }


}

