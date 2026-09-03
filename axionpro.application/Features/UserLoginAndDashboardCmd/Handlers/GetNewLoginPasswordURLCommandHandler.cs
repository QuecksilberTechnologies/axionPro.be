// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates password-reset links and delegates failures to middleware.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class GetNewLoginPasswordURLCommand : IRequest<ApiResponse<GetNewPasswordLinkResponseDTO>>
    {
        public SetNewPasswordLinkRequestDTO DTO { get; set; }

        public GetNewLoginPasswordURLCommand(SetNewPasswordLinkRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetNewLoginPasswordURLCommandHandler
        : IRequestHandler<GetNewLoginPasswordURLCommand, ApiResponse<GetNewPasswordLinkResponseDTO>>
    {
        private const int PasswordResetLinkExpiryMinutes = 30;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetNewLoginPasswordURLCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IEmailService _emailService;

        public GetNewLoginPasswordURLCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetNewLoginPasswordURLCommandHandler> logger,
            ITokenService tokenService,
            IConfiguration config,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
            _config = config;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _emailService = emailService;
        }

        public async Task<ApiResponse<GetNewPasswordLinkResponseDTO>> Handle(
            GetNewLoginPasswordURLCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.DTO;

                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateTenantUserRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);
                long empId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(request.DTO.UserLoginId);
                _logger.LogInformation("Validation result for LoginId {LoginId}: EmployeeId = {empId}", request.DTO.UserLoginId, empId);

                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for UserLoginId: {LoginId}", request.DTO.UserLoginId);
                    // await _unitOfWork.RollbackTransactionAsync();
                    throw new UnauthorizedAccessException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.Unauthorized);
                }

                // 2️⃣ Get employee
                GetMinimalEmployeeResponseDTO emp =
                    await _unitOfWork.Employees.GetSingleRecordAsync(empId, true);

                if (emp == null)
                    throw new axionpro.application.Exceptions.NotFoundException(
                        axionpro.application.Constants.AppConstants.ErrorMessages.ResourceNotFound);

                // 3️⃣ Encode IDs
                string encryptedEmployeeId =
                    _idEncoderService.EncodeId_long(emp.Id, null);

                string encryptedTenantId =
                    _idEncoderService.EncodeId_long(emp.TenantId, null);

                // 4️⃣ Token Info
                var tokenInfo = new GetTokenInfoDTO
                {
                    EmployeeId = encryptedEmployeeId,
                    TenantId = encryptedTenantId,
                    Email = dto.UserLoginId,
                    FullName = emp.FirstName ?? "",
                    TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, ""),
                    IssuedAt = DateTime.UtcNow,
                    Expiry = DateTime.UtcNow.AddMinutes(PasswordResetLinkExpiryMinutes),
                    IsFirstLogin = false,
                    ClientType = "Web"
                };

                // 5️⃣ Generate token
                var token = await _tokenService.GenerateTenantToken(tokenInfo);
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new ApiException(
                        AppConstants.ErrorCodes.InternalServerError,
                        AppConstants.ErrorMessages.InternalServerError,
                        StatusCodes.Status500InternalServerError);
                }

                // 6️⃣ Build URL
                var baseUrl = _config["FrontEndWebURL:BaseUrl"]?.Trim();
                if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
                {
                    _logger.LogError("Password reset email was not sent because FrontEndWebURL:BaseUrl is invalid.");
                    throw new ApiException(
                        AppConstants.ErrorCodes.InternalServerError,
                        AppConstants.ErrorMessages.InternalServerError,
                        StatusCodes.Status500InternalServerError);
                }

                var resetUrl = $"{baseUrl.TrimEnd('/')}/auth/set-password?token={Uri.EscapeDataString(token)}";

                var emailSent = await _emailService.SendTemplatedEmailAsync(
                    ConstantValues.WelcomeEmail,
                    dto.UserLoginId,
                    emp.TenantId,
                    new Dictionary<string, string>
                    {
                        ["UserName"] = emp.FirstName ?? string.Empty,
                        ["VerificationUrl"] = resetUrl,
                        ["LinkExpiryMinutes"] = PasswordResetLinkExpiryMinutes.ToString()
                    });

                if (!emailSent)
                {
                    _logger.LogWarning(
                        "Password reset email was not accepted by SMTP | EmployeeId={EmployeeId} | TenantId={TenantId}",
                        emp.Id,
                        emp.TenantId);

                    throw new ApiException(
                        AppConstants.ErrorCodes.EmailDeliveryFailed,
                        AppConstants.ErrorMessages.PasswordResetEmailNotSent,
                        StatusCodes.Status503ServiceUnavailable);
                }

                _logger.LogInformation(
                    "Password reset email accepted by SMTP | EmployeeId={EmployeeId} | TenantId={TenantId}",
                    emp.Id,
                    emp.TenantId);
                               
                var response = new GetNewPasswordLinkResponseDTO
                {
                    UrlLink = resetUrl
                };

                return ApiResponse<GetNewPasswordLinkResponseDTO>
                    .Success(response, "Password link generated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating password reset URL");

                throw;
            }
        }
    }
}
