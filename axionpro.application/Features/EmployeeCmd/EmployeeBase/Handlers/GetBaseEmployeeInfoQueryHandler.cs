using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination; 
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetBaseEmployeeInfoQuery : IRequest<ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        public GetBaseEmployeeRequestDTO DTO { get; }

        public GetBaseEmployeeInfoQuery(GetBaseEmployeeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetBaseEmployeeInfoQueryHandler : IRequestHandler<GetBaseEmployeeInfoQuery, ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetBaseEmployeeInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetBaseEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBaseEmployeeInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetBaseEmployeeResponseDTO>>> Handle(GetBaseEmployeeInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("User invalid.");
                }

                long decryptedEmployeeId = Convert.ToInt64(_encryptionService.Decrypt(request.DTO.UserEmployeeId, tenantKey) ?? "0");
                long decryptedTenantId = Convert.ToInt64(tokenClaims.TenantId ?? "0");

                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee info missing in token.");
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                // 🧩 STEP 4: Validate EmployeeId match between token and request
                long requestEmployeeId = decryptedEmployeeId; // ✅ already decrypted
                if (requestEmployeeId != decryptedEmployeeId)
                {
                    _logger.LogWarning("❌ EmployeeId mismatch. TokenEmployeeId: {TokenEmp}, RequestEmployeeId: {ReqEmp}",
                        decryptedEmployeeId, requestEmployeeId);
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                // 🧩 STEP 5: Permission Check (optional)
                var tokenPermissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                // if (tokenPermissions == null || !tokenPermissions.Contains("ViewEmployeeBase"))
                //     return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("You do not have permission to view base employee info.");

                // 🧩 STEP 6: Fetch data from repository
                var entityPaged = await _unitOfWork.Employees.GetInfo(request.DTO, decryptedTenantId);
                if (entityPaged == null || !entityPaged.Items.Any())
                {
                    _logger.LogInformation("No Base Employee info found for EmployeeId: {EmpId}", decryptedEmployeeId);
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("No Base Employee info found.");
                }

                // 🧩 STEP 7: Encrypt Ids and Map using ProjectionHelper (optimized)
                var resultList = ProjectionHelper.ToGetBaseInfoResponseDTOs(entityPaged.Items, _encryptionService, tenantKey);

                // 🧩 STEP 8: Construct success response with pagination
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.SuccessPaginated(
                    data: resultList,
                    message: "Base Employee info retrieved successfully.",
                    pageNumber: entityPaged.PageNumber,
                    pageSize: entityPaged.PageSize,
                    totalRecords: entityPaged.TotalCount,
                    totalPages: entityPaged.TotalPages
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error while fetching Base Employee info for EmployeeId: {EmployeeId}", request.DTO?.UserEmployeeId);
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}
