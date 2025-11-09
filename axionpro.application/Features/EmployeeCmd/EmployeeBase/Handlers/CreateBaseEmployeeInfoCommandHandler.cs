using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class CreateBaseEmployeeInfoCommand : IRequest<ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        public CreateBaseEmployeeRequestDTO DTO { get; set; }

        public CreateBaseEmployeeInfoCommand(CreateBaseEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateBaseEmployeeInfoCommandHandler : IRequestHandler<CreateBaseEmployeeInfoCommand, ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateBaseEmployeeInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public CreateBaseEmployeeInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateBaseEmployeeInfoCommandHandler> logger,
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
            _encryptionService = encryptionService ;
            _idEncoderService = idEncoderService ;
        }

        public async Task<ApiResponse<List<GetBaseEmployeeResponseDTO>>> Handle(CreateBaseEmployeeInfoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Decrypt Tenant and Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("User invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);               
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                int designationId = SafeParser.TryParseInt(request.DTO.DesignationId);
                int departmentId = SafeParser.TryParseInt(request.DTO.DepartmentId);
                int genderId = SafeParser.TryParseInt(request.DTO.GenderId);                
                int refrenceId = SafeParser.TryParseInt(request.DTO.ReferalId);
                int employeeTypeId = SafeParser.TryParseInt(request.DTO.EmployeeTypeId);
                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var timePart = DateTime.UtcNow.ToString("HHmmss");
                var randomSuffix = Path.GetRandomFileName().Replace(".", "").Substring(0, 4).ToUpper();

                // ✅ Step: Validate Reference IDs before proceeding
                if (designationId <= 0 || departmentId <= 0 || genderId <= 0 
                   || employeeTypeId <= 0)
                       {
                    _logger.LogWarning(
                        "❌ One or more reference fields missing: DesignationId={DesignationId}, DepartmentId={DepartmentId}, GenderId={GenderId}" +
                        ", ReferalId={ReferalId}, EmployeeTypeId={EmployeeTypeId}",
                        designationId, departmentId, genderId, refrenceId, employeeTypeId


                      );

                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("One or more reference fields are missing or invalid.");
                }

                // 🧩 STEP 4: Validate all employee references


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data

                // 3️⃣ DTO Configuration
                var entity = _mapper.Map<Employee>(request.DTO);
                // 🧩 STEP 5: Entity Mapping (join fields + base fields)

                entity.AddedById = decryptedEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsInfoVerified = false;
                entity.IsEditAllowed = true;
                entity.InfoVerifiedById = null;
                entity.TenantId = decryptedTenantId;
                entity.DesignationId = designationId;
                entity.DepartmentId = departmentId;
                entity.ReferalId = refrenceId;
                entity.EmployeeTypeId = employeeTypeId;
                entity.GenderId = genderId;
                entity.EmployementCode = $"{decryptedTenantId}/{datePart}/{timePart}-{randomSuffix}";
              //   HttpRequestOptionsKey

                // 4️⃣ Repository Operation
                var responseDTO = await _unitOfWork.Employees.CreateAsync(entity);
                 
                // 5️⃣ Encrypt Result Data
                 var encryptedList = ProjectionHelper.ToGetBaseInfoResponseDTOs(responseDTO.Items, _idEncoderService, tenantKey);

                // 6️⃣ Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 7️⃣ Final API Response
                return new ApiResponse<List<GetBaseEmployeeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = encryptedList,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while adding base employee info");
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Failed to add base employee info.", new List<string> { ex.Message });
            }
        }
    }
}
