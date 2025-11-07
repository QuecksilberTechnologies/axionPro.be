using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using axionpro.application.Interfaces.ITokenService;

namespace axionpro.persistance.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(WorkforceDbContext context, ILogger<RefreshTokenRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ✅ Insert Refresh Token (Stored Procedure)
        public async Task<bool> SaveOrUpdateRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp)
        {
            try
            {
                _logger.LogInformation($"Saving/Updating refresh token for LoginId: {loginId}");

                var statusParam = new SqlParameter("@Status", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var errorMsgParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AxionPro.InsertOrUpdateRefreshToken @LoginId, @Token, @ExpiryDate, @CreatedByIp, @Status OUTPUT, @ErrorMessage OUTPUT",
                    new SqlParameter("@LoginId", loginId),
                    new SqlParameter("@Token", token),
                    new SqlParameter("@ExpiryDate", expiryDate),
                    new SqlParameter("@CreatedByIp", createdByIp),
                    statusParam,
                    errorMsgParam
                );

                int status = (int)statusParam.Value;
                string errorMessage = errorMsgParam.Value as string;

                if (status == 1)
                {
                    _logger.LogInformation($"Refresh token successfully saved/updated for LoginId: {loginId}");
                    return true;
                }
                else
                {
                    _logger.LogError($"Failed to save/update refresh token for LoginId: {loginId}. Error: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in SaveOrUpdateRefreshToken for LoginId {loginId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Get Valid Refresh Token (Stored Procedure)
        public async Task<string?> GetValidRefreshTokenAsync(string loginId, string token)
        {
            try
            {
                _logger.LogInformation($"Fetching valid refresh token for LoginId: {loginId}");

                var refreshTokenParam = new SqlParameter("@Token", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AxionPro.GetValidRefreshToken @LoginId, @Token OUTPUT",
                    new SqlParameter("@LoginId", loginId),
                    refreshTokenParam
                );

                string? refreshToken = refreshTokenParam.Value as string;

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation($"Valid refresh token found for LoginId: {loginId}");
                }
                else
                {
                    _logger.LogWarning($"No valid refresh token found for LoginId: {loginId}");
                }

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving refresh token for LoginId: {loginId} - {ex.Message}");
                return null;
            }
        }

        // ✅ Revoke Refresh Token (Stored Procedure)
        public async Task<bool> RevokeRefreshTokenAsync(string loginId, string token)
        {
            try
            {
                _logger.LogInformation($"Revoking refresh token for LoginId: {loginId}");

                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AxionPro.RevokeRefreshToken @LoginId, @Token",
                    new SqlParameter("@LoginId", loginId),
                    new SqlParameter("@Token", token)
                );

                if (result > 0)
                {
                    _logger.LogInformation($"Refresh token revoked successfully for LoginId: {loginId}");
                    return true;
                }

                _logger.LogWarning($"No refresh token found to revoke for LoginId: {loginId}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revoking refresh token for LoginId: {loginId} - {ex.Message}");
                return false;
            }
        }

        // ✅ Delete Expired Tokens (Stored Procedure)
        public async Task<bool> DeleteExpiredTokensAsync()
        {
            try
            {
                _logger.LogInformation("Deleting expired refresh tokens.");

                var result = await _context.Database.ExecuteSqlRawAsync("EXEC AxionPro.DeleteExpiredTokens");

                if (result > 0)
                {
                    _logger.LogInformation("Expired refresh tokens deleted successfully.");
                    return true;
                }

                _logger.LogWarning("No expired refresh tokens found to delete.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting expired refresh tokens - {ex.Message}");
                return false;
            }
        }
    }
}
