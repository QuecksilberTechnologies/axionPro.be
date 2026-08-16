// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Validate Otp.
// ================================================================

using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.ITokenService;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Validate Otp.
    /// </summary>
public class ValidateOtpCommand : IRequest<ApiResponse<bool>>
    {
        public ValidateOtpRequestDTO dTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateOtpCommand"/> class.
        /// </summary>


        public ValidateOtpCommand(ValidateOtpRequestDTO dto)
        {
            dTO = dto;
        }



    }

    #endregion
}

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    /// <summary>
    /// Handles the request to Validate Otp.
    /// </summary>
public class ValidateOtpCommandHandler : IRequestHandler<ValidateOtpCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
       
      
        private readonly ILogger<ValidateOtpCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateOtpCommandHandler"/> class.
        /// </summary>

     
        public ValidateOtpCommandHandler(IMapper mapper, IUnitOfWork unitOfWork  , ILogger<ValidateOtpCommandHandler> logger )
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            

        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied ValidateOtpCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(ValidateOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔐 Step 1: Validate if user exists
                long empId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(request.dTO.LoginId);
                _logger.LogInformation("Validation result for LoginId {LoginId}: EmployeeId = {empId}", request.dTO.LoginId, empId);

                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for LoginId: {LoginId}", request.dTO.LoginId);
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new UnauthorizedAccessException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.Unauthorized);
                }

                long userId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserCrendentialOnlyAsync(request.dTO.LoginId);
                GetMinimalEmployeeResponseDTO? empInfo = await _unitOfWork.Employees.GetSingleRecordAsync(empId,true);;

                // 🔍 Step 2: Check Existing OTP
                var existingOtpEntry = await _unitOfWork.ForgotPasswordOtpRepository.GetValidOtpByEmployeeIdAsync(userId, empInfo.TenantId);

                if (existingOtpEntry == null)
                {
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                // 🔄 Step 3: Validate OTP and Expiry
                if (existingOtpEntry.Otp != request.dTO.OTP)
                {
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                if (existingOtpEntry.OtpexpireDateTime <= DateTime.Now)
                {
                    throw new axionpro.application.Exceptions.ValidationErrorException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);
                }

                existingOtpEntry.IsUsed = false;
                existingOtpEntry.IsValidate = true;
                
                await _unitOfWork.ForgotPasswordOtpRepository.UpdateOTPAsync(existingOtpEntry);
                var isOtpUpdated=  await _unitOfWork.ForgotPasswordOtpRepository.UpdateOTPAsync(existingOtpEntry);
                if(isOtpUpdated)

                return ApiResponse<bool>.Success(true);
                throw new InvalidOperationException("The OTP validation state could not be persisted.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ValidateOtpCommand Handler.");
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }


    
        #endregion
}
}
