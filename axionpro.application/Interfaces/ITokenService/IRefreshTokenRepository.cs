using axionpro.domain.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.ITokenService
{
    public interface IRefreshTokenRepository
    {
        //  public Task<bool> InsertRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp);
        public Task<bool> SaveOrUpdateRefreshToken(string loginId, string token, DateTime expiryDate, string createdByIp);
        public Task<RefreshToken?> GetValidRefreshTokenAsync(string loginId, string token);
        public  Task<bool> RevokeRefreshTokenAsync(string loginId, string token, string revokedByIp);
      
    }
}
