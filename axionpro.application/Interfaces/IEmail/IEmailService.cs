using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IEmail
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, string token, long? TenantId);
        Task<bool> SendOtpEmailAsync(string toEmail, string subject, string body, long? TenantId, string otp);
        Task<bool> SendTemplatedEmailAsync(string templateCode, string toEmail, long? TenantId, Dictionary<string, string> placeholders);
        
    }
}
