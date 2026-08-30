// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides shared tenant and Host request validation.
// ================================================================

using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.infrastructure.CommonRequest
{
    public class CommonRequestService : ICommonRequestService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private readonly IIdEncoderService _encoder;
        private readonly ILogger<CommonRequestService> _logger;

        public CommonRequestService(
            IHttpContextAccessor ctx,
            IConfiguration cfg,
            IUnitOfWork uow,
            IIdEncoderService enc,
            ILogger<CommonRequestService> logger)
        {
            _context = ctx;
            _config = cfg;
            _uow = uow;
            _encoder = enc;
            _logger = logger;
        }

        #region Tenant Request Validation

        /// <summary>
        /// Validates the current authenticated tenant request and resolves the trusted tenant, employee, and role context.
        /// </summary>
        /// <returns>The validated tenant request context.</returns>
        public async Task<CommonDecodedResult> ValidateTenantUserRequestAsync()
        {
            try
            {
                var token = RequestCommonHelper.ExtractBearerToken(_context.HttpContext);
                if (string.IsNullOrEmpty(token))
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token missing." };

                var claims = RequestCommonHelper.ValidateAndExtractClaims(token, _config);
                if (claims == null)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token expired or invalid." };

                // Tenant access tokens now carry an explicit principal type. Continue to accept
                // an empty value only for sessions issued before that claim was introduced.
                if (!string.IsNullOrWhiteSpace(claims.UserType) &&
                    !string.Equals(
                        claims.UserType,
                        AppConstants.TenantUserType,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return new CommonDecodedResult { Success = false, ErrorMessage = AppConstants.ErrorMessages.Unauthorized };
                }

                long loggedInId = await _uow.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(claims.UserId);
                if (loggedInId < 1)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Inactive user." };

                var tenantId = RequestCommonHelper.DecodeUserAndTenantIds(claims.TenantId!,claims.TenantEncriptionKey!, _encoder );

                var decoded = (UserEmpId: loggedInId, TenantId: tenantId);

                if (decoded.UserEmpId != loggedInId)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "User mismatch." };

                if (decoded.TenantId <= 0 )
                {
                    _logger.LogWarning("❌ Tenant information missing .");
                    return new CommonDecodedResult { Success = false, ErrorMessage = "TenantId not correct" };
                }

                return new CommonDecodedResult
                {
                    Success = true,
                    LoggedInEmployeeId = loggedInId,
                    UserEmployeeId = decoded.UserEmpId,
                    TenantId = decoded.TenantId,
                    RoleId = SafeParser.TryParseInt(claims.RoleId),                   
                    Claims = claims
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CommonRequestService Error");
                return new CommonDecodedResult { Success = false, ErrorMessage = "Internal validation error." };
            }
        }

        #endregion

        #region Unified Authentication Validation

        /// <summary>
        /// Validates the current authenticated Host or Tenant request by reusing the established principal-specific validation path.
        /// </summary>
        /// <returns>The trusted authenticated principal context.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the JWT, token purpose, or represented principal is invalid.</exception>
        public async Task<AuthenticatedRequestContext> ValidateAuthenticatedRequestAsync()
        {
            var principal = _context.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            // Host tokens carry the explicit principal-type claim established by GenerateHostToken.
            var userType = principal.FindFirst(AppConstants.UserTypeClaim)?.Value;
            if (string.Equals(userType, AppConstants.HostUserType, StringComparison.OrdinalIgnoreCase))
            {
                var hostUserId = await ValidateHostUserRequestAsync();
                return new AuthenticatedRequestContext
                {
                    UserType = LoginUserType.Host,
                    AuthenticatedUserId = hostUserId
                };
            }

            // Tenant tokens carry their explicit principal type. Empty remains supported only for
            // access tokens issued before the Tenant UserType claim was added.
            if (!string.IsNullOrWhiteSpace(userType) &&
                !string.Equals(userType, AppConstants.TenantUserType, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            var tenantValidation = await ValidateTenantUserRequestAsync();
            if (!tenantValidation.Success ||
                tenantValidation.Claims == null ||
                !string.Equals(
                    tenantValidation.Claims.TokenPurpose,
                    ConstantValues.Auth.ToString(),
                    StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            return new AuthenticatedRequestContext
            {
                UserType = LoginUserType.TenantEmployee,
                AuthenticatedUserId = tenantValidation.LoggedInEmployeeId,
                TenantId = tenantValidation.TenantId,
                RoleId = tenantValidation.RoleId
            };
        }

        #endregion

        #region HostUser Token Validation

        /// <summary>
        /// Validates that the current authenticated JWT represents an active Host user with an active Host role.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current token, claims, Host user, or Host role are invalid.</exception>
        public async Task<long> ValidateHostUserRequestAsync()
        {
            var hostContext = await ValidateHostUserContextAsync(
                enforceTokenRoleMatch: true,
                requireTenantEncryptionKey: false);

            return hostContext.HostUserId;
        }

        /// <summary>
        /// Validates the authenticated Host token and requires the current active Host role to be the verified Super Admin role.
        /// </summary>
        /// <returns>The trusted Host JWT and current database context.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the principal is not a Host user or no longer holds the Super Admin role.</exception>
        public async Task<HostUserRequestContext> ValidateHostSuperAdminRequestAsync()
        {
            var hostContext = await ValidateHostUserContextAsync(
                enforceTokenRoleMatch: true,
                requireTenantEncryptionKey: false);

            if (!string.Equals(hostContext.UserType, AppConstants.HostUserType, StringComparison.Ordinal) ||
                hostContext.CurrentHostRoleId != AppConstants.SuperAdminHostRoleId)
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            return hostContext;
        }

        /// <summary>
        /// Validates a Host request for runtime permission enforcement without treating a changed database role as a valid session.
        /// </summary>
        /// <returns>The trusted Host context used by Host runtime permission and Tenant identifier protection logic.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current Host token or current Host principal is invalid.</exception>
        public Task<HostUserRequestContext> ValidateHostUserPermissionRequestAsync() =>
            ValidateHostUserContextAsync(
                enforceTokenRoleMatch: false,
                requireTenantEncryptionKey: true);

        /// <summary>
        /// Resolves a current Host principal from signed claims while preserving stale-role detection for the database permission function.
        /// </summary>
        /// <param name="enforceTokenRoleMatch">Whether callers require immediate role-snapshot equality.</param>
        /// <param name="requireTenantEncryptionKey">Whether the Host-facing Tenant identifier key claim is required.</param>
        /// <returns>The trusted Host context.</returns>
        private async Task<HostUserRequestContext> ValidateHostUserContextAsync(
            bool enforceTokenRoleMatch,
            bool requireTenantEncryptionKey)
        {
            const string unauthorizedMessage = "A valid Host user token is required.";

            var principal = _context.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var userType = principal.FindFirst(AppConstants.UserTypeClaim)?.Value;
            if (!string.Equals(userType, AppConstants.HostUserType, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var tokenPurpose = principal.FindFirst("TokenPurpose")?.Value;
            if (!string.Equals(tokenPurpose, AppConstants.AccessTokenPurpose, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var hostUserIdValue = principal.FindFirst(AppConstants.HostUserIdClaim)?.Value;
            var hostRoleIdValue = principal.FindFirst(AppConstants.HostRoleIdClaim)?.Value;
            var loginId = principal.FindFirst(AppConstants.LoginIdClaim)?.Value;
            var tenantEncryptionKey = principal.FindFirst("TenantEncriptionKey")?.Value;

            if (!long.TryParse(hostUserIdValue, out var hostUserId) ||
                !long.TryParse(hostRoleIdValue, out var hostRoleId) ||
                hostUserId <= 0 ||
                hostRoleId <= 0 ||
                string.IsNullOrWhiteSpace(loginId) ||
                (requireTenantEncryptionKey && string.IsNullOrWhiteSpace(tenantEncryptionKey)))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var hostUser = await _uow.HostUserRepository.GetByIdAsync(hostUserId);
            if (hostUser == null ||
                !hostUser.IsActive ||
                hostUser.IsSoftDeleted ||
                (enforceTokenRoleMatch && hostUser.HostRoleId != hostRoleId) ||
                !string.Equals(hostUser.LoginId, loginId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var hostRole = await _uow.HostRoleRepository.GetByIdAsync(hostUser.HostRoleId);
            if (hostRole == null || !hostRole.IsActive || hostRole.IsSoftDeleted)
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            // Permission checks intentionally defer role-snapshot comparison to the database function so a
            // changed Host role returns the explicit stale-context result instead of silently authorizing.
            return new HostUserRequestContext
            {
                HostUserId = hostUser.Id,
                TokenHostRoleId = hostRoleId,
                CurrentHostRoleId = hostUser.HostRoleId,
                UserType = userType,
                TenantEncryptionKey = requireTenantEncryptionKey
                    ? EncryptionSanitizer.SuperSanitize(tenantEncryptionKey)
                    : string.Empty
            };
        }

        #endregion
    
    }

}
