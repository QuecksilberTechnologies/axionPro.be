using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{


    public class UpdateSectionBulkCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeSectionStatusRequestDTO DTO { get; set; }

        public UpdateSectionBulkCommand(UpdateEmployeeSectionStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateSectionBulkCommandHandler
        : IRequestHandler<UpdateSectionBulkCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateSectionBulkCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public UpdateSectionBulkCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateSectionBulkCommandHandler> logger,
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


        public async Task<ApiResponse<bool>> Handle(UpdateSectionBulkCommand request, CancellationToken ct)
        {
            try
            { 
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



                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<bool>.Fail("User invalid.");
                }

                // ------------------------------------
                // STEP 2: BASIC DTO VALIDATION
                // ------------------------------------
                if (request.DTO == null)
                    return ApiResponse<bool>.Fail("Invalid payload.");

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId))
                    return ApiResponse<bool>.Fail("UserEmployeeId missing.");

                if (request.DTO.Sections == null || request.DTO.Sections.Count == 0)
                    return ApiResponse<bool>.Fail("No sections provided.");

                // ------------------------------------
                // STEP 3: DECODE MAIN USER EMPLOYEE ID
                // ------------------------------------
                string cleanedUserId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long employeeId = _idEncoderService.DecodeId(cleanedUserId, tenantKey);

                if (employeeId != loggedInEmpId)
                    return ApiResponse<bool>.Fail("User mismatch.");
                

                // ------------------------------------
                // STEP 4: LOOP ALL SECTIONS
                // ------------------------------------
                foreach (var section in request.DTO.Sections)
                {
                    // Validate section fields
                    if (string.IsNullOrWhiteSpace(section.SectionName))
                        return ApiResponse<bool>.Fail("Section name missing.");

                    if (string.IsNullOrWhiteSpace(section.EmployeeId))
                        return ApiResponse<bool>.Fail("Section EmployeeId missing.");

                    // ------------------------------------
                    // DECODE SECTION EMPLOYEE ID (YOUR STYLE)
                    // ------------------------------------
                    string cleanedSectionId = EncryptionSanitizer.CleanEncodedInput(section.EmployeeId);
                    long decodedEmployeeId = _idEncoderService.DecodeId(cleanedSectionId, tenantKey);

                    if (decodedEmployeeId <= 0)
                        return ApiResponse<bool>.Fail("Invalid EmployeeId in section.");


                    // ------------------------------------
                    // UPDATE VIA UOW (YOUR SAME CALL STYLE)
                    // ------------------------------------
                    bool isUpdated = await _unitOfWork.Employees.UpdateVerifyEditStatusAsync(
                        section.SectionName?.Trim().ToLower(),
                        decodedEmployeeId,
                        section.IsVerified,
                        section.IsEditAllowed,
                        true,                    // isActive default
                        loggedInEmpId          // admin who verified
                    );

                    if (!isUpdated)
                    {
                        _logger.LogWarning(
                            "Update failed → Section={Sec}, EmpId={EmpId}",
                            section.SectionName,
                            decodedEmployeeId
                        );
                    }
                }

                return ApiResponse<bool>.Success(true, "Section update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update error");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }

    }
}
