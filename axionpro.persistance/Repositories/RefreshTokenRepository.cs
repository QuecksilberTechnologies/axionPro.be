using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

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

     
        public async Task<bool> RevokeRefreshTokenAsync(string loginId, string token, string revokedByIp)
        {
            try
            {
                var tokenRec = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.LoginId == loginId && t.Token == token);

                if (tokenRec == null)
                {
                    _logger.LogWarning($"No token found to revoke for LoginId: {loginId}");
                    return false;
                }

                tokenRec.IsRevoked = true;
                tokenRec.RevokedAt = DateTime.UtcNow;
                tokenRec.RevokedByIp = revokedByIp;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Refresh token revoked for LoginId: {loginId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RevokeRefreshTokenAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetValidRefreshTokenAsync(string refreshToken)
        {
            var record = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && x.IsRevoked == false && x.ExpiryDate > DateTime.UtcNow);

            return record?.LoginId;
        }
        // ✅ Get Valid Refresh Token (Stored Procedure)
        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string loginId, string token)
        {
            try
            {
                _logger.LogInformation($"Fetching valid refresh token for LoginId: {loginId}");

                var tokenRec = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t =>
                        t.LoginId == loginId &&
                        t.Token == token &&
                        t.IsRevoked == false &&
                        t.ExpiryDate > DateTime.UtcNow);

                if (tokenRec == null)
                {
                    _logger.LogWarning($"No valid refresh token found for LoginId: {loginId}");
                }

                return tokenRec;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetValidRefreshTokenAsync for LoginId {loginId}: {ex.Message}");
                return null;
            }
        }




    }
}
