using AutoMapper;
using axionpro.application.DTOs.Verify;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace axionpro.application.Features.VerifyEmailCmd.Handlers
{
    public class VerifyEmailCommand : IRequest<ApiResponse<VerifyEmailResponseDTO>>
    {
        public VerifyEmailRequestDTO verifyEmailRequestDTO { get; set; }

        public VerifyEmailCommand(VerifyEmailRequestDTO verifyEmailRequestDTO)
        {
            this.verifyEmailRequestDTO = verifyEmailRequestDTO;
        }

    }
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponse<VerifyEmailResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<VerifyEmailCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<VerifyEmailResponseDTO>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.verifyEmailRequestDTO.Token))
                {
                    return new ApiResponse<VerifyEmailResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Token is required.",
                        Data = null
                    };
                }

                string tokenPayload = await _tokenService.GetUserInfoFromToken(request.verifyEmailRequestDTO.Token);

                if (string.IsNullOrEmpty(tokenPayload))
                {
                    return new ApiResponse<VerifyEmailResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid or expired token.",
                        Data = null
                    };
                }

                VerifyEmailResponseDTO userInfo = JsonConvert.DeserializeObject<VerifyEmailResponseDTO>(tokenPayload);

                if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
                {
                    return new ApiResponse<VerifyEmailResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Token does not contain valid user info.",
                        Data = null
                    };
                }

                // ✅ Directly try to get EmployeeId. If not found, email doesn't exist.
                LoginCredential employeeRecord = await _unitOfWork.UserLoginRepository.AuthenticateUser(userInfo.UserId);

                if (employeeRecord.EmployeeId == 0)
                {
                    return new ApiResponse<VerifyEmailResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Email not found in the system.",
                        Data = null
                    };
                }

                // Set empId in response DTO


                Tenant tenant = new Tenant();

                tenant.Id = employeeRecord.TenantId ?? 0;


                var tenantResult = await _unitOfWork.TenantRepository.UpdateTenantAsync(tenant);

                if (tenantResult == null)
                {
                    return new ApiResponse<VerifyEmailResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant update failed.",
                        Data = null
                    };
                }

                userInfo.EmployeeId = employeeRecord.EmployeeId;
                userInfo.TenantId = employeeRecord.TenantId;

                return new ApiResponse<VerifyEmailResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Email verified successfully.",
                    Data = userInfo
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in VerifyEmailCommandHandler.Handle");

                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<VerifyEmailResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred during email verification.",
                    Data = null
                };
            }
        }


    }

}
