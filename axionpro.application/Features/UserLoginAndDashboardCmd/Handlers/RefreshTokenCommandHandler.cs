using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    // ✅ Command
    public class RefreshTokenCommand : IRequest<ApiResponse<TokenInfoResponseDTO>>
    {
        public RefreshTokenRequestDTO DTO { get; }

        public RefreshTokenCommand(RefreshTokenRequestDTO request)
        {
            DTO = request;
        }
    }

    // ✅ Handler
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenInfoResponseDTO>>
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

        public async Task<ApiResponse<TokenInfoResponseDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate refresh token
                var tokenRec = await _refreshTokenRepository.GetValidRefreshTokenAsync(request.DTO.UserLoginId, request.DTO.RefreshToken);
                if (tokenRec == null)
                    return ApiResponse<TokenInfoResponseDTO>.Fail("Invalid or expired refresh token.");

                // ✅ Step 2: Get user info by LoginId
                var tokenInfo = await _tokenService.GetUserInfoByLoginIdAsync(tokenRec.LoginId);
                if (tokenInfo == null)
                    return ApiResponse<TokenInfoResponseDTO>.Fail("User info not found for this token.");

                // ✅ Step 3: Generate new JWT token
                var newJwtToken = await _tokenService.GenerateToken(tokenInfo);

                // ✅ Step 4: Generate new Refresh Token (optional)
                var newRefreshToken = await _tokenService.GenerateRefreshToken();

                // ✅ Step 5: Save new refresh token to DB
                await _refreshTokenRepository.SaveOrUpdateRefreshToken(
                    tokenRec.LoginId,
                    newRefreshToken,
                    DateTime.UtcNow.AddDays(7),
                    tokenRec.CreatedByIp
                );

                // ✅ Step 6: Prepare response
                var responseDto = new TokenInfoResponseDTO
                {
                    
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken,
                   
                };

                return ApiResponse<TokenInfoResponseDTO>.Success(responseDto, "Token refreshed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while refreshing token.");
                return ApiResponse<TokenInfoResponseDTO>.Fail("Error while refreshing token.");
            }
        }
    }
}
