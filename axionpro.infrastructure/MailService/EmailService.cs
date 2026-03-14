using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Configruations;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IRepositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Sockets;

namespace axionpro.infrastructure.MailService
{

    public class EmailService : IEmailService
    {
        private readonly ITenantEmailConfigRepository _configRepo;
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly ILogger<EmailService> _logger;
        
        private readonly EmailConfig _emailConfig;
        public EmailService(
            ITenantEmailConfigRepository configRepo,
            IEmailTemplateRepository templateRepo,
            ILogger<EmailService> logger, IOptions<EmailConfig> emailConfig)
        {
            _configRepo = configRepo;
            _templateRepo = templateRepo;
            _logger = logger;

            _emailConfig = emailConfig.Value;

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

        public async Task<bool> SendTemplatedEmailAsync(
    string templateCode,
    string toEmail,
    long? tenantId,
    Dictionary<string, string> placeholders)
        {
            try
            {
                // 1️⃣ Get Template
                var template = await _templateRepo.GetTemplateByCodeAsync(templateCode);
                if (template == null || !template.IsActive)
                {
                    _logger.LogWarning("Email template missing | Code={Code}", templateCode);
                    return false;
                }

                // 2️⃣ Get Tenant SMTP config
                var configDb = await _configRepo.GetActiveEmailConfigAsync(tenantId);
                if (configDb == null || configDb.Tenant == null)
                {
                    _logger.LogWarning("SMTP config missing | TenantId={TenantId}", tenantId);
                    return false;
                }

                var tenant = configDb.Tenant;

                // 3️⃣ Prepare placeholders
                var finalPlaceholders = new Dictionary<string, string>
                {
                    ["TenantName"] = tenant.CompanyName ?? "",
                    ["TenantLogoUrl"] = tenant.TenantProfiles
                                            .Select(x => x.LogoUrl)
                                            .FirstOrDefault()
                                            ?? "https://cdn.axionpro.com/default-logo.png",
                    ["SupportEmail"] = tenant.TenantEmail ?? "",
                    ["Year"] = DateTime.UtcNow.Year.ToString()
                };

                foreach (var kv in placeholders)
                    finalPlaceholders[kv.Key] = kv.Value;

                // 4️⃣ Render subject & body
                var subject = EmailTemplateRenderer.RenderBody(
                    template.Subject ?? string.Empty,
                    finalPlaceholders);

                var body = EmailTemplateRenderer.RenderBody(
                    template.Body ?? string.Empty,
                    finalPlaceholders);

                // 5️⃣ Build Email Message
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(
                    configDb.FromName ?? "AxionPro",
                    configDb.SmtpUsername));

                message.To.Add(MailboxAddress.Parse(toEmail));

                message.Subject = subject;
                message.Body = new BodyBuilder
                {
                    HtmlBody = body
                }.ToMessageBody();

                // 6️⃣ Send Email
                using var smtp = new SmtpClient();

                smtp.Timeout = 20000;

                await smtp.ConnectAsync(
                    "smtp-relay.brevo.com",
                    2525, // Render free plan compatible port
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _emailConfig.SMTPUserName,
                    _emailConfig.Secret);

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);

                _logger.LogInformation(
                    "SMTP ACCEPTED | Template={Template} | To={To}",
                    templateCode,
                    toEmail);

                return true;
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

        public static async Task CheckSmtpPorts()
        {
            string host = "smtp-relay.brevo.com";
            int[] ports = { 25, 465, 587, 2525 };

            foreach (var port in ports)
            {
                try
                {
                    using var client = new TcpClient();
                    var connectTask = client.ConnectAsync(host, port);

                    if (await Task.WhenAny(connectTask, Task.Delay(5000)) == connectTask)
                    {
                        Console.WriteLine($"✅ {host}:{port} OPEN");
                    }
                    else
                    {
                        Console.WriteLine($"❌ {host}:{port} BLOCKED");
                    }
                }
                catch
                {
                    Console.WriteLine($"❌ {host}:{port} FAILED");
                }
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
