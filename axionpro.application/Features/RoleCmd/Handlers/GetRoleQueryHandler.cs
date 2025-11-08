using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
 
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Role;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class GetRoleQuery : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public GetRoleRequestDTO DTO { get; set; }

        public GetRoleQuery(GetRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, ApiResponse<List<GetRoleResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetRoleQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
       

        public GetRoleQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetRoleQueryHandler> logger,
            ITokenService tokenService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _config = config;
            _encryptionService = encryptionService;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;

        }

        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate Request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ GetRoleQuery received null or invalid request.");
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Invalid request data.");
                }

                // 🧩 STEP 2: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 3: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 4: Decrypt Tenant & Employee info
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId in GetRoleQuery.");
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Invalid user context.");
                }
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                request.DTO.Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                int id = SafeParser.TryParseInt(request.DTO.Id);
                 request.DTO.RoleType = EncryptionSanitizer.CleanEncodedInput(request.DTO.RoleType);
                 request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                 request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
             

                
                var responseDTO = await _unitOfWork.RoleRepository.GetAsync(request.DTO, decryptedTenantId, id );

                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("⚠️ No roles found for TenantId: {TenantId}", decryptedTenantId);
                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No roles found.",
                        Data = new List<GetRoleResponseDTO>(),
                        PageNumber = request.DTO.PageNumber,
                        PageSize = request.DTO.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0
                    };
                }

              //  var encryptedList = ProjectionHelper.ToGetRoleResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);


                // 🧩 STEP 7: Success response
                _logger.LogInformation("✅ {Count} roles retrieved successfully for TenantId: {TenantId}",
                    responseDTO.TotalCount, decryptedTenantId);

                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Roles retrieved successfully.",
                    Data = responseDTO.Items,
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages
                };
            }
            catch (Exception ex)
            {
                // 🧩 STEP 8: Error Handling
                _logger.LogError(ex, "❌ Error occurred while retrieving roles.");
                return ApiResponse<List<GetRoleResponseDTO>>.Fail("Error occurred while retrieving roles.");
            }
        }
    }
}
