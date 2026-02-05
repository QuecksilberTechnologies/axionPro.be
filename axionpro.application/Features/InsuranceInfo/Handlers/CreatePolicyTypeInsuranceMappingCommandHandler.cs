using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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
            try
            {
                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                {
                    return ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>
                        .Fail(validation.ErrorMessage);
                }

                // 2️⃣ DTO → Entity
                var entity = _mapper.Map<PolicyTypeInsuranceMapping>(request.DTO);

                entity.TenantId = validation.TenantId;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.SoftDeleteById = null;
                entity.SoftDeleteDateTime = null;

                // 3️⃣ Save
                var responseDto = await _repository.AddAsync(entity);

                if (responseDto == null)
                {
                    _logger.LogWarning(
                        "PolicyTypeInsuranceMapping creation failed. PolicyTypeId: {PolicyTypeId}, InsurancePolicyId: {InsurancePolicyId}",
                        entity.PolicyTypeId,
                        entity.InsurancePolicyId);

                    return ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>
                        .Fail("Failed to create policy type insurance mapping.");
                }

                await _unitOfWork.CommitAsync();

                // 4️⃣ Success
                return ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>
                    .Success(responseDto, "Policy type insurance mapping created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while creating PolicyTypeInsuranceMapping");

                return ApiResponse<GetPolicyTypeInsuranceMappingResponseDTO>
                    .Fail("An unexpected error occurred while creating mapping.");
            }
        }
    }
}

 



