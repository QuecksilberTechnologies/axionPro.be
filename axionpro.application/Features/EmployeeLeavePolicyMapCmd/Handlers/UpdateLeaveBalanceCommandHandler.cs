using AutoMapper;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;

public class UpdateLeaveBalanceCommandHandler : IRequestHandler<UpdateLeaveBalanceCommand, ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
   

    public UpdateLeaveBalanceCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork
       )
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
       
    }

    public async Task<ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>> Handle(UpdateLeaveBalanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ Begin Transaction
            await _unitOfWork.BeginTransactionAsync();

            // ✅ Step 1: Update EmployeeLeaveBalance record
            var updatedLeaveBalance = await _unitOfWork.Employees.UpdateLeaveBalanceToEmployee(request.DTO);

            if (updatedLeaveBalance == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "❌ Failed to update Employee Leave Balance.",
                    Data = null
                };
            }

            // ✅ Step 2: Commit Transaction
            await _unitOfWork.CommitTransactionAsync();

            return new ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>
            {
                IsSucceeded = true,
                Message = "✅ Employee Leave Balance updated successfully.",
                Data = updatedLeaveBalance
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            return new ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>
            {
                IsSucceeded = false,
                Message = $"❌ Error while updating Employee Leave Balance: {ex.Message}",
                Data = null
            };
        }
    }




}

