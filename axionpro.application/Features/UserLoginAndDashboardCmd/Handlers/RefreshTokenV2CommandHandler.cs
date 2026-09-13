// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Purpose     : Opt-in token-only refresh; retain the legacy endpoint until UI acceptance.
// Reference   : docs/AUTH_REFRESH_TOKEN_V2.md
// ============================================================================

using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.Hash;
using axionpro.application.Constants;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    #region Command

    /// <summary>Requests token-only refresh for an existing Host or Tenant session.</summary>
    public class RefreshTokenV2Command : IRequest<ApiResponse<RefreshTokenV2ResponseDTO>>
    {
        public RefreshTokenRequestDTO DTO { get; }

        public RefreshTokenV2Command(RefreshTokenRequestDTO request)
        {
            DTO = request;
        }
    }

    #endregion

    /// <summary>
    /// Validates current session eligibility without fetching presentation data or menu permissions.
    /// Uses the existing token service, repositories and centralized exception pipeline.
    /// </summary>
    public class RefreshTokenV2CommandHandler
        : IRequestHandler<RefreshTokenV2Command, ApiResponse<RefreshTokenV2ResponseDTO>>
    {
        #region Dependencies

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ILogger<RefreshTokenV2CommandHandler> _logger;

        public RefreshTokenV2CommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IIdEncoderService idEncoderService,
            ILogger<RefreshTokenV2CommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _idEncoderService = idEncoderService;
            _logger = logger;
        }

        #endregion

        #region Refresh

        public async Task<ApiResponse<RefreshTokenV2ResponseDTO>> Handle(
            RefreshTokenV2Command request,
            CancellationToken cancellationToken)
        {
            if (request?.DTO == null || string.IsNullOrWhiteSpace(request.DTO.RefreshToken))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            if (request.DTO.IpAddress?.Length > 50)
            {
                throw new ValidationErrorException("IpAddress must not exceed 50 characters.");
            }

            var oldToken = await _refreshTokenRepository.GetByHashedTokenAsync(
                HashHelper.Sha256(request.DTO.RefreshToken));
            if (oldToken == null || oldToken.IsRevoked == true || oldToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid, expired or revoked refresh token.");
            }

            var validOwner = oldToken.UserType switch
            {
                (short)LoginUserType.Host => oldToken.HostUserId.HasValue && !oldToken.LoginCredentialId.HasValue,
                (short)LoginUserType.TenantEmployee => oldToken.LoginCredentialId.HasValue && !oldToken.HostUserId.HasValue,
                _ => false
            };
            if (!validOwner)
            {
                throw new UnauthorizedAccessException("Invalid refresh token owner.");
            }

            string accessToken;
            if (oldToken.UserType == (short)LoginUserType.Host)
            {
                accessToken = await GenerateHostAccessTokenAsync(oldToken);
            }
            else
            {
                // Preserve legacy business-failure envelopes for inactive employees/subscriptions.
                var tenantResult = await GenerateTenantAccessTokenAsync(oldToken, cancellationToken);
                if (!tenantResult.IsSucceeded)
                {
                    return new ApiResponse<RefreshTokenV2ResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = tenantResult.Message
                    };
                }
                accessToken = tenantResult.Data!;
            }

            var expiry = _tokenService.GetExpiryFromToken(accessToken);
            if (string.IsNullOrWhiteSpace(accessToken) || !expiry.HasValue)
            {
                throw new InvalidOperationException("The access token could not be issued.");
            }

            var response = await RotateAsync(oldToken, accessToken, expiry.Value, request.DTO.IpAddress, cancellationToken);
            return ApiResponse<RefreshTokenV2ResponseDTO>.Success(response, "Token refreshed successfully.");
        }

        #endregion

        #region Current Owner Validation

        private async Task<string> GenerateHostAccessTokenAsync(RefreshToken oldToken)
        {
            var hostUser = await _unitOfWork.HostUserRepository.GetByIdAsync(oldToken.HostUserId!.Value);
            if (hostUser == null || !hostUser.IsActive || hostUser.IsSoftDeleted ||
                !string.Equals(hostUser.LoginId, oldToken.LoginId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            var role = await _unitOfWork.HostRoleRepository.GetByIdAsync(hostUser.HostRoleId);
            if (role == null || !role.IsActive || role.IsSoftDeleted)
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            return await _tokenService.GenerateHostToken(new HostTokenInfoDTO
            {
                HostUserId = hostUser.Id,
                HostRoleId = hostUser.HostRoleId,
                LoginId = hostUser.LoginId,
                Name = hostUser.Name,
                Email = hostUser.Email
            });
        }

        private async Task<ApiResponse<string>> GenerateTenantAccessTokenAsync(
            RefreshToken oldToken,
            CancellationToken cancellationToken)
        {
            var credential = await _unitOfWork.UserLoginRepository.GetActiveByIdAsync(oldToken.LoginCredentialId!.Value);
            if (credential == null || string.IsNullOrWhiteSpace(credential.LoginId) ||
                !string.Equals(credential.LoginId, oldToken.LoginId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("Invalid refresh token owner.");
            }

            var employeeId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(credential.LoginId);
            if (employeeId < 1)
            {
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            var employee = await _unitOfWork.Employees.GetSingleRecordAsync(employeeId, true);
            if (employee == null)
            {
                return new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "Employee not active. Please contact admin."
                };
            }

            var subscription = await _unitOfWork.TenantSubscriptionRepository.GetValidateTenantPlan(
                new TenantSubscriptionPlanRequestDTO { TenantId = employee.TenantId });
            if (subscription == null || !subscription.SubscriptionEndDate.HasValue ||
                subscription.SubscriptionEndDate.Value.Date < DateTime.Today)
            {
                return new ApiResponse<string>
                {
                    IsSucceeded = false,
                    Message = "Your subscription has expired. Please contact admin to renew the plan."
                };
            }

            var roles = await _unitOfWork.UserRoleRepository.GetEmployeeRolesWithDetailsByIdAsync(
                employeeId, employee.TenantId, cancellationToken);
            var primaryRole = roles?.FirstOrDefault(x => x.IsPrimaryRole == true && x.IsActive);
            if (primaryRole?.Role == null || !primaryRole.RoleId.HasValue)
            {
                throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            }

            var key = await _unitOfWork.TenantEncryptionKeyRepository.GetActiveKeyByTenantIdAsync(employee.TenantId);
            if (key == null || string.IsNullOrWhiteSpace(key.EncryptionKey))
            {
                throw new InvalidOperationException("Tenant encryption key not found or invalid.");
            }

            var finalKey = EncryptionSanitizer.SuperSanitize(key.EncryptionKey);
            // Keep existing JWT claims and signing policy; only the response/presentation reads are reduced.
            var token = await _tokenService.GenerateTenantToken(new GetTokenInfoDTO
            {
                TenantEncriptionKey = finalKey,
                TenantId = _idEncoderService.EncodeId_long(employee.TenantId, finalKey),
                EmployeeId = _idEncoderService.EncodeId_long(employeeId, finalKey).Trim(),
                UserId = credential.LoginId,
                RoleId = primaryRole.RoleId.Value.ToString(),
                RoleTypeId = primaryRole.Role.RoleType.ToString(),
                RoleTypeName = primaryRole.Role.RoleName ?? string.Empty,
                EmployeeTypeId = employee.EmployeeTypeId.ToString(),
                GenderId = employee.GenderId.ToString(),
                GenderName = employee.GenderName,
                Email = credential.LoginId,
                FullName = ((employee.FirstName ?? "") + "-" + (employee.LastName ?? "")).Trim('-'),
                Expiry = DateTime.UtcNow.AddMinutes(15),
                TokenPurpose = ConstantValues.Auth.ToString()
            });
            return ApiResponse<string>.Success(token, "Token generated.");
        }

        #endregion

        #region Existing Refresh Rotation Policy

        private async Task<RefreshTokenV2ResponseDTO> RotateAsync(
            RefreshToken oldToken,
            string accessToken,
            DateTime accessExpiry,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            var raw = await _tokenService.GenerateRefreshToken();
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new InvalidOperationException("The refresh token could not be issued.");
            }
            var hash = HashHelper.Sha256(raw);
            var createdAt = DateTime.UtcNow;
            var refreshExpiry = createdAt.AddDays(7);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var claimed = await _refreshTokenRepository.TryClaimForRotationAsync(
                    oldToken.Id, hash, ipAddress, DateTime.UtcNow, cancellationToken);
                if (!claimed)
                {
                    throw new UnauthorizedAccessException("Refresh token expired or already consumed.");
                }
                var inserted = await _refreshTokenRepository.InsertAsync(new RefreshToken
                {
                    LoginId = oldToken.LoginId,
                    UserType = oldToken.UserType,
                    LoginCredentialId = oldToken.LoginCredentialId,
                    HostUserId = oldToken.HostUserId,
                    Token = hash,
                    CreatedAt = createdAt,
                    ExpiryDate = refreshExpiry,
                    CreatedByIp = ipAddress,
                    IsRevoked = false
                });
                if (!inserted)
                {
                    throw new InvalidOperationException("The refresh token could not be persisted.");
                }
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            } 
            catch (Exception exception)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                _logger.LogError(exception, "Token-only refresh rotation failed.");
                throw;
            }

            return new RefreshTokenV2ResponseDTO
            {
                Token = accessToken,
                RefreshToken = raw,
                TokenExpiry = accessExpiry,
                RefreshTokenExpiresAtUtc = refreshExpiry
            };
        }

        #endregion
    }
}
