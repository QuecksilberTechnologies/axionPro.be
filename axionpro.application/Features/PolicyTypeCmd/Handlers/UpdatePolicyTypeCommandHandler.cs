using AutoMapper;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    public class UpdatePolicyTypeCommandHandler : IRequestHandler<UpdatePolicyTypeCommand, ApiResponse<bool>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePolicyTypeCommandHandler(IPolicyTypeRepository policyTypeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _policyTypeRepository = policyTypeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdatePolicyTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Null check
                if (request.DTO == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. PolicyType data is required.",
                        Data = false
                    };
                }

                // 🔹 Business Validations
                if (request.DTO.TenantId <= 0 && request.DTO.EmployeeId <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "TenantId must be greater than zero.",
                        Data = false
                    };
                }

                if (string.IsNullOrWhiteSpace(request.DTO.PolicyName))
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "PolicyName is required.",
                        Data = false
                    };
                }

 

                // 🔹 Repository call
                var PolicyUpdated = await _policyTypeRepository.UpdatePolicyTypeAsync(request.DTO);

                if (PolicyUpdated == false)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Policy Type upadation failed.",
                        Data = false
                    };
                }

                // 🔹 Transaction Commit (UnitOfWork)
                await _unitOfWork.CommitAsync();

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Policy Type updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }
    
    }
}




 
