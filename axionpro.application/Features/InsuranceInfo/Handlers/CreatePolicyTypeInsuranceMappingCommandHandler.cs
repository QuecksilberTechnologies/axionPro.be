using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{


    public class CreatePolicyTypeInsuranceMappingCommand : IRequest<ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>>
    {
        public CreatePolicyTypeInsuranceMappingRequetDTO DTO { get; set; }

        public CreatePolicyTypeInsuranceMappingCommand(CreatePolicyTypeInsuranceMappingRequetDTO dto)
        {
            DTO = dto;
        }
    }
    public class CreatePolicyTypeInsuranceMappingCommandHandler
      : IRequestHandler<
          CreatePolicyTypeInsuranceMappingCommand,
          ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>>
    {
        private readonly IPolicyTypeInsuranceMappingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePolicyTypeInsuranceMappingCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreatePolicyTypeInsuranceMappingCommandHandler(
            IPolicyTypeInsuranceMappingRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreatePolicyTypeInsuranceMappingCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>> Handle(
     CreatePolicyTypeInsuranceMappingCommand request,
     CancellationToken cancellationToken)
        {
            // 🔹 Transaction start (standardized)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🔹 CreatePolicyTypeInsuranceMapping started");

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
                var entity = _mapper.Map<PolicyTypeInsuranceMapping>(request.DTO);

                entity.TenantId = validation.TenantId;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.SoftDeleteById = null;
                entity.SoftDeleteDateTime = null;

                // ===============================
                // 5️⃣ SAVE
                // ===============================
                var responseDto = await _repository.AddAsync(entity);

                if (responseDto == null)
                    throw new ApiException(
                        "Failed to create policy type insurance mapping.", 500);

                // ===============================
                // 6️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ PolicyTypeInsuranceMapping created successfully");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>
                    .Success(responseDto,
                        "Policy type insurance mapping created successfully.");
            }
            catch (Exception ex)
            {
                // 🔁 Rollback (IMPORTANT)
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex,
                    "❌ CreatePolicyTypeInsuranceMapping failed");

                throw; // ✅ CRITICAL
            }
        }
    }
}

 



