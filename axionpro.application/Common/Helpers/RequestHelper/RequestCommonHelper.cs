using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Interfaces.IEncryptionService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers.RequestHelper
{
    public static class RequestCommonHelper
    {
        public static string? ExtractBearerToken(HttpContext? httpContext)
        {
            return httpContext?
                .Request?
                .Headers["Authorization"]
                .ToString()?
                .Replace("Bearer ", "");
        }

        public static TokenClaimsModel? ValidateAndExtractClaims(string token, IConfiguration config)
        {
            var secretKey = TokenKeyHelper.GetJwtSecret(config);
            var rawClaims = TokenClaimHelper.ExtractClaims(token, secretKey);

            if (rawClaims == null || rawClaims.IsExpired)
                return null;

            return new TokenClaimsModel
            {
                UserId = rawClaims.UserId,
                TenantId = rawClaims.TenantId,
                TenantEncriptionKey =  EncryptionSanitizer.SuperSanitize(rawClaims.TenantEncriptionKey),
                RoleId = rawClaims.RoleId,
                Expiry = rawClaims.Expiry,
                IsExpired = rawClaims.IsExpired,
                TokenPurpose = rawClaims.TokenPurpose,
                EmployeeId = rawClaims.EmployeeId,
                Email = rawClaims.Email



            };
        }

        public static (long UserEmpId, long TenantId) DecodeUserAndTenant(
            string encodedUserId,
            string encTenantId,
            string tenantKey,
            IIdEncoderService encoder)
        {
            string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);

            long userId = encoder.DecodeId_long(    EncryptionSanitizer.CleanEncodedInput(encodedUserId),   finalKey );

            long tenantId = encoder.DecodeId_long(encTenantId, finalKey);

            return (userId, tenantId);
        }

        public static long DecodeUserAndTenantIds(  string encTenantId,
             string tenantKey,
            IIdEncoderService encoder)
        {
            string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);

            long tenantId = encoder.DecodeId_long(encTenantId, finalKey);

            return tenantId;
        }
    
        public static long  DecodeOnlyEmployeeId(
           string encodedEmpId,         
           string tenantKey,
           IIdEncoderService encoder)
        {
           

                long userId = encoder.DecodeId_long(EncryptionSanitizer.CleanEncodedInput(encodedEmpId),
                tenantKey
            );

           

            return (userId);
        }

        public static string? Clean(string? value)
        {
            return EncryptionSanitizer.CleanEncodedInput(value);
        }
    }

}
