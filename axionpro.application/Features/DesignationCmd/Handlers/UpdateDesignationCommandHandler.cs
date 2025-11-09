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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class UpdateDesignationCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateDesignationRequestDTO DTO { get; set; }

        public UpdateDesignationCommand(UpdateDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// ✅ Ideal handler for updating an existing Designation (with JWT + Validation)
    /// </summary>
    public class UpdateDesignationCommandHandler : IRequestHandler<UpdateDesignationCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateDesignationCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public UpdateDesignationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateDesignationCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDesignationCommand request, CancellationToken cancellationToken)
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

                // 🧩 STEP 3: Decrypt Employee and Tenant
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
                int id =  SafeParser.TryParseInt(Id);               
                if(id<=0)
                {
                    _logger.LogWarning("❌  Id not correct.");
                    return ApiResponse<bool>.Fail("Tenant or employee information missing.");
                }
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

                // 🧩 STEP 5: Validate name and existence
                string? designationName = request.DTO.DesignationName?.Trim();
                if (string.IsNullOrWhiteSpace(designationName))
                    return ApiResponse<bool>.Fail("Designation name should not be empty.");
 
                // 🧩 STEP 6: Update in repository
                bool isUpdated = await _unitOfWork.DesignationRepository.UpdateDesignationAsync(request.DTO, decryptedEmployeeId,id);

                if (!isUpdated)
                {
                    _logger.LogWarning("⚠️ Update failed for DesignationId {Id}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("No designation was updated.");
                }

                // 🧩 STEP 7: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Designation updated successfully. Id: {Id}", request.DTO.Id);

                return ApiResponse<bool>.Success(true, "Designation updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating designation: {Message}", ex.Message);
                return ApiResponse<bool>.Fail($"An error occurred while updating designation. {ex.Message}");
            }
        }
    }
}
