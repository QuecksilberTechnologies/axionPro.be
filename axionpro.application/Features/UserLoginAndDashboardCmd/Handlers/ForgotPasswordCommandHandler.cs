using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.Common.Helpers;
using axionpro.application.Interfaces.IEmail;
using Newtonsoft.Json.Linq;
using axionpro.domain.Entity;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IEmailService _emailService;
        public ForgotPasswordCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, ILogger<ForgotPasswordCommandHandler> logger, IEmailService emailService)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
            
        }
      
        public async Task<ApiResponse<ForgotPasswordResponseDTO>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
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
                    return ApiResponse<ForgotPasswordResponseDTO>.Fail("User is not authenticated or authorized to perform this action.");
                }
                long userId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserCrendentialOnlyAsync(request.dTO.LoginId);                

                var empInfo = await _unitOfWork.Employees.GetSingleRecordAsync(empId,true);;



                // 🔍 Check Existing OTP

                var existingOtpEntry = await _unitOfWork.ForgotPasswordOtpRepository.GetValidOtpByEmployeeIdAsync(userId, empInfo.TenantId);
                 
                var now = DateTime.Now;

                if (existingOtpEntry != null)
                {
                    var resendWaitTime = existingOtpEntry.CreatedDateTime.AddMinutes(2);

                    // OTP already sent within 2 minutes
                    if (existingOtpEntry.CreatedDateTime.AddMinutes(2) > now)
                    {
                        var responseApi = new ForgotPasswordResponseDTO
                        {
                            EmailResendAfterMinute = 2,
                            Message = "OTP already sent. Please wait 2 minutes without refreshing page."
                        };


                        return ApiResponse<ForgotPasswordResponseDTO>.Success(responseApi);
                    }

                    // ✅ 2. Delete expired OTP if 2 minutes have passed
                    if (existingOtpEntry.OtpexpireDateTime <= now || resendWaitTime <= now)
                    {
                        await _unitOfWork.ForgotPasswordOtpRepository.DeleteAsync(existingOtpEntry);
                    }
                }

                // ✅ Get Employee Info
                if (empInfo == null)
                {
                    _logger.LogWarning("Employee info not found for EmployeeId: {empId}", empId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<ForgotPasswordResponseDTO>.Fail("Employee information not found.");
                }

                string Otp = OtpHelper.Generate6CharAlphanumericOtp();
                // 🔐 OTP Generate
                //string otp = "A11111";

                // 🧍‍♂️ Full Name build
                string UserName = $"{empInfo.FirstName} {(empInfo.MiddleName ?? "")} {empInfo.LastName}".Trim();
                long? TenantId = empInfo.TenantId;

                // 📩 Get Template from DB
                var emailTemplate = await _unitOfWork.EmailTemplateRepository.GetTemplateByCodeAsync("FORGOT_PASSWORD");
                if (emailTemplate == null)
                {
                    _logger.LogError("Email template 'FORGOT_PASSWORD' not found.");
                    return ApiResponse<ForgotPasswordResponseDTO>.Fail("Failed to send OTP email: template missing.");
                }

                // 🧩 Placeholder Dictionary
                var placeholders = new Dictionary<string, string>
                  {
                   { "UserName", UserName },
                      { "Otp", Otp } // use 'Otp' with capital 'O' if template has {{Otp}}
                   };

                // 🛠️ Render subject and body from template
                string subject = EmailTemplateRenderer.RenderBody(emailTemplate.Subject ?? "Verification Email", placeholders);
                string body = EmailTemplateRenderer.RenderBody(emailTemplate.Body ?? "", placeholders);

                // 📬 Send Email
                bool isSent = await _emailService.SendOtpEmailAsync(
                    toEmail: request.dTO.LoginId,
                    subject: subject,
                    body: body,
                    otp: Otp, // If your service needs it, else remove
                    TenantId: TenantId
                );

                if (!isSent)
                {
                    _logger.LogWarning("OTP email send failed to {Email}", request.dTO.LoginId);
                    return ApiResponse<ForgotPasswordResponseDTO>.Fail("OTP email sending failed.");
                }
                //   string body = EmailTemplateRenderer.RenderBody(emailTemplate.Body ?? "", placeholders);
                //   string subject = EmailTemplateRenderer.RenderBody(emailTemplate.Subject ?? "", placeholders);

                  
                var newOtpEntry = new ForgotPasswordOTPDetail
                {
                    EmployeeId = empId,
                    Otp = Otp,
                    OtpexpireDateTime = DateTime.Now.AddMinutes(5),
                    TenantId = TenantId,
                    CreatedDateTime = DateTime.Now,
                    IsUsed = false,
                    UserId = userId,

                };

               

                _logger.LogInformation("OTP sent successfully to {Email}", request.dTO.LoginId);


             
                

                // 📨 Step 2: Save to database and get inserted Id
                long otpId = await _unitOfWork.ForgotPasswordOtpRepository.AddAsync(newOtpEntry);



                var response = new ForgotPasswordResponseDTO
                {
                    Message = "OTP sent successfully.",
                    EmailResendAfterMinute = 0,
                   
                };

                return ApiResponse<ForgotPasswordResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ForgotPassword Handler.");
                await _unitOfWork.RollbackTransactionAsync();

                return ApiResponse<ForgotPasswordResponseDTO>.Fail("An error occurred while processing the OTP request. Please try again later.");
            }
        }

      
    }

}
