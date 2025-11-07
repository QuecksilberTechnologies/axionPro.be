using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class GetBankInfoQuery : IRequest<ApiResponse<List<GetBankResponseDTO>>>
    {
        public GetBankReqestDTO DTO { get; set; }

        public GetBankInfoQuery(GetBankReqestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetBankInfoQueryHandler : IRequestHandler<GetBankInfoQuery, ApiResponse<List<GetBankResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetBankInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetBankInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBankInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(GetBankInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                long decryptedUserEmployeeId = Convert.ToInt64(_encryptionService.Decrypt(request.DTO.UserEmployeeId, tenantKey) ?? "0");
                long decryptedActualEmployeeId = Convert.ToInt64(_encryptionService.Decrypt(request.DTO.EmployeeId, tenantKey) ?? "0");
                long tokenEmployeeId = Convert.ToInt64(tokenClaims.EmployeeId ?? "0");
                long decryptedTenantId = Convert.ToInt64(tokenClaims.TenantId ?? "0");
                long Id = SafeParser.TryParseLong(request.DTO.Id);
               
                // 🧩 STEP 4: Validate all employee references
                if (decryptedTenantId <= 0 || decryptedUserEmployeeId <= 0 || tokenEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedUserEmployeeId == tokenEmployeeId && tokenEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, TokenEmpId: {TokenEmp}, LoggedEmpId: {LoggedEmp}",
                        decryptedUserEmployeeId, tokenEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("AddIdentityInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add identity info.");
                }
                
                PagedResponseDTO<GetBankResponseDTO> bankEntities = await _unitOfWork.EmployeeBankRepository.GetInfo(request.DTO, decryptedActualEmployeeId,Id );
                if (bankEntities == null || !bankEntities.Items.Any())
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("No bank info found.");

                // 5️⃣ Projection (fastest approach)
                var result = ProjectionHelper.ToGetBankResponseDTOs(bankEntities.Items, _encryptionService, tenantKey, request.DTO.EmployeeId);

                // 6️⃣ Return success response
                return ApiResponse<List<GetBankResponseDTO>>.Success(result, "Bank info retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching bank info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetBankResponseDTO>>.Fail("Failed to fetch bank info.", new List<string> { ex.Message });
            }
        }

    }
}
 