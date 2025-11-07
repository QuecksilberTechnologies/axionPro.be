using axionpro.application.Interfaces.IEncryptionService;

namespace axionpro.application.Common.Helpers.Converters
{
    public class IdEncryptionConverter
    {
        private readonly IEncryptionService _encryptionService;

        public IdEncryptionConverter(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public string Convert(long id, string tenantKey)
        {
            if (id == 0) return string.Empty;
            return _encryptionService.Encrypt(id.ToString(), tenantKey);
        }

        public string? ConvertNullable(long? id, string tenantKey)
        {
            if (id == null || id == 0) return string.Empty;
            return _encryptionService.Encrypt(id.Value.ToString(), tenantKey);
        }

        public long? Decrypt(string? encryptedId, string tenantKey)
        {
            if (string.IsNullOrEmpty(encryptedId)) return null;
            var decrypted = _encryptionService.Decrypt(encryptedId, tenantKey);
            return long.TryParse(decrypted, out var val) ? val : (long?)null;
        }
    }
}
