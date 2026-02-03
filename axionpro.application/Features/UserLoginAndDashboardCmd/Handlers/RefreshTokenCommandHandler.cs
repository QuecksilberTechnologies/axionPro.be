using axionpro.application.Common.Helpers.Hash;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Interfaces.IRepositories;
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
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenInfoResponseDTO>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // STEP 1: Client se aaya hua PLAIN refresh token HASH karo
                // =====================================================
                var incomingHashedToken =
                    HashHelper.Sha256(request.DTO.RefreshToken);

                // =====================================================
                // STEP 2: DB me HASHED token se record dhundo
                // (YAHAN SELECT QUERY CHALTI HAI)
                // =====================================================
                var oldTokenRec =
                    await _refreshTokenRepository
                        .GetValidByHashedTokenAsync(incomingHashedToken);

                if (oldTokenRec == null)
                {
                    // 🔴 POSSIBLE ATTACK / INVALID TOKEN
                    // Token DB me hai hi nahi
                    _logger.LogWarning(
                        "Invalid refresh token attempt. IP={IP}",
                        request.DTO.IpAddress);

                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("Invalid refresh token.");
                }

                // =====================================================
                // STEP 3: SAFETY CHECK (extra layer)
                // Normally ye condition already DB query me hoti hai
                // =====================================================
                if (oldTokenRec.ExpiryDate < DateTime.UtcNow)
                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("Refresh token expired.");
                
                if (oldTokenRec.IsRevoked)
                {
                    _logger.LogCritical(
                        "REFRESH TOKEN REUSE DETECTED | LoginId={LoginId} | IP={IP}",
                        oldTokenRec.LoginId,
                        request.DTO.IpAddress);

                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("Refresh token already revoked.");
                }


                // =====================================================
                // STEP 4: LoginId se user info nikaalo
                // =====================================================
                var tokenInfo =
                    await _tokenService
                        .GetUserInfoByLoginIdAsync(oldTokenRec.LoginId);

                if (tokenInfo == null)
                    return ApiResponse<TokenInfoResponseDTO>
                        .Fail("User not found.");

                // =====================================================
                // STEP 5: NEW ACCESS TOKEN (JWT) banao
                // =====================================================
                var newAccessToken =
                    await _tokenService.GenerateToken(tokenInfo);

                // =====================================================
                // STEP 6: NEW REFRESH TOKEN banao (ROTATION START)
                // =====================================================
                var newRefreshToken =
                    await _tokenService.GenerateRefreshToken(); // PLAIN

                var newHashedToken =
                    HashHelper.Sha256(newRefreshToken);          // HASHED

                // =====================================================
                // STEP 7: OLD TOKEN REVOKE
                // 🔥 YAHAN UPDATE QUERY CHALTI HAI
                // =====================================================
                await _refreshTokenRepository.RevokeAsync(
                    oldTokenRec.Id,
                    request.DTO.IpAddress
                );

                // =====================================================
                // STEP 8: OLD TOKEN ME "ReplacedByToken" UPDATE
                // 🔥 ATTACK CHAIN TRACK YAHI BANTA HAI
                // =====================================================
                await _refreshTokenRepository.UpdateReplacedByTokenAsync( oldTokenRec.Id,
                        newHashedToken
                    );

                // =====================================================
                // STEP 9: NEW REFRESH TOKEN INSERT
                // 🔥 YAHAN INSERT QUERY CHALTI HAI
                // =====================================================
                await _refreshTokenRepository.InsertAsync(
                    new  RefreshToken
                    {
                        LoginId = oldTokenRec.LoginId,
                        Token = newHashedToken,
                        ExpiryDate = DateTime.UtcNow.AddDays(7),
                        IsRevoked = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByIp = request.DTO.IpAddress,
                        RevokedAt = null,
                        RevokedByIp = null,
                        ReplacedByToken = null
                    });

                // =====================================================
                // STEP 10: CLIENT KO NEW TOKENS RETURN
                // =====================================================
                return ApiResponse<TokenInfoResponseDTO>.Success(
                    new TokenInfoResponseDTO
                    {
                        Token = newAccessToken,          // JWT
                        RefreshToken = newRefreshToken  // PLAIN
                    },
                    "Token refreshed successfully."
                );


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
