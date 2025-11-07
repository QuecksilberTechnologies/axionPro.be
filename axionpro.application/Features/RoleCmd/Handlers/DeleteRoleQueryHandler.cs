using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
 
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
 
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using axionpro.application.Common.Helpers.Converters;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class DeleteRoleQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRoleRequestDTO DTO { get; set; }

        public DeleteRoleQuery(DeleteRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeleteRoleQueryHandler : IRequestHandler<DeleteRoleQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteRoleQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;

        public DeleteRoleQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteRoleQueryHandler> logger,
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

        public async Task<ApiResponse<bool>> Handle(DeleteRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate Request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ DeleteRoleCommand received null request or DTO.");
                    return ApiResponse<bool>.Fail("Invalid delete request.");
                }

                // 🧩 STEP 2: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");

                // 🧩 STEP 3: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<bool>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 4: Tenant and Employee info from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<bool>.Fail("Invalid user or tenant context.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                string Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                request.DTO.Id = (_idEncoderService.DecodeId(Id, finalKey)).ToString();
                // 🧩 STEP 4: Validate all employee references


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
                if (request.DTO.Id ==null)
                {
                    _logger.LogWarning("⚠️ Invalid RoleId for delete operation: {RoleId}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("Invalid RoleId. It must be greater than zero.");
                }
               
                _logger.LogInformation("🗑️ Attempting to delete RoleId: {RoleId} for TenantId: {TenantId}", request.DTO.Id, decryptedTenantId);

                // 🧩 STEP 6: Repository call

                bool isDeleted = await _unitOfWork.RoleRepository.DeleteAsync(request.DTO, decryptedEmployeeId);

                if (!isDeleted)
                {
                    _logger.LogWarning("❌ Delete failed or role not found. RoleId: {RoleId}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("Delete failed. Role not found or already deleted.");
                }

                // 🧩 STEP 7: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Role deleted successfully. RoleId: {RoleId}, TenantId: {TenantId}", request.DTO.Id, decryptedTenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Role deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting RoleId: {RoleId}", request?.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to delete role due to an internal error.",
                    Data = false
                };
            }
        }
    }
}
