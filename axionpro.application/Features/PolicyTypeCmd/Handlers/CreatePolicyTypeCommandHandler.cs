using AutoMapper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    public class CreatePolicyTypeCommandHandler
        : IRequestHandler<CreatePolicyTypeCommand, ApiResponse<List<GetPolicyTypeResponseDTO>>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePolicyTypeCommandHandler(
            IPolicyTypeRepository policyTypeRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _policyTypeRepository = policyTypeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<GetPolicyTypeResponseDTO>>> Handle(
     CreatePolicyTypeCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Null check
                if (request.DTO == null)
                {
                    return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. PolicyType data is required.",
                        Data = null
                    };
                }

                // 🔹 Business Validations
                if (request.DTO.TenantId <= 0 || request.DTO.EmployeeId <= 0)
                {
                    return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "TenantId and EmployeeId must be greater than zero.",
                        Data = null
                    };
                }

                if (string.IsNullOrWhiteSpace(request.DTO.PolicyName))
                {
                    return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "PolicyName is required.",
                        Data = null
                    };
                }

                // 🔹 Map DTO to Entity
                var policyType = new PolicyType
                {
                    IsActive = request.DTO.IsActive ?? false,
                    PolicyName = request.DTO.PolicyName.Trim(),
                    TenantId = request.DTO.TenantId,
                    AddedById = request.DTO.EmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsSoftDelete = false,
                    Description = string.IsNullOrWhiteSpace(request.DTO.Description)
                                    ? null
                                    : request.DTO.Description.Trim()
                };

                // 🔹 Repository call (assuming this returns updated list after insert)
                List<GetPolicyTypeResponseDTO> createdList =  await _policyTypeRepository.CreatePolicyTypeAsync(policyType);

                if (createdList == null || !createdList.Any())
                {
                    return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Policy Type creation failed.",
                        Data = null
                    };
                }

                // 🔹 Commit transaction
                await _unitOfWork.CommitAsync();

                // 🔹 Return updated list
                return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Policy Type created successfully.",
                    Data = createdList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

    }
}
