using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace axionpro.infrastructure.CommonRequest
{
    public class CommonRequestService : ICommonRequestService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private readonly IIdEncoderService _encoder;
        private readonly ILogger<CommonRequestService> _logger;

        public CommonRequestService(
            IHttpContextAccessor ctx,
            IConfiguration cfg,
            IUnitOfWork uow,
            IIdEncoderService enc,
            ILogger<CommonRequestService> logger)
        {
            _context = ctx;
            _config = cfg;
            _uow = uow;
            _encoder = enc;
            _logger = logger;
        }

        public async Task<CommonDecodedResult> ValidateRequestAsync(string encodedUserId)
        {
            try
            {
                var token = RequestCommonHelper.ExtractBearerToken(_context.HttpContext);
                if (string.IsNullOrEmpty(token))
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token missing." };

                var claims = RequestCommonHelper.ValidateAndExtractClaims(token, _config);
                if (claims == null)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Token expired or invalid." };

                long loggedInId = await _uow.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(claims.UserId);
                if (loggedInId < 1)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "Inactive user." };
               
              

                var decoded = RequestCommonHelper.DecodeUserAndTenant(
                    encodedUserId,
                    claims.TenantId!,
                    claims.TenantEncriptionKey!,
                    _encoder
                );


                if (decoded.UserEmpId != loggedInId)
                    return new CommonDecodedResult { Success = false, ErrorMessage = "User mismatch." };

                if (decoded.TenantId <= 0 )
                {
                    _logger.LogWarning("❌ Tenant information missing .");
                    return new CommonDecodedResult { Success = false, ErrorMessage = "TenantId not correct" };
                }

                return new CommonDecodedResult
                {
                    Success = true,
                    LoggedInEmployeeId = loggedInId,
                    UserEmployeeId = decoded.UserEmpId,
                    TenantId = decoded.TenantId,
                    RoleId = SafeParser.TryParseInt(claims.RoleId),                   
                    Claims = claims
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CommonRequestService Error");
                return new CommonDecodedResult { Success = false, ErrorMessage = "Internal validation error." };
            }
        }
    }

}
