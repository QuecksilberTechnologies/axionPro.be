using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
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
    public class CreateDepartmentCommand : IRequest<ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        public CreateDepartmentRequestDTO DTO { get; set; }

        public CreateDepartmentCommand(CreateDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateDepartmentCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateDepartmentCommandHandler> logger,
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

        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
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
                request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                


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
                }
                // 🧩 STEP 4: Call Repository to get data


                // 🧩 STEP 5: Validate Department Name
                string? departmentName = request.DTO.DepartmentName?.Trim();
             
                if (string.IsNullOrWhiteSpace(departmentName))
                {
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Department name should not be empty or whitespace.");
                }

                // 🧩 STEP 6: Create Department
                var responseDTO = await _unitOfWork.DepartmentRepository.CreateAsync(request.DTO);

                // 🧩 STEP 7: Validate Response
                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogWarning("❌ Department creation failed or empty result. TenantId: {TenantId}", request.DTO.Prop.UserEmployeeId);
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("No department was created. Please try again.");
                }

                // 🧩 STEP 8: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ STEP 9: Return Success
                return new ApiResponse<List<GetDepartmentResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} department(s) created successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating department(s).");
                return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Failed to create department(s) due to an internal error.");
            }
        }
    }
}
