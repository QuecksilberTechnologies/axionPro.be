using AutoMapper;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    public class DeletePolicyTypeCommandHandler : IRequestHandler<DeletePolicyTypeCommand, ApiResponse<bool>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePolicyTypeCommandHandler(IPolicyTypeRepository policyTypeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _policyTypeRepository = policyTypeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeletePolicyTypeCommand request, CancellationToken cancellationToken)
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


                // 🔹 Repository call
                var PolicyUpdated = await _policyTypeRepository.SoftDeletePolicyTypeAsync(request.DTO);

                if (PolicyUpdated == false)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Policy Type deletion failed.",
                        Data = false
                    };
                }

                // 🔹 Transaction Commit (UnitOfWork)
                await _unitOfWork.CommitAsync();

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Policy Type deleted successfully.",
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