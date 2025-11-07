using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class UpdateLoginPasswordCommandHandler : IRequestHandler<UpdateLoginPasswordCommand, ApiResponse<UpdateLoginPasswordResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<UpdateLoginPasswordCommandHandler> _logger;

    public UpdateLoginPasswordCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<UpdateLoginPasswordCommandHandler> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<UpdateLoginPasswordResponseDTO>> Handle(UpdateLoginPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var loginId = request.setLoginPasswordRequest.LoginId;

            long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(loginId);

            _logger.LogInformation("Validation result for LoginId {LoginId}: {EmpId}", loginId, empId);

            if (empId < 1)
            {
                _logger.LogWarning("User not found or not authorized.");
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "User is not authenticated or authorized to perform this action.",
                    Data = null
                };
            }

                //   var loginCredential = _mapper.Map<LoginCredential>(request.setLoginPasswordRequest);
                LoginCredential loginCredential = new LoginCredential();
                loginCredential.EmployeeId = empId; ;
                loginCredential.TenantId = request.setLoginPasswordRequest.TenantId;
                loginCredential.LoginId = request.setLoginPasswordRequest.LoginId;
                loginCredential.Password = request.setLoginPasswordRequest.Password;
                // Just in case EmployeeId not mapped from request, ensure it is set
                  

            bool isUpdated = await _unitOfWork.UserLoginRepository.UpdateNewPassword(loginCredential);

            if (!isUpdated)
            {
                _logger.LogWarning("Password update failed for LoginId: {LoginId}", loginId);
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<UpdateLoginPasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Password could not be updated. Please check if this is your first login.",
                    Data = null
                };
            }

            var response = new UpdateLoginPasswordResponseDTO
            {
                Success = true,
               
            };

            return new ApiResponse<UpdateLoginPasswordResponseDTO>
            {
                IsSucceeded = true,
                Message = "Password has been set successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in SetLoginPasswordCommandHandler.Handle.");

            await _unitOfWork.RollbackTransactionAsync();

            return new ApiResponse<UpdateLoginPasswordResponseDTO>
            {
                IsSucceeded = false,
                Message = "An error occurred while setting the password. Please try again later.",
                Data = null
            };
        }
    }
}

}