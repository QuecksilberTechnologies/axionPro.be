using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.ITokenService
{
    public interface IRefreshTokenRepository
    {
        //   public Task<bool> InsertRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp);
        public Task<bool> SaveOrUpdateRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp);

        public Task<string?> GetValidRefreshTokenAsync(string loginId, string token);

        public Task<bool> RevokeRefreshTokenAsync(string loginId, string token);

        public Task<bool> DeleteExpiredTokensAsync();
    }
}
