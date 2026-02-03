using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.ITokenService
{
    // ITokenService.cs (Application Layer)
    public interface ITokenService
    {
           public Task <string> GenerateToken(GetTokenInfoDTO dto);
       
           bool ValidateToken(string token);
        public Task<string>  GenerateRefreshToken();
        // ✅ Naye methods for extracting info
        Task<string> GetUserInfoFromToken(string token);
        DateTime? GetExpiryFromToken(string token);
        Task<GetTokenInfoDTO?> GetUserInfoByLoginIdAsync(string loginId);
        public TokenClaimsModel? ValidateAndExtractClaims(string token);
    }


}
