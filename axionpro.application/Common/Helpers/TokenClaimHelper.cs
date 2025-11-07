using axionpro.application.DTOS.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace axionpro.application.Common.Helpers
{
    public static class TokenClaimHelper
    {
        public static GetTokenInfoDTO? ExtractClaims(string token, string? secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // ✅ Automatically detect whether key is Base64 or plain text
                byte[] key;
                try
                {
                    key = Convert.FromBase64String(secretKey);
                }
                catch
                {
                    key = Encoding.UTF8.GetBytes(secretKey);
                }

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var dto = new GetTokenInfoDTO
                {
                    UserId = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? string.Empty,
                    EmployeeId = principal.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value ?? "0",
                    RoleId = Convert.ToInt32(principal.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value ?? "0"),
                    RoleTypeId = Convert.ToInt32(principal.Claims.FirstOrDefault(c => c.Type == "RoleTypeId")?.Value ?? "0"),
                    EmployeeTypeId = Convert.ToInt32(principal.Claims.FirstOrDefault(c => c.Type == "EmployeeTypeId")?.Value ?? "0"),
                    TenantId = principal.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value ?? "0",
                    Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                    TenantEncriptionKey = principal.Claims.FirstOrDefault(c => c.Type == "TenantEncriptionKey")?.Value ?? string.Empty,
                    FullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? string.Empty,
                    Expiry = jwtToken.ValidTo,
                    IsExpired = jwtToken.ValidTo < DateTime.UtcNow
                };

                return dto;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
