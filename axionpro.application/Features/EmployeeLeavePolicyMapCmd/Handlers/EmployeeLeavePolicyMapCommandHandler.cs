using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Commands;
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

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Handlers
{
    public class EmployeeLeavePolicyMapCommandHandler
       : IRequestHandler<EmployeeLeavePolicyMapCommand, ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseEmployeeRepository _employeeRepository;

        public EmployeeLeavePolicyMapCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBaseEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
        }

        public async Task<ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>> Handle(EmployeeLeavePolicyMapCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Begin Transaction
                await _unitOfWork.BeginTransactionAsync();

                // ✅ Step 1: Update PolicyLeaveTypeMapping → IsEmployeeMapped = true
                bool isSuccess = await _unitOfWork.LeaveRepository.UpdateLeaveAssignOnlyAsync(
                    request.DTO.PolicyLeaveTypeMappingId,
                    request.DTO.EmployeeId,
                    true);

                if (!isSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "❌ Failed to update Leave Policy AssignOnly flag.",
                        Data = new List<GetEmployeeLeavePolicyMappingReponseDTO>()
                    };
                }

                // ✅ Step 2: Insert into EmployeeLeavePolicyMapping
                var employeeLeavePolicyMappingResponseDTOs =
                    await _unitOfWork.EmployeeLeaveRepository.CreateEmployeeLeaveMapAsync(request.DTO);

                // ✅ Step 3: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Leave Policy Assigned Successfully.",
                    Data = employeeLeavePolicyMappingResponseDTOs
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<List<GetEmployeeLeavePolicyMappingReponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while assigning Leave Policy: {ex.Message}",
                    Data = new List<GetEmployeeLeavePolicyMappingReponseDTO>()
                };
            }
        }
    }

}
