using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
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


namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{
    public class GetContactInfoQuery : IRequest<ApiResponse<List<GetContactResponseDTO>>>
    {
        public GetContactRequestDTO DTO { get; set; }

        public GetContactInfoQuery(GetContactRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class GetContactInfoQueryHandler : IRequestHandler<GetContactInfoQuery, ApiResponse<List<GetContactResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetContactInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetContactInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetContactInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetContactResponseDTO>>> Handle(GetContactInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string encryptedFromDb = EncryptionSanitizer.SuperSanitize(request.DTO.UserEmployeeId);
                string key = EncryptionSanitizer.SuperSanitize(tenantKey);
                string decryptedString = _encryptionService.Decrypt(encryptedFromDb, key);
                long decryptedUserEmployeeId = SafeParser.TryParseLong(decryptedString ?? "0");
                long decryptedTenantId = SafeParser.TryParseLong(tokenClaims.TenantId ?? "0");
                long tokenEmployeeId = SafeParser.TryParseLong(tokenClaims.EmployeeId ?? "0");
                long Id = SafeParser.TryParseLong(_encryptionService.Decrypt(request.DTO.Id, tenantKey) ?? "0");
                 
                // 🧩 STEP 4: Validate all employee references
                if (decryptedTenantId <= 0 || decryptedUserEmployeeId <= 0 || tokenEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedUserEmployeeId == tokenEmployeeId && tokenEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, TokenEmpId: {TokenEmp}, LoggedEmpId: {LoggedEmp}",
                        decryptedUserEmployeeId, tokenEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("AddIdentityInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("You do not have permission to add identity info.");
                }
                // 4️⃣ Fetch Data from Repository
                // 4️⃣ Fetch from repository
                PagedResponseDTO<GetContactResponseDTO> Entity = await _unitOfWork.EmployeeContactRepository.GetInfo(request.DTO, decryptedUserEmployeeId, Id);
                if (Entity == null || !Entity.Items.Any())
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("No Contact info found.");

                // 5️⃣ Projection (fastest approach)
                var result = ProjectionHelper.ToGetContactResponseDTOs(Entity.Items, _encryptionService, tenantKey);

                // 6️⃣ Return success response    
                return ApiResponse<List<GetContactResponseDTO>>.Success(result, "Contact info retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Contact info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetContactResponseDTO>>.Fail("Failed to fetch Contact info.", new List<string> { ex.Message });
            }
        }

    }
}
