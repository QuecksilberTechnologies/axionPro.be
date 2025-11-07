using axionpro.application.Interfaces.IEmail;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using axionpro.application.Interfaces.IRepositories;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using axionpro.application.Common.Helpers;
using Microsoft.AspNetCore.Hosting.Server;

namespace axionpro.infrastructure.MailService
{
    public class EmailService : IEmailService
    {
        private readonly ITenantEmailConfigRepository _configRepo;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public EmailService(
            ITenantEmailConfigRepository configRepo,
            IEmailTemplateRepository emailTemplateRepository,
            ILogger<EmailService> logger)
        {
            _configRepo = configRepo;
            _emailTemplateRepository = emailTemplateRepository;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string token, long? tenantId)
        {
            try
            {
                tenantId = 322;
                subject = "Verification Email";
                var config = await _configRepo.GetActiveEmailConfigAsync(tenantId);

                if (config == null)
                {
                    _logger.LogWarning("No active SMTP config found for TenantId: {TenantId}", tenantId);
                    return false;
                }

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Axion-Pro Verification", config.FromEmail ?? "hr@quecksilber.in"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                // 🔁 Replace placeholders inside the HTML body
                string finalBody = body
                    .Replace("{{UserName}}", toEmail.Split('@')[0])  // OR pass name as extra parameter
                    .Replace("{{Token}}", token);
                email.Body = new TextPart("html")
                {
                    Text = finalBody
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(config.SmtpHost ?? "smtpout.secureserver.net", config.SmtpPort ?? 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(config.SmtpUsername ?? "hr@quecksilber.in", config.SmtpPasswordEncrypted ?? "Abhi@123#$%");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {ToEmail} for TenantId: {TenantId}", toEmail, tenantId);
                return false;
            }
        }



        public async Task<bool> SendOtpEmailAsync(string toEmail, string subject, string body, long? tenantId, string otp)
        {
            try
            {

                tenantId = 322;
                subject = "Verification Email";
                var config = await _configRepo.GetActiveEmailConfigAsync(tenantId);

                if (config == null)
                {
                    _logger.LogWarning("No active SMTP config found for TenantId: {TenantId}", tenantId);
                     return false;
                }

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Axion-Pro Verification", config.FromEmail ?? "hr@quecksilber.in"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                // 🔁 Replace placeholders inside the HTML body
                string finalBody = body
                    .Replace("{{UserName}}", toEmail.Split('@')[0])  // OR pass name as extra parameter
                    .Replace("{{OTP}}", otp);
                email.Body = new TextPart("html")
                {
                    Text = finalBody
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(config.SmtpHost ?? "smtpout.secureserver.net", config.SmtpPort ?? 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(config.SmtpUsername ?? "hr@quecksilber.in", config.SmtpPasswordEncrypted ?? "Abhi@123#$%");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {ToEmail} for TenantId: {TenantId}", toEmail, tenantId);
                return false;
            }
        }


        public async Task<bool> SendTemplatedEmailAsync(string templateCode, string toEmail, long? tenantId, Dictionary<string, string> placeholders)
        {
            try
            {
                var template = await _emailTemplateRepository.GetTemplateByCodeAsync(templateCode);
                if (template == null)
                {
                    _logger.LogWarning("Template not found: {TemplateCode}", templateCode);
                    return false;
                }
                tenantId = 322;

                string body = EmailTemplateRenderer.RenderBody(template.Body ?? "", placeholders);
                string subject = EmailTemplateRenderer.RenderBody(template.Subject ?? "", placeholders);

                var config = await _configRepo.GetActiveEmailConfigAsync(tenantId);
                if (config == null)
                {
                    _logger.LogWarning("SMTP config not found for tenantId: {TenantId}", tenantId);
                    return false;
                }

                /*
                                var email = new MimeMessage();
                                email.From.Add(new MailboxAddress("Test Sender", "hr@quecksilber.in"));
                                email.To.Add(MailboxAddress.Parse("mca.deepesh@gmail.com"));
                                email.Subject = "Test Email";
                                email.Body = new TextPart("plain") { Text = "Hello, this is a test email." };

                                using var smtp = new SmtpClient();
                                                      
                                await smtp.ConnectAsync("smtpout.secureserver.net", 587, SecureSocketOptions.StartTls);
                                await smtp.AuthenticateAsync("hr@quecksilber.in", "Abhi@123#$%");

                                await smtp.SendAsync(email);
                                await smtp.DisconnectAsync(true);


                  //        var email = new MimeMessage();
        //        email.From.Add(new MailboxAddress("Test Sender", "hr@quecksilber.in"));
        //        email.To.Add(MailboxAddress.Parse("mca.deepesh@gmail.com"));
        //        email.Subject = "Test Email";

        //        // HTML content
        //        email.Body = new TextPart("html")
        //        {
        //            Text = @"
        //<html>
        //    <body style='font-family: Arial, sans-serif; padding: 20px;'>
        //        <h2 style='color: #2E86C1;'>Hello Deepesh ji,</h2>
        //        <p style='font-size: 16px;'>
        //            This is a <strong>test email</strong> sent using <em>MailKit</em> and <em>MimeKit</em>.
        //        </p>
        //        <p style='font-size: 14px; color: gray;'>
        //            Regards,<br/>
        //            <b>axionpro Team</b>
        //        </p>
        //    </body>
        //</html>"
        //        };



                */
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(template.FromName ?? "", template.FromEmail ?? config.FromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtpout.secureserver.net", 587, SecureSocketOptions.StartTls);
                //   await smtp.ConnectAsync(config.SmtpHost, config.SmtpPort ?? 587, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(config.SmtpUsername, config.SmtpPasswordEncrypted);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent using template {TemplateCode} to {ToEmail}", templateCode, toEmail);
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send templated email: {TemplateCode}", templateCode);
                return false;
            }
        }

        private string Decrypt(string? encrypted)
        {
            // 🔐 TODO: Replace with actual decryption logic
            return encrypted ?? "";
        }
    }
}
