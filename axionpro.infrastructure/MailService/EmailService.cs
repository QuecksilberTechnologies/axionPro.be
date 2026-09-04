using axionpro.application.Common.Helpers;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Sockets;

namespace axionpro.infrastructure.MailService;

/// <summary>
/// Sends rendered email templates through a Tenant SMTP configuration. When a
/// Tenant configuration is unavailable or invalid, the active Host default
/// configuration is used as a safe delivery fallback.
/// </summary>
public sealed class EmailService : IEmailService
{
    private const string FallbackLogoUrl = "https://cdn.axionpro.com/default-logo.png";

    private readonly ITenantEmailConfigRepository _tenantEmailConfigRepository;
    private readonly IDefaultEmailConfigRepository _defaultEmailConfigRepository;
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        ITenantEmailConfigRepository tenantEmailConfigRepository,
        IDefaultEmailConfigRepository defaultEmailConfigRepository,
        IEmailTemplateRepository templateRepository,
        ILogger<EmailService> logger)
    {
        _tenantEmailConfigRepository = tenantEmailConfigRepository;
        _defaultEmailConfigRepository = defaultEmailConfigRepository;
        _templateRepository = templateRepository;
        _logger = logger;
    }

    public Task<bool> SendOtpEmailAsync(
        string toEmail,
        string subject,
        string body,
        long? tenantId,
        string otp) => throw new NotImplementedException();

    /// <summary>
    /// Sends using the active Tenant configuration, falling back to the active
    /// Host default configuration when Tenant SMTP is not usable.
    /// </summary>
    public Task<bool> SendTemplatedEmailAsync(
        string templateCode,
        string toEmail,
        long? tenantId,
        Dictionary<string, string> placeholders) =>
        SendTemplatedEmailInternalAsync(
            templateCode,
            toEmail,
            tenantId,
            placeholders,
            preferHostEmailConfiguration: false);

    /// <summary>
    /// Sends with the active Host default configuration. This is used during
    /// self-registration, before a newly created Tenant's SMTP details should
    /// be trusted for delivery.
    /// </summary>
    public Task<bool> SendTemplatedEmailUsingHostConfigAsync(
        string templateCode,
        string toEmail,
        long? tenantId,
        Dictionary<string, string> placeholders) =>
        SendTemplatedEmailInternalAsync(
            templateCode,
            toEmail,
            tenantId,
            placeholders,
            preferHostEmailConfiguration: true);

    private async Task<bool> SendTemplatedEmailInternalAsync(
        string templateCode,
        string toEmail,
        long? tenantId,
        IReadOnlyDictionary<string, string> placeholders,
        bool preferHostEmailConfiguration)
    {
        try
        {
            var template = await _templateRepository.GetTemplateByCodeAsync(templateCode);
            if (template is null || !template.IsActive)
            {
                _logger.LogWarning("Email template missing or inactive | Code={TemplateCode}", templateCode);
                return false;
            }

            var emailConfiguration = await ResolveSmtpConfigurationAsync(
                tenantId,
                preferHostEmailConfiguration);
            var recipientEmail = CleanDatabaseValue(toEmail);

            if (emailConfiguration is null || recipientEmail is null)
            {
                _logger.LogWarning(
                    "Email delivery configuration is unavailable or recipient is invalid | Template={TemplateCode} | TenantId={TenantId} | HasRecipient={HasRecipient}",
                    templateCode,
                    tenantId,
                    recipientEmail is not null);
                return false;
            }

            var finalPlaceholders = BuildPlaceholders(emailConfiguration, placeholders);
            var subject = EmailTemplateRenderer.RenderBody(template.Subject ?? string.Empty, finalPlaceholders);
            var body = EmailTemplateRenderer.RenderBody(template.Body ?? string.Empty, finalPlaceholders);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailConfiguration.FromName, emailConfiguration.FromEmail));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            using var smtp = new SmtpClient { Timeout = 20_000 };
            await smtp.ConnectAsync(
                emailConfiguration.SmtpHost,
                emailConfiguration.SmtpPort,
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailConfiguration.SmtpUsername, emailConfiguration.SmtpSecret);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation(
                "SMTP accepted email | Template={TemplateCode} | TenantId={TenantId} | ConfigurationSource={ConfigurationSource} | To={To}",
                templateCode,
                tenantId,
                emailConfiguration.Source,
                recipientEmail);

            return true;
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(
                ex,
                "SMTP rejected email | Status={Status} | Template={TemplateCode} | TenantId={TenantId} | To={To}",
                ex.StatusCode,
                templateCode,
                tenantId,
                toEmail);
            return false;
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(
                ex,
                "SMTP protocol failure | Template={TemplateCode} | TenantId={TenantId} | To={To}",
                templateCode,
                tenantId,
                toEmail);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Email delivery failed | Template={TemplateCode} | TenantId={TenantId} | To={To}",
                templateCode,
                tenantId,
                toEmail);
            return false;
        }
    }

    private async Task<ResolvedSmtpConfiguration?> ResolveSmtpConfigurationAsync(
        long? tenantId,
        bool preferHostEmailConfiguration)
    {
        TenantEmailConfig? tenantConfiguration = null;
        if (tenantId is > 0)
        {
            tenantConfiguration = await _tenantEmailConfigRepository.GetActiveEmailConfigAsync(tenantId);
        }

        var tenantContext = TenantEmailTemplateContext.From(tenantConfiguration);

        if (!preferHostEmailConfiguration)
        {
            var resolvedTenantConfiguration = CreateSmtpConfiguration(
                tenantConfiguration?.SmtpHost,
                tenantConfiguration?.SmtpPort,
                tenantConfiguration?.SmtpUsername,
                tenantConfiguration?.SecrateKey,
                tenantConfiguration?.SmtpPasswordEncrypted,
                tenantConfiguration?.FromName,
                tenantConfiguration?.FromEmail,
                tenantContext,
                "Tenant");

            if (resolvedTenantConfiguration is not null)
            {
                return resolvedTenantConfiguration;
            }

            _logger.LogWarning(
                "Tenant SMTP configuration is missing or incomplete; falling back to the Host default configuration | TenantId={TenantId}",
                tenantId);
        }

        var hostConfiguration = await GetActiveHostEmailConfigurationAsync();
        var resolvedHostConfiguration = CreateSmtpConfiguration(
            hostConfiguration?.SmtpHost,
            hostConfiguration?.SmtpPort,
            hostConfiguration?.SmtpUsername,
            hostConfiguration?.SecrateKey,
            hostConfiguration?.SmtpPasswordEncrypted,
            hostConfiguration?.FromName,
            hostConfiguration?.FromEmail,
            tenantContext,
            "HostDefault");

        if (resolvedHostConfiguration is null)
        {
            _logger.LogError(
                "Active default Host SMTP configuration is missing or incomplete | TenantId={TenantId}",
                tenantId);
        }

        return resolvedHostConfiguration;
    }

    /// <summary>
    /// Retrieves only the Host configuration selected by both IsActive and
    /// IsDefault. Keeping this in one helper makes the fallback reusable by
    /// every mail flow without allowing an arbitrary Host record to be used.
    /// </summary>
    private Task<DefaultEmailConfig?> GetActiveHostEmailConfigurationAsync() =>
        _defaultEmailConfigRepository.GetActiveDefaultEmailConfigAsync();

    private static ResolvedSmtpConfiguration? CreateSmtpConfiguration(
        string? smtpHost,
        int? smtpPort,
        string? smtpUsername,
        string? smtpSecret,
        string? encryptedSmtpPassword,
        string? fromName,
        string? fromEmail,
        TenantEmailTemplateContext tenantContext,
        string source)
    {
        var cleanedHost = CleanDatabaseValue(smtpHost);
        var cleanedUsername = CleanDatabaseValue(smtpUsername);
        var cleanedSecret = CleanDatabaseValue(smtpSecret) ?? CleanDatabaseValue(encryptedSmtpPassword);
        var cleanedFromName = CleanDatabaseValue(fromName);
        var cleanedFromEmail = CleanDatabaseValue(fromEmail);
        var port = smtpPort.GetValueOrDefault();

        if (cleanedHost is null || cleanedUsername is null || cleanedSecret is null ||
            cleanedFromName is null || cleanedFromEmail is null || port is < 1 or > 65535)
        {
            return null;
        }

        return new ResolvedSmtpConfiguration(
            cleanedHost,
            port,
            cleanedUsername,
            cleanedSecret,
            cleanedFromName,
            cleanedFromEmail,
            tenantContext,
            source);
    }

    private static Dictionary<string, string> BuildPlaceholders(
        ResolvedSmtpConfiguration configuration,
        IReadOnlyDictionary<string, string> suppliedPlaceholders)
    {
        var placeholders = new Dictionary<string, string>
        {
            ["TenantName"] = configuration.TenantContext.TenantName,
            ["TenantLogoUrl"] = configuration.TenantContext.TenantLogoUrl,
            ["SupportEmail"] = string.IsNullOrWhiteSpace(configuration.TenantContext.SupportEmail)
                ? configuration.FromEmail
                : configuration.TenantContext.SupportEmail,
            ["Year"] = DateTime.UtcNow.Year.ToString()
        };

        foreach (var placeholder in suppliedPlaceholders)
        {
            placeholders[placeholder.Key] = placeholder.Value;
        }

        return placeholders;
    }

    private static string? CleanDatabaseValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var cleanedValue = value.Trim().Trim('\u200B', '\uFEFF');
        return string.IsNullOrWhiteSpace(cleanedValue) ? null : cleanedValue;
    }

    public static async Task CheckSmtpPorts()
    {
        const string host = "smtp-relay.brevo.com";
        int[] ports = [25, 465, 587, 2525];

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

    private sealed record ResolvedSmtpConfiguration(
        string SmtpHost,
        int SmtpPort,
        string SmtpUsername,
        string SmtpSecret,
        string FromName,
        string FromEmail,
        TenantEmailTemplateContext TenantContext,
        string Source);

    private sealed record TenantEmailTemplateContext(
        string TenantName,
        string TenantLogoUrl,
        string SupportEmail)
    {
        public static TenantEmailTemplateContext From(TenantEmailConfig? emailConfiguration)
        {
            var tenant = emailConfiguration?.Tenant;
            return new TenantEmailTemplateContext(
                tenant?.CompanyName ?? string.Empty,
                tenant?.TenantProfile
                    .Select(profile => profile.LogoUrl)
                    .FirstOrDefault() ?? FallbackLogoUrl,
                tenant?.TenantEmail ?? string.Empty);
        }
    }
}
