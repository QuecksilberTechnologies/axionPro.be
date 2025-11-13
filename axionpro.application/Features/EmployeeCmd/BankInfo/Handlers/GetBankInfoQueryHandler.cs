using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
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
        private readonly IIdEncoderService _idEncoderService;


        public GetBankInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBankInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
             IEncryptionService encryptionService, IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(GetBankInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {

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



                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                //UserEmployeeId
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                //Token TenantId
                string tokenTenant = EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenTenant, finalKey);
                //Id
                request.DTO.Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                long id = _idEncoderService.DecodeId(request.DTO.Id, finalKey);
                int Id = SafeParser.TryParseInt(id);
                // Actual EmployeeId
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                long decryptedActualEmployeeId = _idEncoderService.DecodeId(actualEmpId, finalKey);
                if(decryptedActualEmployeeId<0)

                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0 || decryptedActualEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                //    await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data GetBankReqestDTO dto, int id, long EmployeeId
                var bankEntities = await _unitOfWork.EmployeeBankRepository.GetInfoAsync(request.DTO, Id , decryptedActualEmployeeId );
                if (bankEntities == null || !bankEntities.Items.Any())
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("No bank info found.");

                // 5️⃣ Projection (fastest approach)
               var result = ProjectionHelper.ToGetBankResponseDTOs(bankEntities, _idEncoderService, tenantKey);

                // ✅ Correct paginated return
                return ApiResponse<List<GetBankResponseDTO>>.SuccessPaginatedPercentage(
                    data: result,
                    message: "Bank info retrieved successfully.",
                    pageNumber: bankEntities.PageNumber,
                    pageSize: bankEntities.PageSize,
                    totalRecords: bankEntities.TotalCount,
                    totalPages: bankEntities.TotalPages,
                    hasUploadedAll : bankEntities.HasUploadedAll,
                    completionPercentage: bankEntities.CompletionPercentage

                  
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching bank info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetBankResponseDTO>>.Fail("Failed to fetch bank info.", new List<string> { ex.Message });
            }
        }

    }
}
 