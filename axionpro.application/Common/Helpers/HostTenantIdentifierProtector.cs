// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Converts Host-facing encoded Tenant identifiers to and from repository-safe numeric identifiers.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IEncryptionService;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Converts Tenant identifiers at the Host API boundary through the established global ID encoder.
/// </summary>
public static class HostTenantIdentifierProtector
{
    /// <summary>
    /// Encodes a persisted Tenant identifier for a Host-facing response.
    /// </summary>
    /// <param name="tenantId">The numeric Tenant identifier from persistence.</param>
    /// <param name="tenantEncryptionKey">The trusted Host token key, sanitized before encoder use.</param>
    /// <param name="idEncoderService">The established application ID encoder.</param>
    /// <returns>The encoded Tenant identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a persisted identifier cannot be protected.</exception>
    public static string Encrypt(
        long tenantId,
        string tenantEncryptionKey,
        IIdEncoderService idEncoderService)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(tenantEncryptionKey))
        {
            throw new InvalidOperationException("A Host-facing Tenant identifier could not be protected.");
        }

        var trustedTenantKey = EncryptionSanitizer.SuperSanitize(tenantEncryptionKey);
        var encodedTenantId = idEncoderService.EncodeId_long(tenantId, trustedTenantKey);
        if (string.IsNullOrWhiteSpace(encodedTenantId))
        {
            throw new InvalidOperationException("A Host-facing Tenant identifier could not be protected.");
        }

        return encodedTenantId;
    }

    /// <summary>
    /// Decodes and validates a Host-facing Tenant identifier before repository access.
    /// </summary>
    /// <param name="encryptedTenantId">The encrypted identifier submitted by the client.</param>
    /// <param name="tenantEncryptionKey">The trusted Host token key, sanitized before encoder use.</param>
    /// <param name="idEncoderService">The established application ID encoder.</param>
    /// <returns>The numeric Tenant identifier for repository use.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the submitted identifier is malformed or raw.</exception>
    public static long Decrypt(
        string? encryptedTenantId,
        string tenantEncryptionKey,
        IIdEncoderService idEncoderService)
    {
        if (string.IsNullOrWhiteSpace(encryptedTenantId) ||
            string.IsNullOrWhiteSpace(tenantEncryptionKey) ||
            long.TryParse(encryptedTenantId, out _))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var trustedTenantKey = EncryptionSanitizer.SuperSanitize(tenantEncryptionKey);
        var tenantId = idEncoderService.DecodeId_long(
            EncryptionSanitizer.CleanEncodedInput(encryptedTenantId),
            trustedTenantKey);
        if (tenantId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        return tenantId;
    }
}
