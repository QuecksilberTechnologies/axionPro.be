using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace axionpro.infrastructure.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IUnitOfWork _UOW)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _UOW = _UOW ?? throw new ArgumentNullException(nameof(_UOW));
        }

        // ✅ 1️⃣ Generate Token with Custom Claims
        public async Task<string> GenerateToken(GetTokenInfoDTO user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User object cannot be null.");

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["JWTSettings:Secret"];

                if (string.IsNullOrEmpty(jwtKey))
                    throw new ArgumentNullException("JWTSettings:Secret", "JWT key cannot be null or empty.");

                var key = Encoding.UTF8.GetBytes(jwtKey);
                if (key.Length < 32)
                    throw new ArgumentException("JWT key must be at least 32 bytes long for HMAC SHA-256.");

                var issuer = _configuration["JWTSettings:Issuer"];
                var audience = _configuration["JWTSettings:Audience"];
                var tokenLifetime = TimeSpan.Parse(_configuration["JWTSettings:TokenLifetime"]);

                // 🧩 Add all custom claims safely
                var claims = new List<Claim>();
                try
                {
                    claims.Add(new Claim("UserId", user.UserId?.ToString() ?? string.Empty));
                    claims.Add(new Claim("EmployeeId", user.EmployeeId?.ToString() ?? string.Empty));
                    claims.Add(new Claim("RoleId", user.RoleId.ToString() ?? string.Empty));
                    claims.Add(new Claim("RoleTypeId", user.RoleTypeId.ToString() ?? string.Empty));
                    claims.Add(new Claim("EmployeeTypeId", user.EmployeeTypeId.ToString() ?? string.Empty));
                    claims.Add(new Claim("TenantId", user.TenantId?.ToString() ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));
                    claims.Add(new Claim("FullName", user.FullName ?? string.Empty));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim("TenantEncriptionKey", user.TenantEncriptionKey ?? string.Empty));
                    claims.Add(new Claim("GenderId", user.GenderId.ToString() ?? string.Empty));
                    claims.Add(new Claim("GenderName", user.GenderName ?? string.Empty));
                    claims.Add(new Claim("EmployeeTypeName", user.EmployeeTypeName ?? string.Empty));
                    claims.Add(new Claim("RoleTypeName", user.RoleTypeName ?? string.Empty));
                    claims.Add(new Claim("HasPermanent", user.HasPermanent.ToString()));
                    claims.Add(new Claim("TokenPurpose", user.TokenPurpose.ToString()));
                    claims.Add(new Claim("IssuedAt", user.HasPermanent.ToString()));
                    claims.Add(new Claim("ClientType", user.HasPermanent.ToString()));


                }
                catch (Exception innerEx)
                {
                    _logger.LogWarning(innerEx, "Error creating claims for user {UserId}", user.UserId);
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(tokenLifetime),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                _logger.LogInformation("Token generated successfully for UserId: {UserId}", user.UserId);

                return await Task.FromResult(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Token generation failed for User: {UserId}", user?.UserId);
                return null;
            }
        }

        // ✅ 2️⃣ Validate Token
        public bool ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed.");
                return false;
            }
        }

        // ✅ 3️⃣ Extract Claims from Token
        public async Task<string> GetUserInfoFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return JsonConvert.SerializeObject(new { Error = "Token is missing.", IsExpired = true });

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]);

            try
            {
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userInfo = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
                userInfo["Expiry"] = (validatedToken as JwtSecurityToken)?.ValidTo.ToString("o") ?? "";
                userInfo["IsExpired"] = ((validatedToken as JwtSecurityToken)?.ValidTo ?? DateTime.UtcNow) < DateTime.UtcNow ? "true" : "false";

                return JsonConvert.SerializeObject(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid or expired token detected.");
                return JsonConvert.SerializeObject(new { Error = "Invalid or tampered token.", IsExpired = true });
            }
        }

        // ✅ 4️⃣ Extract Expiry from Token
        public DateTime? GetExpiryFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken?.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not extract expiry from token.");
                return null;
            }
        }

        // ✅ 5️⃣ Generate Refresh Token
        public async Task<string> GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token.");
                return string.Empty;
            }
        }
        public TokenClaimsModel? ValidateAndExtractClaims(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]);

                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JWTSettings:Issuer"],
                    ValidAudience = _configuration["JWTSettings:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;

                return new TokenClaimsModel
                {
                    UserId = principal.FindFirst("UserId")?.Value,
                    TenantId = principal.FindFirst("TenantId")?.Value,
                    EmployeeId = principal.FindFirst("EmployeeId")?.Value,
                    RoleId = principal.FindFirst("RoleId")?.Value,
                    Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                    Expiry = jwt.ValidTo,
                    IsExpired = jwt.ValidTo < DateTime.UtcNow,
                    TokenPurpose = principal.FindFirst("TokenPurpose")?.Value
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<GetTokenInfoDTO?> GetUserInfoByLoginIdAsync(string loginId)
        {
            try
            {
                // ✅ Step 1: Validate active login and get EmployeeId
                var empId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(loginId);
                if (empId < 1)
                {
                    _logger.LogWarning($"No active user found for LoginId: {loginId}");
                    return null;
                }

                // ✅ Step 2: Get Employee details
                var userMin = await _unitOfWork.Employees.GetSingleRecordAsync(empId, true);
                if (userMin == null)
                {
                    _logger.LogWarning($"Employee record not found for EmployeeId: {empId}");
                    return null;
                }

                // ✅ Step 3: Get Tenant encryption key
                var tenantKey = await _unitOfWork.TenantEncryptionKeyRepository.GetActiveKeyByTenantIdAsync(userMin.TenantId);
                if (tenantKey == null)
                {
                    _logger.LogWarning($"Tenant encryption key not found for TenantId: {userMin.TenantId}");
                    return null;
                }

                // ✅ Step 4: Get user roles
                var roles = await _unitOfWork.UserRoleRepository.GetEmployeeRolesWithDetailsByIdAsync(empId, userMin.TenantId);
                var roleInfo = roles?.FirstOrDefault(r => r.IsPrimaryRole == true);
                if (roleInfo == null)
                {
                    _logger.LogWarning($"Primary role not found for EmployeeId: {empId}");
                    return null;
                }

                // ✅ Step 5: Build and return token info DTO
                var tokenInfo = new GetTokenInfoDTO
                {
                    TenantEncriptionKey = tenantKey.EncryptionKey,
                    TenantId = userMin.TenantId.ToString(),
                    UserId = loginId,
                    EmployeeId = userMin.Id.ToString(),
                    RoleId = roleInfo.RoleId.ToString(),
                    RoleTypeId = roleInfo.Role.RoleType.ToString(),
                    RoleTypeName = roleInfo.Role.RoleName,
                    Email = userMin.OfficialEmail ?? string.Empty,
                    FullName = $"{userMin.FirstName} {userMin.LastName}",
                    Expiry = DateTime.UtcNow.AddMinutes(15)
                };

                return tokenInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user info for LoginId: {loginId}");
                return null;
            }
        }
    }
}
   

   
//claims.Add(new Claim("UserId", user.UserId ?? ""));
//claims.Add(new Claim("EmployeeId", user.EmployeeId ?? ""));
//claims.Add(new Claim("RoleId", user.RoleId ?? ""));
//claims.Add(new Claim("RoleTypeId", user.RoleTypeId ?? ""));
//claims.Add(new Claim("EmployeeTypeId", user.EmployeeTypeId ?? ""));
//claims.Add(new Claim("TenantId", user.TenantId ?? ""));
//claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
//claims.Add(new Claim("FullName", user.FullName ?? ""));
//claims.Add(new Claim("TokenPurpose", user.TokenPurpose ?? "AUTH"));

//claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
//claims.Add(new Claim(JwtRegisteredClaimNames.Iat,
//    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
//    ClaimValueTypes.Integer64));


