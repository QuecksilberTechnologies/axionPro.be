namespace axionpro.application.Interfaces.IEmail;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(
        string toEmail,
        string subject,
        string body,
        long? TenantId,
        string otp);

    /// <summary>
    /// Uses the Tenant SMTP configuration first and falls back to the active
    /// Host default SMTP configuration when the Tenant configuration is absent
    /// or incomplete.
    /// </summary>
    Task<bool> SendTemplatedEmailAsync(
        string templateCode,
        string toEmail,
        long? TenantId,
        Dictionary<string, string> placeholders);

    /// <summary>
    /// Sends through the active Host default SMTP configuration. Used by
    /// self-registration before the Tenant SMTP configuration is used.
    /// </summary>
    Task<bool> SendTemplatedEmailUsingHostConfigAsync(
        string templateCode,
        string toEmail,
        long? TenantId,
        Dictionary<string, string> placeholders);
}
