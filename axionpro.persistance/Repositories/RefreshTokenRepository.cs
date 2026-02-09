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


        public RefreshTokenRepository(
            WorkforceDbContext context,
            ILogger<RefreshTokenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        //public async Task<bool> SaveOrUpdateRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Saving/Updating refresh token for LoginId: {loginId}");

        //        var statusParam = new SqlParameter("@Status", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //        var errorMsgParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

        //        var result = await _context.Database.ExecuteSqlRawAsync(
        //            "EXEC AxionPro.InsertOrUpdateRefreshToken @LoginId, @Token, @ExpiryDate, @CreatedByIp, @Status OUTPUT, @ErrorMessage OUTPUT",
        //            new SqlParameter("@LoginId", loginId),
        //            new SqlParameter("@Token", token),
        //            new SqlParameter("@ExpiryDate", expiryDate),
        //            new SqlParameter("@CreatedByIp", createdByIp),
        //            statusParam,
        //            errorMsgParam
        //        );

        //        int status = (int)statusParam.Value;
        //        string errorMessage = errorMsgParam.Value as string;

        //        if (status == 1)
        //        {
        //            _logger.LogInformation($"Refresh token successfully saved/updated for LoginId: {loginId}");
        //            return true;
        //        }
        //        else
        //        {
        //            _logger.LogError($"Failed to save/update refresh token for LoginId: {loginId}. Error: {errorMessage}");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Exception in SaveOrUpdateRefreshToken for LoginId {loginId}: {ex.Message}");
        //        return false;
        //    }
        //}

        //    - Token reuse / attack chain detect karne ke liye
        // =====================================================
        public async Task UpdateReplacedByTokenAsync(
            long refreshTokenId,
            string replacedByHashedToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Id == refreshTokenId);

                if (token == null)
                {
                    _logger.LogWarning(
                        "UpdateReplacedByTokenAsync: Token not found. Id={Id}",
                        refreshTokenId);
                    return;
                }

                // 🔥 YAHI ACTUAL UPDATE HOTA HAI
                token.ReplacedByToken = replacedByHashedToken;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while updating ReplacedByToken. TokenId={Id}",
                    refreshTokenId);
                throw; // let handler decide what to do
            }
        }
        public async Task<RefreshToken?> GetValidByHashedTokenAsync(string hashedToken)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.Token == hashedToken &&
                    t.IsRevoked == false &&
                    t.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<bool> InsertAsync(RefreshToken token)
        {
            try
            {
                await _context.RefreshTokens.AddAsync(token);
                var rows = await _context.SaveChangesAsync();

                return rows > 0; // ✅ true = saved
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert refresh token");
                return false;
            }
        }


        public async Task RevokeAsync(long refreshTokenId, string? revokedByIp)
        {
            var token = await _context.RefreshTokens.FindAsync(refreshTokenId);
            if (token == null) return;

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;

            await _context.SaveChangesAsync();
        }
    }
}






