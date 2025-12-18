using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
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
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
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
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Decrypt Tenant and Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("User invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId_long(UserEmpId, finalKey);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId_long(tokenClaims.TenantId, finalKey);                        
                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);
                // 🧩 STEP 4: Validate all employee references

               

                if (request.DTO.Prop.UserEmployeeId <= 0 || request.DTO.Prop.TenantId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO.Prop.UserEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         request.DTO.Prop.UserEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");

                    //return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }
                
                var responseDTO = await _unitOfWork.DepartmentRepository.GetAsync(request.DTO);

                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogWarning("No active departments found for the given filter.");

                    return new ApiResponse<List<GetDepartmentResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No active departments found.",
                        Data = new List<GetDepartmentResponseDTO>()
                    };
                }
              //  var encryptedList = ProjectionHelper.ToGetDepartmentResponseDTOs(responseDTO.Items, _idEncoderService, finalKey);

                _logger.LogInformation("Successfully total record {Count} found active departments.", responseDTO.TotalCount);
                // 6️⃣ Return API response
                return new ApiResponse<List<GetDepartmentResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching active departments.");

                return new ApiResponse<List<GetDepartmentResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch departments due to an internal error.",
                    Data = new List<GetDepartmentResponseDTO>()
                };
            }
        }

    }
}
