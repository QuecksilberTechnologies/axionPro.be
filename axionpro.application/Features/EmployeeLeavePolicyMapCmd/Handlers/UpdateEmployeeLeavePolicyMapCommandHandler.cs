// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Update Employee Leave Policy Map.
// ================================================================

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
using axionpro.application.DTOs.Employee;
using axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Update Employee Leave Policy Map.
    /// </summary>
public class UpdateEmployeeLeavePolicyMapCommand : IRequest<ApiResponse<bool>>
    {

        public UpdateEmployeeLeavePolicyMappingRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEmployeeLeavePolicyMapCommand"/> class.
        /// </summary>

        public UpdateEmployeeLeavePolicyMapCommand(UpdateEmployeeLeavePolicyMappingRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.EmployeeLeavePolicyMapCmd.Handlers
{
    /// <summary>
    /// Handles the request to Update Employee Leave Policy Map.
    /// </summary>
public class UpdateEmployeeLeavePolicyMapCommandHandler : IRequestHandler<UpdateEmployeeLeavePolicyMapCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseEmployeeRepository _employeeRepository;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEmployeeLeavePolicyMapCommandHandler"/> class.
        /// </summary>


        public UpdateEmployeeLeavePolicyMapCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBaseEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied UpdateEmployeeLeavePolicyMapCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(UpdateEmployeeLeavePolicyMapCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Begin Transaction
                await _unitOfWork.BeginTransactionAsync();

                // ✅ Step 1: Update EmployeeLeavePolicyMapping record
                bool isUpdated = await _unitOfWork.EmployeeLeaveRepository.UpdateEmployeeLeaveMap(request.DTO);

                if (!isUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "❌ Failed to update Employee Leave Policy Mapping record.",
                        Data = false
                    };
                }

                // ✅ Step 2: Update PolicyLeaveTypeMapping → IsEmployeeMapped = false (Unmap)
                bool isLeaveUnmapped = await _unitOfWork.LeaveRepository.UpdateLeaveAssignOnlyAsync(
                    request.DTO.PolicyLeaveTypeMappingId,
                    request.DTO.EmployeeId,
                    request.DTO.IsActive // means unmapped
                );

                if (!isLeaveUnmapped)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "❌ Failed to update Leave Policy mapping flag in PolicyLeaveTypeMapping table.",
                        Data = false
                    };
                }

                // ✅ Step 3: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ Employee Leave Policy Mapping updated and unmapped successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while updating Leave Policy Mapping: {ex.Message}",
                    Data = false
                };
            }
        }


    
        #endregion
}
}
