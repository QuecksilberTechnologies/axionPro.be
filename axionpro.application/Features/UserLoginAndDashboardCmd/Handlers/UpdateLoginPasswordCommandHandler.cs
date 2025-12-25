using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.UserLogin;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class UpdateLoginPasswordCommand : IRequest<ApiResponse<UpdatePasswordResponseDTO>>
    {
        public UpdatePasswordRequestDTO? DTO { get; set; }


        public UpdateLoginPasswordCommand(UpdatePasswordRequestDTO? setLoginPasswordRequest)
        {
            this.DTO = setLoginPasswordRequest;
        }



    }
    public class UpdateLoginPasswordCommandHandler : IRequestHandler<UpdateLoginPasswordCommand, ApiResponse<UpdatePasswordResponseDTO>>
{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateLoginPasswordCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPasswordService _passwordService;

        public UpdateLoginPasswordCommandHandler(
           IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateLoginPasswordCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService,
            IPasswordService passwordService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _passwordService = passwordService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<UpdatePasswordResponseDTO>> Handle(UpdateLoginPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
                await _unitOfWork.BeginTransactionAsync();
               
                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<UpdatePasswordResponseDTO>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                 request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                 request.DTO.Prop.TenantId = validation.TenantId;
                 request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                 request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey,
                  _idEncoderService
              );

                // 🔐 Authenticate user
                var user = await _unitOfWork.UserLoginRepository.AuthenticateUser(request.DTO.LoginId);

                if (user == null || string.IsNullOrWhiteSpace(user.Password))
                {
                    return ApiResponse<UpdatePasswordResponseDTO>.Fail(ConstantValues.invalidCredential);
                }

                // 🔑 Verify password
                if (!_passwordService.VerifyPassword(user.Password, request.DTO.OldPassword))
                {
                    return ApiResponse<UpdatePasswordResponseDTO>.Fail("Old password not corrected");
                }

                // Inside Handle() method 
                string? hashedPassword = _passwordService.HashPassword(request.DTO.NewPassword);
                if(string.IsNullOrEmpty(hashedPassword))
                {
                    _logger.LogWarning("Password hashing failed for LoginId: {LoginId}", request.DTO.LoginId);
                    return new ApiResponse<UpdatePasswordResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while processing your request. Please try again.",
                        Data = null
                    };
                }
                hashedPassword = hashedPassword.Trim();


               
                // Just in case EmployeeId not mapped from request, ensure it is set
                  

            bool isUpdated = await _unitOfWork.UserLoginRepository.UpdatePassword(request.DTO.Prop.EmployeeId, hashedPassword, request.DTO.Prop.UserEmployeeId);

                if (!isUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UpdatePasswordResponseDTO>.Fail("Password could not be updated.");
                }

                // ✅ COMMIT HERE
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<UpdatePasswordResponseDTO>.Success(
                    new UpdatePasswordResponseDTO { Success = true },
                    "Password has been set successfully."
                );


            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in SetLoginPasswordCommandHandler.Handle.");

            await _unitOfWork.RollbackTransactionAsync();

            return new ApiResponse<UpdatePasswordResponseDTO>
            {
                IsSucceeded = false,
                Message = "An error occurred while setting the password. Please try again later.",
                Data = null
            };
        }
    }
}

}
