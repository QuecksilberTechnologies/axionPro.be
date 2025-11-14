using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination; 
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetAllEmployeeInfoQuery : IRequest<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        public GetAllEmployeeInfoRequestDTO DTO { get; }

        public GetAllEmployeeInfoQuery(GetAllEmployeeInfoRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetAllEmployeeInfoQueryHandler : IRequestHandler<GetAllEmployeeInfoQuery, ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetAllEmployeeInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public GetAllEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetAllEmployeeInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>> Handle(GetAllEmployeeInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token

                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("User invalid.");
                }



                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO._UserEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                request.DTO.Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
                request.DTO.Id_long = _idEncoderService.DecodeId(request.DTO.Id, finalKey);


                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || request.DTO._UserEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO._UserEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         request.DTO._UserEmployeeId, loggedInEmpId
                    );
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data
                // 🧩 STEP 4: Call Repository to get data
                PagedResponseDTO <GetAllEmployeeInfoResponseDTO> responseDTO = await _unitOfWork.Employees.GetAllInfo(request.DTO, decryptedTenantId);
                if (responseDTO == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("No Base Employee info found for EmployeeId: {EmpId}", request.DTO._UserEmployeeId );
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("No Base Employee info found.");
                }

                var resultList = ProjectionHelper.ToGetAllEmployeeInfoResponseDTOs(responseDTO, _idEncoderService, tenantKey);

                return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.SuccessPaginatedPercentage(
                    data: resultList,
                    message: "Base Employee info retrieved successfully.",
                    pageNumber: responseDTO.PageNumber,
                    pageSize: responseDTO.PageSize,
                    totalRecords: responseDTO.TotalCount,
                    totalPages: responseDTO.TotalPages,
                    completionPercentage: responseDTO.CompletionPercentage,
                    hasUploadedAll: responseDTO.HasUploadedAll
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error while fetching Base Employee info for EmployeeId: {EmployeeId}", request.DTO?.UserEmployeeId);
                return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}

