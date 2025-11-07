using AutoMapper;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveRuleCmd.Commands;
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

namespace axionpro.application.Features.LeaveRuleCmd.Handlers
{
    public class CreateLeaveRuleCommandHandler : IRequestHandler<CreateLeaveRuleCommand, ApiResponse<List<GetLeaveRuleResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
     

        public CreateLeaveRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            
        }

        public async Task<ApiResponse<List<GetLeaveRuleResponseDTO>>> Handle(CreateLeaveRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. DTO → Entity
                var leaveRulePolicy = _mapper.Map<LeaveRule>(request.DTO);

                // 2. Common fields set karo
                leaveRulePolicy.AddedById = request.DTO.EmployeeId;
                leaveRulePolicy.AddedDateTime = DateTime.UtcNow;

                // 3. Repository call
                var leaveRulePolicies = await _unitOfWork.LeaveRuleRepository.CreateLeaveRuleAsync(leaveRulePolicy);

                if (leaveRulePolicies == null || !leaveRulePolicies.Any())
                {
                    return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "❌ Leave Policy creation failed.",
                        Data = new List<GetLeaveRuleResponseDTO>()
                    };
                }

                // 4. Commit transaction
                await _unitOfWork.CommitAsync();
 

                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Leave Policy created successfully.",
                    Data = leaveRulePolicies
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetLeaveRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while creating Leave Policy: {ex.Message}",
                    Data = new List<GetLeaveRuleResponseDTO>()
                };
            }
        }
    }
}
