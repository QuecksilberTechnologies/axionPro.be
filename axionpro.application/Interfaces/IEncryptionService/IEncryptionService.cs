using System;

namespace axionpro.application.Interfaces.IEncryptionService
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypt plain text using tenantKey.
        /// Returns Base64 string (IV + Cipher)
        /// </summary>
        string Encrypt(string plainText, string tenantKey);

        /// <summary>
        /// Decrypt encrypted text using tenantKey.
        /// Input is sanitized automatically to handle spaces, newlines, Base64Url, etc.
        /// </summary>
        string Decrypt(string cipherText, string tenantKey);

        /// <summary>
        /// Generates a random 32-byte Base64 key for a tenant.
        /// </summary>
        string GenerateKey();
    }
}
