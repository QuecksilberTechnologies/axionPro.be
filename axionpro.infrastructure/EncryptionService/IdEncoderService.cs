using axionpro.application.Interfaces.IEncryptionService;
using HashidsNet;
using System;
using System.Text;

namespace axionpro.infrastructure.EncryptionService
{
    public class IdEncoderService : IIdEncoderService
    {
        private const string GlobalSalt = "AxionPro_Global_Static_Salt";

        // 🔹 Common salt generator
        private string GetSalt(string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(tenantKey))
                tenantKey = "default";
            return $"{GlobalSalt}_{tenantKey.Trim()}";
        }

        // ✅ Encode numeric ID
        public string EncodeId(long id, string tenantKey)
        {
            if (id <= 0)
                return string.Empty;

            var salt = GetSalt(tenantKey);
            var hashids = new Hashids(salt, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            return hashids.EncodeLong(id);
        }

        // ✅ Decode numeric ID
        public long DecodeId(string encodedId, string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(encodedId))
                return 0;

            var salt = GetSalt(tenantKey);
            var hashids = new Hashids(salt, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");

            var decoded = hashids.DecodeLong(encodedId);
            return decoded.Length > 0 ? decoded[0] : 0;
        }

        // ✅ Encode string → string (convert string to bytes → numeric → encode)
        public string EncodeString(string input, string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(input);
            long numericValue = BitConverter.ToInt64(PadBytes(bytes), 0);

            var salt = GetSalt(tenantKey);
            var hashids = new Hashids(salt, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            return hashids.EncodeLong(numericValue);
        }

        // ✅ Decode string → original text
        public string DecodeString(string encoded, string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(encoded))
                return string.Empty;

            var salt = GetSalt(tenantKey);
            var hashids = new Hashids(salt, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");

            var decoded = hashids.DecodeLong(encoded);
            if (decoded.Length == 0)
                return string.Empty;

            var bytes = BitConverter.GetBytes(decoded[0]);
            return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }

        // Helper to make sure byte array has at least 8 bytes
        private byte[] PadBytes(byte[] bytes)
        {
            if (bytes.Length >= 8)
                return bytes;

            var padded = new byte[8];
            Array.Copy(bytes, padded, bytes.Length);
            return padded;
        }
    }
}
