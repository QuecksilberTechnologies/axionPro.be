using axionpro.domain.Entity;
using System;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetValidByHashedTokenAsync(string hashedToken);

        Task<bool> InsertAsync(RefreshToken token);

        Task RevokeAsync(long refreshTokenId, string? revokedByIp);
        // 🔥 NEW METHOD (ROTATION CHAIN TRACKING)
        // Old refresh token ke andar yeh batata hai
        // ki uski jagah kaunsa naya token aaya
        Task UpdateReplacedByTokenAsync(
            long refreshTokenId,
            string replacedByHashedToken
        );

        //   public Task<bool> SaveOrUpdateRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp);
    }
}
