using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.UserLogin;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{


    public class NewLoginPasswordCommand : IRequest<ApiResponse<UpdatePasswordResponseDTO>>
    {
        //till completed
        public NewLoginPasswordRequestDTO DTO { get; set; }
        public NewLoginPasswordCommand(NewLoginPasswordRequestDTO dto)
        {
            this.DTO = dto;
        }
     
    }
    public class NewLoginPasswordCommandHandler : IRequestHandler<NewLoginPasswordCommand, ApiResponse<UpdatePasswordResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NewLoginPasswordCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPasswordService _passwordService;

        public NewLoginPasswordCommandHandler(
           IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<NewLoginPasswordCommandHandler> logger,
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

        public async Task<ApiResponse<UpdatePasswordResponseDTO>> Handle(
  NewLoginPasswordCommand request,
  CancellationToken cancellationToken)
        {
            var dto = request.DTO;

            // 1️⃣ Validation
            if (string.IsNullOrWhiteSpace(dto.Token))
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Token is required.");

            if (dto.NewPassword != dto.ConfirmPassword)
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Password and confirm password do not match.");

             
            var claims = RequestCommonHelper.ValidateAndExtractClaims(dto.Token, _config);

            if (claims == null)
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Invalid or expired token.");

      

            int tokenPurpose = _idEncoderService.DecodeId_int(EncryptionSanitizer.CleanEncodedInput(claims.TokenPurpose), claims.TenantEncriptionKey);
            

            if (tokenPurpose != ConstantValues.SetPassword)
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Invalid token purpose.");


            // 3️⃣ Decode EmployeeId
            long employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                claims.EmployeeId!,
                claims.TenantEncriptionKey!,
                _idEncoderService);

           

            // 4️⃣ Fetch login credential
            var loginCredential =
                await _unitOfWork.UserLoginRepository.GetEmployeeIdByUserLogin(claims.Email.Trim());

            if (loginCredential == null)
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Login account not found.");
            // Inside Handle() method 
            if (loginCredential.IsPasswordChangeRequired == false || loginCredential.HasFirstLogin==false)
            {
                return ApiResponse<UpdatePasswordResponseDTO>
                    .Fail("Token expired");
            }
            string? hashedPassword = _passwordService.HashPassword(request.DTO.NewPassword);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                _logger.LogWarning("Password hashing failed for LoginId: {LoginId}", employeeId);
                return new ApiResponse<UpdatePasswordResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing your request. Please try again.",
                    Data = null
                };
            }
            hashedPassword = hashedPassword.Trim();



            // Just in case EmployeeId not mapped from request, ensure it is set


            bool isUpdated = await _unitOfWork.UserLoginRepository.UpdatePassword(employeeId, hashedPassword, employeeId);

            if (!isUpdated)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<UpdatePasswordResponseDTO>.Fail("Password could not be updated.");
            }


            await _unitOfWork.CommitAsync();

            return ApiResponse<UpdatePasswordResponseDTO>.Success(
               new UpdatePasswordResponseDTO { Success = true },
               "Password has been set successfully."
           );

        }


    }
}
