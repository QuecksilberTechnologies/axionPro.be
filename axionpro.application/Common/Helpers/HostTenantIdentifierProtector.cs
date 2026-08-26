// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Converts Host-facing encrypted Tenant identifiers to and from repository-safe numeric identifiers.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IEncryptionService;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Protects Tenant identifiers at the Host API boundary with authenticated encryption material derived from the validated Host JWT.
/// </summary>
public static class HostTenantIdentifierProtector
{
    /// <summary>
    /// Encrypts a persisted Tenant identifier for a Host-facing response.
    /// </summary>
    /// <param name="tenantId">The numeric Tenant identifier from persistence.</param>
    /// <param name="tenantEncryptionKey">The Host-scoped signed token key.</param>
    /// <param name="encryptionService">The established encryption service.</param>
    /// <returns>The protected Tenant identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a persisted identifier cannot be protected.</exception>
    public static string Encrypt(
        long tenantId,
        string tenantEncryptionKey,
        IEncryptionService encryptionService)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(tenantEncryptionKey))
        {
            throw new InvalidOperationException("A Host-facing Tenant identifier could not be protected.");
        }

        var encryptedTenantId = encryptionService.Encrypt(tenantId.ToString(), tenantEncryptionKey);
        if (string.IsNullOrWhiteSpace(encryptedTenantId))
        {
            throw new InvalidOperationException("A Host-facing Tenant identifier could not be protected.");
        }

        var routeSafeCiphertext = ToBase64Url(encryptedTenantId);
        return $"{routeSafeCiphertext}.{CreateIntegrityTag(routeSafeCiphertext, tenantEncryptionKey)}";
    }

    /// <summary>
    /// Decrypts and validates a Host-facing Tenant identifier before repository access.
    /// </summary>
    /// <param name="encryptedTenantId">The encrypted identifier submitted by the client.</param>
    /// <param name="tenantEncryptionKey">The Host-scoped signed token key.</param>
    /// <param name="encryptionService">The established encryption service.</param>
    /// <returns>The numeric Tenant identifier for repository use.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the submitted identifier is malformed or raw.</exception>
    public static long Decrypt(
        string? encryptedTenantId,
        string tenantEncryptionKey,
        IEncryptionService encryptionService)
    {
        if (string.IsNullOrWhiteSpace(encryptedTenantId) ||
            string.IsNullOrWhiteSpace(tenantEncryptionKey) ||
            long.TryParse(encryptedTenantId, out _))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        try
        {
            var protectedParts = encryptedTenantId.Split('.', StringSplitOptions.None);
            if (protectedParts.Length != 2 ||
                !IsBase64Url(protectedParts[0]) ||
                !IsBase64Url(protectedParts[1]) ||
                !HasValidIntegrityTag(protectedParts[0], protectedParts[1], tenantEncryptionKey))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            var decryptedTenantId = encryptionService.Decrypt(
                FromBase64Url(protectedParts[0]),
                tenantEncryptionKey);
            if (!long.TryParse(decryptedTenantId, out var tenantId) || tenantId <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            return tenantId;
        }
        catch (ValidationErrorException)
        {
            throw;
        }
        catch
        {
            // Ciphertext details are intentionally not exposed at the API boundary.
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }
    }

    /// <summary>
    /// Converts standard Base64 ciphertext into a route-safe Base64URL segment.
    /// </summary>
    private static string ToBase64Url(string value) => value
        .Replace('+', '-')
        .Replace('/', '_')
        .TrimEnd('=');

    /// <summary>
    /// Converts a validated Base64URL segment back to the standard Base64 form accepted by the existing AES service.
    /// </summary>
    private static string FromBase64Url(string value)
    {
        var standardBase64 = value.Replace('-', '+').Replace('_', '/');
        return standardBase64.PadRight(standardBase64.Length + ((4 - standardBase64.Length % 4) % 4), '=');
    }

    /// <summary>
    /// Determines whether a value is a non-empty Base64URL segment.
    /// </summary>
    private static bool IsBase64Url(string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        Regex.IsMatch(value, "^[A-Za-z0-9_-]+$");

    /// <summary>
    /// Creates the route-safe HMAC tag that prevents ciphertext manipulation before decryption.
    /// </summary>
    private static string CreateIntegrityTag(string routeSafeCiphertext, string tenantEncryptionKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(tenantEncryptionKey));
        return ToBase64Url(Convert.ToBase64String(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(routeSafeCiphertext))));
    }

    /// <summary>
    /// Validates the identifier integrity tag in constant time.
    /// </summary>
    private static bool HasValidIntegrityTag(
        string routeSafeCiphertext,
        string suppliedTag,
        string tenantEncryptionKey)
    {
        var expectedTag = CreateIntegrityTag(routeSafeCiphertext, tenantEncryptionKey);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedTag),
            Encoding.UTF8.GetBytes(suppliedTag));
    }
}
