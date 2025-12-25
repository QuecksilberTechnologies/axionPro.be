using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
 
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetEmployeeImageQuery : IRequest<ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        public GetEmployeeImageRequestDTO DTO { get; }

        public GetEmployeeImageQuery(GetEmployeeImageRequestDTO dTO)
        {
            DTO = dTO;
        }
    }


    public class GetEmployeeImageQueryHandler : IRequestHandler<GetEmployeeImageQuery, ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetEmployeeImageQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeeImageQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetEmployeeImageQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetEmployeeImageReponseDTO>>> Handle(GetEmployeeImageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                  .ToString()?.Replace("Bearer ", "");

                request.DTO.Prop ??= new ExtraPropRequestDTO();


                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("User invalid.");
                }



                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO.Prop.UserEmployeeId =  _idEncoderService.DecodeId_long(UserEmpId, finalKey);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId_long(tokenClaims.TenantId, finalKey);
                string _Id= EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                request.DTO.Prop.EmployeeId = _idEncoderService.DecodeId_long(_Id, finalKey);
                
                
                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);

                // 🧩 STEP 4: Validate all employee references


                if (request.DTO.Prop.TenantId <= 0 || request.DTO.Prop.UserEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO.Prop.TenantId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         request.DTO.Prop.TenantId, loggedInEmpId
                    );
                }
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                  //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 10: Fetch Data from Repository
                var entityPaged = await _unitOfWork.Employees.GetImage(request.DTO);

                if (entityPaged == null || !entityPaged.Items.Any())
                {
                    _logger.LogInformation("No images found for EmployeeId: {EmpId}", request.DTO.Prop.EmployeeId);
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("No images found for the employee.");
                }

                // 🧩 STEP 8: Encrypt Ids and Map using ProjectionHelper (optimized)
                var resultList = ProjectionHelper.ToGetEmployeeImageResponseDTOs(entityPaged, _idEncoderService, tenantKey,  _config);

                // 🧩 STEP 9: Construct success response with pagination
                return ApiResponse<List<GetEmployeeImageReponseDTO>>.SuccessPaginatedPercentageMarkPrimary(
                    data: resultList,
                    message: "Base Employee info retrieved successfully.",
                    pageNumber: entityPaged.PageNumber,
                    pageSize: entityPaged.PageSize,
                    totalRecords: entityPaged.TotalCount,
                    totalPages: entityPaged.TotalPages,
                    completionPercentage: entityPaged.CompletionPercentage,
                    isPrimaryMarked: entityPaged.IsPrimaryMarked
                  
                     
                    
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error while fetching employee images.");
                return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}
