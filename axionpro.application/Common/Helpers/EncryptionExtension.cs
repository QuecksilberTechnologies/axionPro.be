
using axionpro.application.Interfaces.IEncryptionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers
{
    public static class EncryptionHelper1
    {
        /// <summary>
        /// Encrypts a numeric ID using tenant-specific key.
        /// </summary>
        public static string EncryptId(IEncryptionService encryptionService, long id, string tenantKey)
        {
            if (id <= 0)
                return string.Empty;

            return encryptionService.Encrypt(id.ToString(), tenantKey);
        }

        /// <summary>
        /// Decrypts an encrypted ID back to long.
        /// </summary>
        public static long DecryptId(IEncryptionService encryptionService, string encryptedId, string tenantKey)
        {
            if (string.IsNullOrEmpty(encryptedId))
                return 0;

            try
            {
                string decryptedValue = encryptionService.Decrypt(encryptedId, tenantKey);
                return long.TryParse(decryptedValue, out long id) ? id : 0;
            }
            catch
            {
                // optional: log error here if needed
                return 0; // fail-safe return
            }
        }
    }


}
