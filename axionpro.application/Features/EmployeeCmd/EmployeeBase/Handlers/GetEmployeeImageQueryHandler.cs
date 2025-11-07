using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetEmployeeImageQuery : IRequest<ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        public GetEmployeeImageRequestDTO DTO { get; }

        public GetEmployeeImageQuery(GetEmployeeImageRequestDTO dTO)
        {
            DTO = dTO;
        }
    }


    public class GetEmployeeImageQueryHandler : IRequestHandler<GetEmployeeImageQuery, ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetEmployeeImageQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetEmployeeImageQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetEmployeeImageQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetEmployeeImageReponseDTO>>> Handle(GetEmployeeImageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                // ✅ simplified: agar tenantKey ya UserEmployeeId missing hai to fail
                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("User invalid.");
                }

                long decryptedEmployeeId = Convert.ToInt64(_encryptionService.Decrypt(request.DTO.UserEmployeeId, tenantKey) ?? "0");
                long decryptedTenantId = Convert.ToInt64(tokenClaims.TenantId ?? "0");

                // ✅ simplified: dono valid hone chahiye
                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee info missing in token.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Tenant or employee information missing.");
                }

                // 🧩 STEP 5: Validate EmployeeId match between token and request
                long requestEmployeeId = decryptedEmployeeId; // ✅ optimized: already decrypted value
                if (requestEmployeeId != decryptedEmployeeId)
                {
                    _logger.LogWarning("❌ EmployeeId mismatch. TokenEmployeeId: {TokenEmp}, RequestEmployeeId: {ReqEmp}",
                        decryptedEmployeeId, requestEmployeeId);
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                // 🧩 STEP 6: Permission Check (optional)
                var tokenPermissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                // if (tokenPermissions == null || !tokenPermissions.Contains("ViewEmployeeBase"))
                //     return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("You do not have permission to view base employee info.");

                // 🧩 STEP 10: Fetch Data from Repository
                var entityPaged = await _unitOfWork.Employees.GetImage(request.DTO, decryptedTenantId);

                if (entityPaged == null || !entityPaged.Items.Any())
                {
                    _logger.LogInformation("No images found for EmployeeId: {EmpId}", decryptedEmployeeId);
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("No images found for the employee.");
                }

                // 🧩 STEP 8: Encrypt Ids and Map using ProjectionHelper (optimized)
                var resultList = ProjectionHelper.ToGetEmployeeImageResponseDTOs(entityPaged.Items, _encryptionService, tenantKey);

                // 🧩 STEP 9: Construct success response with pagination
                return ApiResponse<List<GetEmployeeImageReponseDTO>>.SuccessPaginated(
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
                _logger.LogError(ex, "🔥 Error while fetching employee images.");
                return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}
