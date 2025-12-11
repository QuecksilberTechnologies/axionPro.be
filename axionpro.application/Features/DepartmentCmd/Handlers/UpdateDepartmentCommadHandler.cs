using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{
    public class UpdateDepartmentCommad : IRequest<ApiResponse<bool>>
    {
        public UpdateDepartmentRequestDTO DTO { get; set; }

        public UpdateDepartmentCommad(UpdateDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }
   
        public class UpdateDepartmentCommadHandler : IRequestHandler<UpdateDepartmentCommad, ApiResponse<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ILogger<UpdateDepartmentCommadHandler> _logger;
            private readonly ITokenService _tokenService;
            private readonly IPermissionService _permissionService;
            private readonly IConfiguration _config;
            private readonly IEncryptionService _encryptionService;
            private readonly IIdEncoderService _idEncoderService; // IIdEncoderService idEncoderService



        public UpdateDepartmentCommadHandler(
                IUnitOfWork unitOfWork,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                ILogger<UpdateDepartmentCommadHandler> logger,
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
        public async Task<ApiResponse<bool>> Handle(UpdateDepartmentCommad request, CancellationToken cancellationToken)
           {
            try
            {


                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (request.DTO.Prop == null)
                {
                    request.DTO.Prop = new ExtraPropRequestDTO();
                }
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
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<bool>.Fail("User invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                  
                
                if  (request.DTO.Id <= 0)
                {
                    _logger.LogWarning("❌  Id not correct.");
                    return ApiResponse<bool>.Fail("Tenant or employee information missing.");
                }

                if (request.DTO.Prop.UserEmployeeId <= 0 || request.DTO.Prop.TenantId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<bool>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO.Prop.UserEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         request.DTO.Prop.UserEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<bool>.Fail("Unauthorized: Employee mismatch.");
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data

                if (string.IsNullOrWhiteSpace(request.DTO.DepartmentName))
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Department name should not be empty or whitespace.",
                        Data = false
                    };
                }
                string? departmentName = request.DTO.DepartmentName?.Trim();

                if (request.DTO.Id == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid Id.",
                        Data = false
                    };

                }
                var isUpdated = await _unitOfWork.DepartmentRepository.UpdateAsync(request.DTO);

                if (!isUpdated)
                {
                    _logger.LogWarning("Department update failed or no record found. DepartmentId: {DepartmentId}", request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "No department was updated. Possibly not found or no changes detected.",
                        Data = false
                    };
                }

                _logger.LogInformation("Department with ID {DepartmentId} updated successfully.",
                    request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Department updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department. DepartmentId: {DepartmentId}",
                    request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to update department due to an internal error.",
                    Data = false
                };
            }
        }

    }
}
