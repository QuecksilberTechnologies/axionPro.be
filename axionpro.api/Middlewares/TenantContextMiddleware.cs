using axionpro.application.Common.Contexts;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace axionpro.api.Middlewares
{
    public class TenantContextMiddleware
    {
        private const string TenantContextKey = "TenantContext";
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantContextMiddleware> _logger;

        public TenantContextMiddleware(
            RequestDelegate next,
            ILogger<TenantContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenService tokenService,
            ITenantKeyResolver tenantKeyResolver,
            IIdEncoderService encoderService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            try
            {
                var token = authHeader.Replace("Bearer ", "").Trim();

                var claims = tokenService.ValidateAndExtractClaims(token);
                if (claims == null || claims.IsExpired)
                {
                    await _next(context);
                    return;
                }

                var encodedTenantId = claims.TenantId;
                if (string.IsNullOrWhiteSpace(encodedTenantId))
                {
                    _logger.LogWarning("TenantId missing in token.");
                    await _next(context);
                    return;
                }

                // ✅ GLOBAL HashId decode (NO tenant key)
                var tenantId = encoderService.DecodeId_long(encodedTenantId, null);

                if (tenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId after decoding.");
                    await _next(context);
                    return;
                }

                // ✅ Resolve tenant encryption key (RAM → DB)
                var encryptionKey = await tenantKeyResolver.ResolveAsync(tenantId);

                context.Items[TenantContextKey] = new TenantContext
                {
                    TenantId = tenantId,
                    TenantEncryptionKey = encryptionKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TenantContextMiddleware error.");
            }

            await _next(context);
        }
    }
}
