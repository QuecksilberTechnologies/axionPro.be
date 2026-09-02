using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IEmail
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string subject, string body, long? TenantId, string otp);
        Task<bool> SendTemplatedEmailAsync(string templateCode, string toEmail, long? TenantId, Dictionary<string, string> placeholders);
        
    }
}
