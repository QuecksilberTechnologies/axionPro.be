using AutoMapper;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{


    public class GetDepartmentQuery : IRequest<ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        public GetDepartmentRequestDTO DTO { get; set; }

        public GetDepartmentQuery(GetDepartmentRequestDTO dto)
        {
            this.DTO = dto;
        }
    }

    public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetDepartmentQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public GetDepartmentQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetDepartmentQueryHandler> logger,
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
        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(
      GetDepartmentQuery request,
      CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 1️⃣ NULL SAFETY (VERY IMPORTANT)
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new ExtraPropRequestDTO();

                // ===============================
                // 2️⃣ GET TOKEN FROM HEADER
                // ===============================
                var bearerToken = _httpContextAccessor.HttpContext?
                    .Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                // ❌ Pehle: return Fail
                // ✅ Ab: throw (middleware handle karega)
                if (string.IsNullOrEmpty(bearerToken))
                    throw new UnauthorizedAccessException("Token not found.");

                // ===============================
                // 3️⃣ VALIDATE TOKEN
                // ===============================
                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    throw new UnauthorizedAccessException("Invalid or expired token.");

                // ===============================
                // 4️⃣ VALIDATE ACTIVE USER
                // ===============================
                long loggedInEmpId = await _unitOfWork.StoreProcedureRepository
                    .ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);

                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    throw new UnauthorizedAccessException("Unauthorized or inactive user.");
                }

                // ===============================
                // 5️⃣ DECRYPT DATA
                // ===============================
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("Missing tenantKey or UserEmployeeId.");
                    throw new ValidationErrorException("User invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string userEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);

                request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId_long(userEmpId, finalKey);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId_long(tokenClaims.TenantId, finalKey);

                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // ===============================
                // 6️⃣ VALIDATE DECODED VALUES
                // ===============================
                if (request.DTO.Prop.UserEmployeeId <= 0 || request.DTO.Prop.TenantId <= 0)
                {
                    _logger.LogWarning("Tenant or employee info invalid.");
                    throw new ValidationErrorException("Tenant or employee information missing.");
                }

                if (request.DTO.Prop.UserEmployeeId != loggedInEmpId)
                {
                    _logger.LogWarning("Employee mismatch. Req: {Req}, Logged: {Logged}",
                        request.DTO.Prop.UserEmployeeId, loggedInEmpId);

                    throw new UnauthorizedAccessException("Employee mismatch.");
                }

                // ===============================
                // 7️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions = await _permissionService
                    .GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));

                // 👉 Yahan tum actual permission name use karo
                // Example:
                // if (!permissions.Contains("ViewDepartment"))
                //     throw new UnauthorizedAccessException("No permission");

                // ===============================
                // 8️⃣ FETCH DATA
                // ===============================
                var responseDTO = await _unitOfWork.DepartmentRepository
                    .GetAsync(request.DTO);

                // ===============================
                // 9️⃣ HANDLE NO DATA (IMPORTANT)
                // ===============================
                if (responseDTO.Data == null || !responseDTO.Data.Any())
                {
                    _logger.LogWarning("No departments found.");

                    // ✅ Recommended: empty list with success
                    return ApiResponse<List<GetDepartmentResponseDTO>>
                        .Success(new List<GetDepartmentResponseDTO>(), "No data found.");
                }

                // ===============================
                // 🔟 SUCCESS RESPONSE
                // ===============================
                _logger.LogInformation("Total {Count} departments found.", responseDTO.TotalCount);

                return new ApiResponse<List<GetDepartmentResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Data
                };
            }
            catch (Exception ex)
            {
                // ❗ VERY IMPORTANT
                // ❌ Pehle: return Fail
                // ✅ Ab: throw (middleware handle karega)
                _logger.LogError(ex, "Error while fetching departments.");

                throw;
            }
        }
    }
}
