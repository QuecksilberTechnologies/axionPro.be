using AutoMapper;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.Features.LeaveCmd.Commands;
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
    public class UpdateLeaveRuleCommandHandler : IRequestHandler<UpdateLeaveRuleCommand, ApiResponse<GetLeaveRuleResponseDTO>>
    {
     
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLeaveRuleCommandHandler(ILeaveRepository leaveRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
           ;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetLeaveRuleResponseDTO>> Handle(UpdateLeaveRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                LeaveRule leaveRuleEntity = _mapper.Map<LeaveRule>(request.DTO);
                long userId = request.DTO.EmployeeId;

                LeaveRule updatedEntity = await _unitOfWork.LeaveRuleRepository.UpdateLeaveRuleAsync(leaveRuleEntity, userId);

                if (updatedEntity == null)
                {
                    return new ApiResponse<GetLeaveRuleResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "No LeaveRule was updated.",
                        Data = null
                    };
                }

                GetLeaveRuleResponseDTO leaveRuleDTO = _mapper.Map<GetLeaveRuleResponseDTO>(updatedEntity);

                return new ApiResponse<GetLeaveRuleResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "LeaveRule updated successfully.",
                    Data = leaveRuleDTO
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetLeaveRuleResponseDTO>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }


    }


}
 