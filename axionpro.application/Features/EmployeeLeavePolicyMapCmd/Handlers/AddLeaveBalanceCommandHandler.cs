using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Handlers
{
    public class AddLeaveBalanceCommandHandler
        : IRequestHandler<AddLeaveBalanceCommand, ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddLeaveBalanceCommandHandler> _logger;

        public AddLeaveBalanceCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AddLeaveBalanceCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>> Handle(AddLeaveBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🟢 Starting AddLeaveBalanceCommandHandler for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);

                // ✅ Step 0: Input Validation
                if (request.DTO == null)
                {
                    _logger.LogWarning("❌ Request DTO is null");
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Request data cannot be null.",
                        Data = null
                    };
                }

                if (request.DTO.EmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Invalid EmployeeId: {EmployeeId}", request.DTO.EmployeeId);
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid EmployeeId.",
                        Data = null
                    };
                }

                if (request.DTO.TenantId <= 0)
                {
                    _logger.LogWarning("❌ Invalid TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid TenantId.",
                        Data = null
                    };
                }

                if (request.DTO.EmployeeLeavePolicyMappingId <= 0)
                {
                    _logger.LogWarning("❌ Invalid Leave Policy Mapping Id: {MappingId}", request.DTO.EmployeeLeavePolicyMappingId);
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid Leave Policy Mapping Id.",
                        Data = null
                    };
                }

                if (request.DTO.LeaveYear < DateTime.Now.Year - 1 || request.DTO.LeaveYear > DateTime.Now.Year + 1)
                {
                    _logger.LogWarning("❌ Invalid Leave Year: {LeaveYear}", request.DTO.LeaveYear);
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid Leave Year.",
                        Data = null
                    };
                }

                if (request.DTO.OpeningBalance < 0 || request.DTO.Availed < 0 || request.DTO.CurrentBalance < 0)
                {
                    _logger.LogWarning("❌ Negative balance values are not allowed for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);
                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Leave balances cannot be negative.",
                        Data = null
                    };
                }

                // ✅ Step 1: Check for Duplicate Record
                //var existingRecord = await _unitOfWork.Employees
                //    .CheckExistingLeaveBalanceAsync(request.DTO.EmployeeId, request.DTO.EmployeeLeavePolicyMappingId, request.DTO.LeaveYear);

                //if (existingRecord)
                //{
                //    _logger.LogWarning("⚠️ Duplicate leave balance found for EmployeeId: {EmployeeId}, MappingId: {MappingId}, Year: {Year}",
                //        request.DTO.EmployeeId, request.DTO.EmployeeLeavePolicyMappingId, request.DTO.LeaveYear);

                //    return new ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>
                //    {
                //        IsSucceeded = false,
                //        Message = "⚠️ Leave balance already exists for this employee and policy mapping.",
                //        Data = null
                //    };
                //}

                // ✅ Step 2: Begin Transaction
                await _unitOfWork.BeginTransactionAsync();
                // ✅ Step 4: Update LeaveBalanceAssigned Flag
                var isLeaveBalUpdated = await _unitOfWork.EmployeeLeaveRepository.UpdateIsLeaveBalanceAssigned(request.DTO.EmployeeLeavePolicyMappingId);

                if (!isLeaveBalUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("⚠️ Failed to update IsLeaveBalanceAssigned for MappingId: {MappingId}", request.DTO.EmployeeLeavePolicyMappingId);

                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Failed to update leave balance assigned flag.",
                        Data = null
                    };
                }

                // ✅ Step 3: Add Leave Balance Entry
                var addedLeaveBalance = await _unitOfWork.EmployeeLeaveRepository.AddLeaveBalanceToEmployee(request.DTO);

                if (addedLeaveBalance == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("⚠️ Failed to add Leave Balance record for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);

                    return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "❌ Failed to add Leave Balance for the employee.",
                        Data = null
                    };
                }

              
                // ✅ Step 5: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Leave Balance successfully added for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);

                // ✅ Step 6: Return Success Response
                return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                {
                    IsSucceeded = true,
                    Message = "✅ Leave Balance added successfully.",
                    Data = addedLeaveBalance
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ Exception while adding leave balance for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);

                return new ApiResponse<GetEmployeeLeavePolicyMappingReponseDTO>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error occurred while adding leave balance: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
