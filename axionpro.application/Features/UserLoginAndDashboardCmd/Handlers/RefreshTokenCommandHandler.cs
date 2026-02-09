using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.Hash;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    // =========================
    // COMMAND
    // =========================
    public class RefreshTokenCommand : IRequest<ApiResponse<TokenInfoResponseDTO>>
    {
        public RefreshTokenRequestDTO DTO { get; }

        public RefreshTokenCommand(RefreshTokenRequestDTO request)
        {
            DTO = request;
        }
    }

    // =========================
    // HANDLER
    // =========================
    public class RefreshTokenCommandHandler
     : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenInfoResponseDTO>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IIdEncoderService idEncoderService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _idEncoderService = idEncoderService;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenInfoResponseDTO>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // STEP 1: Incoming refresh token → HASH
                // =====================================================
                var incomingHashedToken =
                    HashHelper.Sha256(request.DTO.RefreshToken);

                // =====================================================
                // STEP 2: DB se VALID refresh token uthao
                // =====================================================
                var oldToken =
                    await _refreshTokenRepository
                        .GetValidByHashedTokenAsync(incomingHashedToken);

                if (oldToken == null)
                {
                    _logger.LogWarning(
                        "Invalid refresh token attempt | IP={IP}",
                        request.DTO.IpAddress);

                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("Invalid refresh token.");
                }

                if (oldToken.IsRevoked || oldToken.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogCritical(
                        "Refresh token reuse detected | LoginId={LoginId} | IP={IP}",
                        oldToken.LoginId,
                        request.DTO.IpAddress);

                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("Refresh token expired or revoked.");
                }

                // =====================================================
                // STEP 3: LoginId se MINIMUM info lao (JWT ke liye)
                // ❌ roles / permissions / menus nahi
                // =====================================================
                var tokenInfo =
                    await _tokenService .GetUserInfoByLoginIdAsync(oldToken.LoginId);

                if (tokenInfo == null)
                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("User not found.");

                long  empid = SafeParser.TryParseLong(tokenInfo.EmployeeId);
                long  tenantid = SafeParser.TryParseLong(tokenInfo.TenantId);
             
                
                
              
                string finalKey = EncryptionSanitizer.SuperSanitize(tokenInfo.TenantEncriptionKey);                 
                string encriptedEmployeeId = _idEncoderService.EncodeId_long(empid, finalKey);
                string encriptedTenantId = _idEncoderService.EncodeId_long(tenantid, finalKey);



                // =====================================================
                // STEP 4: NEW ACCESS TOKEN (JWT)
                // (TenantId / EmployeeId ENCODED string hi rahega)
                // =====================================================
                var newAccessToken =
                    await _tokenService.GenerateToken(tokenInfo);

                // =====================================================
                // STEP 5: NEW REFRESH TOKEN (ROTATION)
                // =====================================================
                var newRefreshToken =
                    await _tokenService.GenerateRefreshToken(); // PLAIN

                var newHashedRefreshToken =
                    HashHelper.Sha256(newRefreshToken);

                // =====================================================
                // STEP 6: OLD TOKEN REVOKE + REPLACE TRACK
                // =====================================================
                await _refreshTokenRepository
                    .UpdateReplacedByTokenAsync(
                        oldToken.Id,
                        newHashedRefreshToken);

                await _refreshTokenRepository
                    .RevokeAsync(oldToken.Id, request.DTO.IpAddress);

                // =====================================================
                // STEP 7: INSERT NEW REFRESH TOKEN
                // =====================================================
                await _refreshTokenRepository.InsertAsync(
                    new RefreshToken
                    {
                        LoginId = oldToken.LoginId,
                        Token = newHashedRefreshToken,
                        ExpiryDate = DateTime.UtcNow.AddDays(7),
                        IsRevoked = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByIp = request.DTO.IpAddress
                    });

                // =====================================================
                // STEP 8: CLIENT RESPONSE
                // =====================================================
                return ApiResponse<TokenInfoResponseDTO>.Success(
                    new TokenInfoResponseDTO
                    {
                        Token = newAccessToken,          // JWT
                        RefreshToken = newRefreshToken  // PLAIN
                    },
                    "Token refreshed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while refreshing token.");
                return ApiResponse<TokenInfoResponseDTO>
                    .Fail("Error while refreshing token.");
            }
        }
    }


}
