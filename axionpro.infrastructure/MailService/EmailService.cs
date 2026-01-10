using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace axionpro.infrastructure.MailService
{

    public class EmailService : IEmailService
    {
        private readonly ITenantEmailConfigRepository _configRepo;
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly ILogger<EmailService> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        public EmailService(
            ITenantEmailConfigRepository configRepo,
            IEmailTemplateRepository templateRepo,
            ILogger<EmailService> logger, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _configRepo = configRepo;
            _templateRepo = templateRepo;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public Task<bool> SendEmailAsync(string toEmail, string subject, string body, string token, long? TenantId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendOtpEmailAsync(string toEmail, string subject, string body, long? TenantId, string otp)
        {
            throw new NotImplementedException();
        }
        //  public Task<bool> SendTemplatedEmailAsync(string templateCode, string toEmail, long? TenantId, Dictionary<string, string> placeholders)
        public async Task<bool> SendTemplatedEmailAsync(string templateCode, string toEmail, long? tenantId, Dictionary<string, string> placeholders)
        {
            try
            {
                // 1️⃣ Template
                var template = await _templateRepo.GetTemplateByCodeAsync(templateCode);
                if (template == null || !template.IsActive)
                {
                    _logger.LogWarning("Email template missing | Code={Code}", templateCode);
                    return false;
                }

                // 2️⃣ SMTP + Tenant
                var config = await _configRepo.GetActiveEmailConfigAsync(tenantId);
                if (config == null || config.Tenant == null)
                {
                    _logger.LogWarning("SMTP config missing | TenantId={TenantId}", tenantId);
                    return false;
                }

                var tenant = config.Tenant;

                // 3️⃣ Placeholders
                var finalPlaceholders = new Dictionary<string, string>
                {
                    ["TenantName"] = tenant.CompanyName,
                    ["TenantLogoUrl"] = tenant.TenantProfiles.Select(x => x.LogoUrl)
                                            .FirstOrDefault()
                                            ?? "https://cdn.axionpro.com/default-logo.png",
                    ["SupportEmail"] = tenant.TenantEmail,
                    ["Year"] = DateTime.UtcNow.Year.ToString()
                };

                foreach (var kv in placeholders)
                    finalPlaceholders[kv.Key] = kv.Value;

                // 4️⃣ Render
                var subject = EmailTemplateRenderer.RenderBody(
                    template.Subject ?? string.Empty,
                    finalPlaceholders);

                var body = EmailTemplateRenderer.RenderBody(
                    template.Body ?? string.Empty,
                    finalPlaceholders);

                // 5️⃣ Build message (🔥 CRITICAL FIX)
                var message = new MimeMessage();

                // 🚨 ALWAYS SAME AS SMTP USER
                message.From.Clear();
                message.From.Add(new MailboxAddress(
                    config.FromName ?? "AxionPro",
                    config.SmtpUsername));

                message.To.Add(MailboxAddress.Parse(toEmail));

                AddEmailAddresses(message.Cc, template.CcEmail);
                AddEmailAddresses(message.Bcc, template.BccEmail);

                message.Subject = subject;
                message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

                // 6️⃣ SMTP SEND (FULLY BLOCKING FLOW)
                using var smtp = new SmtpClient();



                await smtp.ConnectAsync(
                    config.SmtpHost,
                    config.SmtpPort ?? 587,
                    SecureSocketOptions.Auto);

                if (!smtp.IsConnected)
                    throw new Exception("SMTP not connected");

                await smtp.AuthenticateAsync(
                    config.SmtpUsername,
                    Decrypt(config.SmtpPasswordEncrypted ?? string.Empty));

                if (!smtp.IsAuthenticated)
                    throw new Exception("SMTP authentication failed");

                // 🔥 SERVER ACK WAIT
                smtp.Send(message);

                // 🔥 NOOP ensures server pipeline flushed
                await smtp.NoOpAsync();

                await smtp.DisconnectAsync(true);

                _logger.LogInformation(
                    "SMTP ACCEPTED | Template={Template} | To={To} | Server={Host} | mesg {message}",
                    templateCode,
                    toEmail,
                    config.SmtpHost, message);

                return true; // ✅ ACCEPTED BY SMTP SERVER
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex,
                    "SMTP REJECTED | Status={Status} | To={To}",
                    ex.StatusCode,
                    toEmail);

                return false;
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(ex,
                    "SMTP PROTOCOL FAILURE | To={To}",
                    toEmail);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "EMAIL FAILED | To={To}",
                    toEmail);

                return false;
            }
        }

        private string Decrypt(string encrypted)
        {
            // 🔐 real encryption service yahan inject karna
            return encrypted;
        }

        private void AddEmailAddresses(InternetAddressList list, string? emails)
        {
            if (string.IsNullOrWhiteSpace(emails))
                return;

            var splitEmails = emails
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e));

            foreach (var email in splitEmails)
            {
                try
                {
                    list.Add(MailboxAddress.Parse(email));
                }
                catch (FormatException)
                {
                    _logger.LogWarning("Invalid email skipped: {Email}", email);
                }
            }
        }

    }
}
