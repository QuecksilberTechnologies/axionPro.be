// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides shared tenant and Host request validation.
// ================================================================

using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
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
        public async Task<CommonDecodedResult> ValidateRequestAsync()
        {
            try
            {
                var token = RequestCommonHelper.ExtractBearerToken(_context.HttpContext);
                if (string.IsNullOrEmpty(token))
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token missing." };

                var claims = RequestCommonHelper.ValidateAndExtractClaims(token, _config);
                if (claims == null)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token expired or invalid." };

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

        #region HostUser Token Validation

        /// <summary>
        /// Validates that the current authenticated JWT represents an active Host user with an active Host role.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when the current token, claims, Host user, or Host role are invalid.</exception>
        public async Task<long> ValidateHostUserRequestAsync()
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

            if (!long.TryParse(hostUserIdValue, out var hostUserId) ||
                !long.TryParse(hostRoleIdValue, out var hostRoleId) ||
                hostUserId <= 0 ||
                hostRoleId <= 0 ||
                string.IsNullOrWhiteSpace(loginId))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var hostUser = await _uow.HostUserRepository.GetByIdAsync(hostUserId);
            if (hostUser == null ||
                !hostUser.IsActive ||
                hostUser.IsSoftDeleted ||
                hostUser.HostRoleId != hostRoleId ||
                !string.Equals(hostUser.LoginId, loginId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            var hostRole = await _uow.HostRoleRepository.GetByIdAsync(hostUser.HostRoleId);
            if (hostRole == null || !hostRole.IsActive || hostRole.IsSoftDeleted)
            {
                throw new UnauthorizedAccessException(unauthorizedMessage);
            }

            return hostUser.Id;
        }

        #endregion
    
    }

}
