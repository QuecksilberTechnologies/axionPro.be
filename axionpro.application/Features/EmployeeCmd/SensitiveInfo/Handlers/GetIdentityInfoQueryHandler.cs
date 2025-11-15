using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
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

namespace axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers
{
    #region Query Definition
    public class GetIdentityInfoQuery : IRequest<ApiResponse<List<GetIdentityResponseDTO>>>
    {
        public GetIdentityRequestDTO DTO { get; }

        public GetIdentityInfoQuery(GetIdentityRequestDTO dto)
        {
            DTO = dto ?? throw new ArgumentNullException(nameof(dto));
        }
    }
    #endregion

    #region Query Handler
    public class GetIdentityInfoQueryHandler : IRequestHandler<GetIdentityInfoQuery, ApiResponse<List<GetIdentityResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetIdentityInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;


        public GetIdentityInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetIdentityInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetIdentityResponseDTO>>> Handle(GetIdentityInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("User invalid.");
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
                // Actual EmployeeId
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                long decryptedActualEmployeeId = _idEncoderService.DecodeId(actualEmpId, finalKey);
                request.DTO.Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                long id = _idEncoderService.DecodeId(request.DTO.Id, finalKey);
                int Id = SafeParser.TryParseInt(id);

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");


                    // 🧩 STEP 4: Call Repository to get data          
                    //  return ApiResponse<List<GetContactResponseDTO>>.Fail("You do not have permission to add identity info.");
                }


                // 4️⃣ Fetch Data from Repository
                // 4️⃣ Fetch from repository
                var entity = await _unitOfWork.EmployeeIdentityRepository.GetInfo(request.DTO, decryptedActualEmployeeId, Id);

                if (entity == null || !entity.Items.Any())
                {
                    _logger.LogInformation("No Identity Info found for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);
                    return ApiResponse<List<GetIdentityResponseDTO>>.Fail("No identity info found.");
                }

                // 🔹 Step 6: Map or project entities to response DTO
                var result = ProjectionHelper.ToGetIdentityResponseDTOs(entity.Items, _idEncoderService, tenantKey);

                _logger.LogInformation("Successfully fetched identity info for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);

                return ApiResponse<List<GetIdentityResponseDTO>>.Success(result, "Identity info retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Identity Info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetIdentityResponseDTO>>.Fail(
                    "An unexpected error occurred while fetching identity info.",
                    new List<string> { ex.Message }
                );
            }
        }
    }
    #endregion
}
