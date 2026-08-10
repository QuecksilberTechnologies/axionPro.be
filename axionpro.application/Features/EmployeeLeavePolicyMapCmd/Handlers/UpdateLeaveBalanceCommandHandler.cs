// ===============================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates employee leave balances.
// ===============================================================

using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update an employee leave balance.
    /// </summary>
    public class UpdateLeaveBalanceCommand : IRequest<ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>>
    {
        public UpdateLeaveBalanceToEmployeeRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLeaveBalanceCommand"/> class.
        /// </summary>
        public UpdateLeaveBalanceCommand(UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceToEmployeeRequest)
        {
            this.DTO = updateLeaveBalanceToEmployeeRequest;
        }
    }

    #endregion
}

/// <summary>
/// Handles the UpdateLeaveBalanceCommand request.
/// </summary>
public class UpdateLeaveBalanceCommandHandler : IRequestHandler<UpdateLeaveBalanceCommand, ApiResponse<GetLeaveBalanceToEmployeeResponseDTO>>
{
#region Fields

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

#endregion

#region Constructor

/// <summary>
/// Initializes a new instance of the <see cref="UpdateLeaveBalanceCommandHandler"/> class.
/// </summary>
   

    public UpdateLeaveBalanceCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork
       )
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
       
    }

#endregion

#region Handler

/// <summary>
/// Handles the request asynchronously.
/// </summary>
/// <param name="request">The request to process.</param>
/// <param name="cancellationToken">A token used to cancel the operation.</param>
/// <returns>The response produced by handling the request.</returns>

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





#endregion
}
