using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using axionpro.application.Features.DepartmentCmd.Handlers;
namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class DeleteDesignationQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteDesignationRequestDTO DTO { get; set; }

        public DeleteDesignationQuery(DeleteDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handler for soft deleting a designation.
    /// </summary>
    public class DeleteDesignationQueryHandler : IRequestHandler<DeleteDesignationQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteDesignationQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        public DeleteDesignationQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteDesignationQueryHandler> logger,
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

        public async Task<ApiResponse<bool>> Handle(DeleteDesignationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<bool>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<bool>.Fail("User invalid.");
                }
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                string Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);

                // 🧩 STEP 4: Validate all employee references

                int id = SafeParser.TryParseInt(request.DTO.Id);
                if (id <= 0)
                {
                    return ApiResponse<bool>.Fail("Invalid role id.");

                }


                if (decryptedEmployeeId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<bool>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<bool>.Fail("Unauthorized: Employee mismatch.");
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                // 🧩 STEP 5: Validate RoleId
                if (request.DTO.Id == null)
                {
                    _logger.LogWarning("⚠️ Invalid RoleId for delete operation: {RoleId}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("Invalid RoleId. It must be greater than zero.");
                }

                _logger.LogInformation("🗑️ Attempting to delete RoleId: {RoleId} for TenantId: {TenantId}", request.DTO.Id, decryptedTenantId);

                // 🧩 STEP 6: Repository call             

                // 🧩 STEP 6: Call repository for delete
                var isDeleted = await _unitOfWork.DesignationRepository.DeleteDesignationAsync(request.DTO, decryptedEmployeeId,id);

                if (isDeleted)
                {
                    _logger.LogInformation("✅ Designation deleted successfully. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, decryptedTenantId);

                    return ApiResponse<bool>.Success(true, "Designation deleted successfully.");
                }

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Designation not found or could not be deleted.",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting designation Id: {Id}", request.DTO?.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to delete designation.",
                    Data = false
                };
            }
        }
    }
}
