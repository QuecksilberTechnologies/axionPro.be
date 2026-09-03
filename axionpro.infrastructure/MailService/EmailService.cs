using axionpro.application.Common.Helpers;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IRepositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Sockets;

namespace axionpro.infrastructure.MailService
{

    public class EmailService : IEmailService
    {
        private readonly ITenantEmailConfigRepository _configRepo;
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            ITenantEmailConfigRepository configRepo,
            IEmailTemplateRepository templateRepo,
            ILogger<EmailService> logger)
        {
            _configRepo = configRepo;
            _templateRepo = templateRepo;
            _logger = logger;
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

                // Use the tenant's database configuration only.  Trim incidental
                // whitespace (including copied zero-width/BOM characters) before SMTP use.
                var smtpHost = CleanDatabaseValue(configDb.SmtpHost);
                var smtpPort = configDb.SmtpPort.GetValueOrDefault();
                var smtpUsername = CleanDatabaseValue(configDb.SmtpUsername);
                var smtpSecret = CleanDatabaseValue(configDb.SecrateKey);
                var fromName = CleanDatabaseValue(configDb.FromName);
                var fromEmail = CleanDatabaseValue(configDb.FromEmail);
                var recipientEmail = CleanDatabaseValue(toEmail);

                if (smtpHost is null || smtpUsername is null || smtpSecret is null ||
                    fromName is null || fromEmail is null || recipientEmail is null ||
                    smtpPort < 1 || smtpPort > 65535)
                {
                    _logger.LogWarning(
                        "SMTP configuration is incomplete or invalid | TenantId={TenantId} | HasHost={HasHost} | Port={Port} | HasUsername={HasUsername} | HasSecret={HasSecret} | HasFromName={HasFromName} | HasFromEmail={HasFromEmail} | HasRecipient={HasRecipient}",
                        tenantId,
                        smtpHost is not null,
                        smtpPort,
                        smtpUsername is not null,
                        smtpSecret is not null,
                        fromName is not null,
                        fromEmail is not null,
                        recipientEmail is not null);
                    return false;
                }

                var tenantConfigInfo = configDb.Tenant;

                // 3️⃣ Prepare placeholders
                var finalPlaceholders = new Dictionary<string, string>
                {
                    ["TenantName"] = tenantConfigInfo.CompanyName ?? "",
                    ["TenantLogoUrl"] = tenantConfigInfo.TenantProfile
                                            .Select(x => x.LogoUrl)
                                            .FirstOrDefault()
                                            ?? "https://cdn.axionpro.com/default-logo.png",
                    ["SupportEmail"] = tenantConfigInfo.TenantEmail ?? "",
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
                    fromName,
                    fromEmail
                ));

                message.To.Add(MailboxAddress.Parse(recipientEmail));

                message.Subject = subject;

                message.Body = new BodyBuilder
                {
                    HtmlBody = body
                }.ToMessageBody();

                using var smtp = new SmtpClient();

                smtp.Timeout = 20000;

                await smtp.ConnectAsync(
                    smtpHost,
                    smtpPort,
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    smtpUsername,
                    smtpSecret);

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

        private static string? CleanDatabaseValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var cleanedValue = value.Trim().Trim('\u200B', '\uFEFF');
            return string.IsNullOrWhiteSpace(cleanedValue) ? null : cleanedValue;
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
