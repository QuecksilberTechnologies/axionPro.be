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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class GetDesignationQuery : IRequest<ApiResponse<List<GetDesignationResponseDTO>>>
    {
        public GetDesignationRequestDTO DTO { get; set; }

        public GetDesignationQuery(GetDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// ✅ Ideal handler for fetching all designations with JWT + Tenant + Employee validation
    /// </summary>
    public class GetDesignationQueryHandler : IRequestHandler<GetDesignationQuery, ApiResponse<List<GetDesignationResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDesignationQueryHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public GetDesignationQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetDesignationQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetDesignationResponseDTO>>> Handle(GetDesignationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Decrypt Tenant and Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                request.DTO.Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                int id = SafeParser.TryParseInt(request.DTO.Id);
                request.DTO.DepartmentId = EncryptionSanitizer.CleanEncodedInput(request.DTO.DepartmentId);
                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );
                }
                    var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data
                var responseDTO = await _unitOfWork.DesignationRepository.GetAsync(request.DTO, decryptedTenantId, id);


                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("⚠️ No designation found for TenantId: {TenantId}", decryptedTenantId);
                    return new ApiResponse<List<GetDesignationResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No roles found.",
                        Data = new List<GetDesignationResponseDTO>(),
                        PageNumber = request.DTO.PageNumber,
                        PageSize = request.DTO.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0
                    };
                }

                //  var encryptedList = ProjectionHelper.ToGetRoleResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);


                // 🧩 STEP 7: Success response
                _logger.LogInformation("✅ {Count} designation retrieved successfully for TenantId: {TenantId}",
                    responseDTO.TotalCount, decryptedTenantId);

                return new ApiResponse<List<GetDesignationResponseDTO>>
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
                _logger.LogError(ex, "❌ Error occurred while fetching designations: {Message}", ex.Message);

                return ApiResponse<List<GetDesignationResponseDTO>>.Fail("An error occurred while fetching designations.");
            }
        }
    }
}
