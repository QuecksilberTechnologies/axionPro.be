using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using axionpro.application.Common.Helpers.EncryptionHelper;
 
using axionpro.application.Features.DepartmentCmd.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using axionpro.application.Common.Helpers.Converters;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class UpdateRoleCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateRoleRequestDTO DTO { get; set; }

        public UpdateRoleCommand(UpdateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService; // IIdEncoderService idEncoderService
       
        public UpdateRoleCommandHandler(
               
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateRoleCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IIdEncoderService idEncoderService)

            {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
           
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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
                // Decrypt / convert values
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
                    await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // ✅ STEP 5: Validate RoleName
                if (string.IsNullOrWhiteSpace(request.DTO.RoleName))
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Role name should not be empty or whitespace.",
                        Data = false
                    };
                }

                string? roleName = request.DTO.RoleName?.Trim();

                // ✅ STEP 6: Validate Role Id
                if (request.DTO.Id == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid role Id.",
                        Data = false
                    };
                }

                // ✅ STEP 7: Perform update via repository
                var isUpdated = await _unitOfWork.RoleRepository.UpdateAsync(request.DTO, decryptedEmployeeId);

                if (!isUpdated)
                {
                    _logger.LogWarning("Role update failed or not found. RoleId: {RoleId}", request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "No role was updated. Possibly not found or no changes detected.",
                        Data = false
                    };
                }

                _logger.LogInformation("✅ Role with ID {RoleId} updated successfully by EmployeeId {EmpId}.",
                    request.DTO.Id, decryptedEmployeeId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Role updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating role. RoleId: {RoleId}", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to update role due to an internal error.",
                    Data = false
                };
            }
        }
    }
}
