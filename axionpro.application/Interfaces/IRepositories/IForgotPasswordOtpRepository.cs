using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;
 // ← jahan ForgotPasswordOtp entity rakhi hai

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IForgotPasswordOtpRepository
    {
        Task<ForgotPasswordOtpdetail?> GetValidOtpByEmployeeIdAsync(long employeeId, long? TenantId);
        Task<ForgotPasswordOtpdetail?> GetOtpValidateTrueAndUsedFalseByEmployeeIdAsync(long employeeId, long? TenantId);


        Task<long> AddAsync(ForgotPasswordOtpdetail otp);
        Task DeleteAsync(ForgotPasswordOtpdetail otp);

        Task<ForgotPasswordOtpdetail?> GetByOtpAndEmployeeIdAsync(string otp, long employeeId);

        Task<bool> UpdateOTPAsync(ForgotPasswordOtpdetail otp); // ✅ New method
    }
}
