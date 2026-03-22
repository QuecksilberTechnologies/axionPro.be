using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.Hash;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class RefreshTokenCommand : IRequest<ApiResponse<LoginResponseDTO>>
    {
        public RefreshTokenRequestDTO DTO { get; }

        public RefreshTokenCommand(RefreshTokenRequestDTO request)
        {
            DTO = request;
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;

        public RefreshTokenCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<RefreshTokenCommandHandler> logger,
            IConfiguration configuration,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService, IFileStorageService fileStorageService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
            _configuration = configuration;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<LoginResponseDTO>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request?.DTO == null || string.IsNullOrWhiteSpace(request.DTO.RefreshToken))
                {
                    return ApiResponse<LoginResponseDTO>.Fail("Refresh token is required.");
                }

                // =====================================================
                // STEP 1: Validate incoming refresh token
                // =====================================================
                var incomingHashedToken = HashHelper.Sha256(request.DTO.RefreshToken);

                var oldToken = await _refreshTokenRepository.GetValidByHashedTokenAsync(incomingHashedToken);

                if (oldToken == null)
                {
                    _logger.LogWarning("Invalid refresh token attempt. IP={IP}", request.DTO.IpAddress);
                    throw new UnauthorizedAccessException("Invalid refresh token.");
                }

                if (oldToken.IsRevoked)
                {
                    _logger.LogWarning("Refresh token reuse detected. LoginId={LoginId}, IP={IP}",
                        oldToken.LoginId, request.DTO.IpAddress);
                    throw new UnauthorizedAccessException("Refresh token revoked.");
                }

                if (oldToken.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogInformation("Expired refresh token used. LoginId={LoginId}, IP={IP}",
                        oldToken.LoginId, request.DTO.IpAddress);
                    throw new UnauthorizedAccessException("Refresh token expired.");
                }
                // =====================================================
                // STEP 2: Fresh loginId from token row
                // =====================================================
                string loginId = oldToken.LoginId;

                if (string.IsNullOrWhiteSpace(loginId))
                {
                    _logger.LogWarning("Refresh token has empty LoginId. TokenId={TokenId}", oldToken.Id);
                    throw new UnauthorizedAccessException("Invalid refresh token data.");
                }

                // =====================================================
                // STEP 3: Validate active user fresh from DB
                // =====================================================
                long empId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(loginId);

                _logger.LogInformation("Refresh validation for LoginId {LoginId}: EmployeeId = {empId}", loginId, empId);

                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed during refresh for LoginId: {LoginId}", loginId);
                    return ApiResponse<LoginResponseDTO>.Fail("User is not authenticated or authorized to perform this action.");
                }

                // =====================================================
                // STEP 4: Fresh employee info
                // =====================================================
                GetMinimalEmployeeResponseDTO empMinimalResponse =
                    await _unitOfWork.Employees.GetSingleRecordAsync(empId, true);

                TenantSubscriptionPlanRequestDTO dto = new TenantSubscriptionPlanRequestDTO();

                if (empMinimalResponse == null)
                {
                    _logger.LogWarning("Employee may not active or deleted during refresh. LoginId: {LoginId}", loginId);

                    return new ApiResponse<LoginResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Employee not active. Please contact admin."
                    };
                }

                // =====================================================
                // STEP 5: Fresh subscription validation
                // =====================================================
                dto.TenantId = empMinimalResponse.TenantId;

                var subscriptionInfo = await _unitOfWork.TenantSubscriptionRepository.GetValidateTenantPlan(dto);

                if (subscriptionInfo == null ||
                    !subscriptionInfo.SubscriptionEndDate.HasValue ||
                    subscriptionInfo.SubscriptionEndDate.Value.Date < DateTime.Today)
                {
                    _logger.LogWarning("Subscription expired or missing for tenant {TenantId} during refresh", dto.TenantId);

                    return new ApiResponse<LoginResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Your subscription has expired. Please contact admin to renew the plan."
                    };
                }

                // =====================================================
                // STEP 6: Fresh roles
                // =====================================================
                var userRoles = await _unitOfWork.UserRoleRepository
                    .GetEmployeeRolesWithDetailsByIdAsync(empId, empMinimalResponse.TenantId);

                if (userRoles == null || userRoles.Count == 0)
                {
                    _logger.LogWarning("No roles found during refresh for LoginId: {LoginId}", loginId);
                    return ApiResponse<LoginResponseDTO>.Fail("No active role found for this user.");
                }

                var roleInfo = userRoles.FirstOrDefault(x => x.IsPrimaryRole == true);

                if (roleInfo == null || roleInfo.Role == null)
                {
                    _logger.LogWarning("Primary role missing during refresh for LoginId: {LoginId}", loginId);
                    return ApiResponse<LoginResponseDTO>.Fail("Primary role not found for this user.");
                }

                List<UserRoleDTO>? userRoleDTOs = null;
                string? allRoleIdsCsv = null;
                UserRoleDTO? primaryRole = null;

                allRoleIdsCsv = userRoles
                    .Where(r => r.RoleId != null)
                    .Select(r => r.RoleId.ToString())
                    .Aggregate((a, b) => $"{a},{b}");

                if (!string.IsNullOrEmpty(allRoleIdsCsv))
                    _logger.LogInformation("Fetched Role IDs during refresh for LoginId {LoginId}: {Roles}", loginId, allRoleIdsCsv);
                else
                    _logger.LogWarning("No roles CSV formed during refresh for LoginId {LoginId}", loginId);

                userRoleDTOs = _mapper.Map<List<UserRoleDTO>>(userRoles);

                primaryRole = userRoleDTOs.FirstOrDefault(ur => ur.IsPrimaryRole && ur.IsActive);

                if (primaryRole != null)
                    userRoleDTOs.Remove(primaryRole);

                // =====================================================
                // STEP 7: Fresh common items
                // =====================================================
                var parent = await _unitOfWork.ModuleRepository.GetCommonMenuParentAsync();
                if (parent == null)
                {
                    return ApiResponse<LoginResponseDTO>.Fail("Common menu parent not found.");
                }

                List<ModuleDTO> CommonItems = await _unitOfWork.ModuleRepository.GetCommonMenuTreeAsync(parent.Id);

                // =====================================================
                // STEP 8: Fresh operational menus / permissions
                // =====================================================
                var requestDto = new GetActiveRoleModuleOperationsRequestDTO
                {
                    RoleIds = allRoleIdsCsv,
                    TenantId = empMinimalResponse.TenantId
                };

                var rolePermissions = await _unitOfWork.StoreProcedureRepository.GetActiveRoleModuleOperationsAsync(requestDto);

                var grouped = rolePermissions
                    .GroupBy(m => new { m.MainModuleId, m.MainModuleName })
                    .Select(main => new MainModuleDto
                    {
                        MainModuleId = main.Key.MainModuleId,
                        MainModuleName = main.Key.MainModuleName,

                        SubModules = main
                            .GroupBy(sm => new { sm.ParentModuleId, sm.SubModuleName })
                            .Select(sub => new SubModuleDto
                            {
                                SubModuleId = sub.Key.ParentModuleId,
                                SubModuleName = sub.Key.SubModuleName,

                                Modules = sub
                                    .GroupBy(mod => new
                                    {
                                        mod.ModuleId,
                                        mod.ModuleName,
                                        mod.DisplayName,
                                        mod.ImageIconWeb,
                                        mod.ImageIconMobile,
                                        mod.URLPath,
                                        mod.DataViewStructureId,
                                        mod.DisplayOn
                                    })
                                    .Select(mod => new ModuleDto
                                    {
                                        ModuleId = mod.Key.ModuleId,
                                        ModuleName = mod.Key.ModuleName,
                                        DisplayName = mod.Key.DisplayName,
                                        ImageIconWeb = mod.Key.ImageIconWeb,
                                        ImageIconMobile = mod.Key.ImageIconMobile,
                                        SubModuleURL = mod.Key.URLPath,
                                        DataViewStructureId = mod.Key.DataViewStructureId,
                                        DisplayOn = mod.Key.DisplayOn,

                                        Operations = mod
                                            .Select(op => new OperationDto
                                            {
                                                OperationId = op.OperationId,
                                                OperationName = op.OperationName
                                            }).ToList()
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                var TenantEnabledModulesWithOperationData =
                    await _unitOfWork.TenantModuleConfigurationRepository
                        .GetAllTenantEnabledModulesWithOperationsAsync(empMinimalResponse.TenantId);

                // =====================================================
                // STEP 9: Fresh encryption key
                // =====================================================
                long tempTenantId = empMinimalResponse?.TenantId ?? 0;
                long tempEmployeeId = empMinimalResponse?.Id ?? 0;

                var tenantEncryptionKey = await _unitOfWork.TenantEncryptionKeyRepository
                    .GetActiveKeyByTenantIdAsync(tempTenantId);

                if (tenantEncryptionKey == null || string.IsNullOrEmpty(tenantEncryptionKey.EncryptionKey))
                {
                    throw new Exception("Tenant encryption key not found or invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantEncryptionKey.EncryptionKey);
                string encriptedEmployeeId = _idEncoderService.EncodeId_long(tempEmployeeId, finalKey);
                string encriptedTenantId = _idEncoderService.EncodeId_long(tempTenantId, finalKey);

                // =====================================================
                // STEP 10: Fresh profile image + employee response
                // =====================================================
                                string? profileKey = await _unitOfWork.Employees.ProfileImage(empId);

                string? ProfileImagePath = null;

                if (!string.IsNullOrWhiteSpace(profileKey))
                {
                    ProfileImagePath = _fileStorageService.GetFileUrl(profileKey);
                }
                bool? isPasswordChange = null;

                var user = await _unitOfWork.UserLoginRepository.AuthenticateUser(loginId);
                if (user != null)
                {
                    isPasswordChange = user.IsPasswordChangeRequired;
                }

                GetEmployeeLoginInfoResponseDTO? employeeInfo =
                    _mapper.Map<GetEmployeeLoginInfoResponseDTO>(empMinimalResponse);

                employeeInfo.IsPasswordChangeRequired = isPasswordChange;
                employeeInfo.UserPrimaryRole = primaryRole;
                employeeInfo.RoleTypeId = roleInfo.Role.RoleType;
                employeeInfo.RoleTypeName = roleInfo.Role.RoleName;
                employeeInfo.EmployeeId = encriptedEmployeeId.Trim();
                employeeInfo.UserSecondryRoles = userRoleDTOs;
                employeeInfo.ProfileImageLink = ProfileImagePath;
                employeeInfo.IsOnboard = user.IsOnboard ;

                var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(dto.TenantId, true);

                GetRoleRequestDTO getRoleRequestDTO = new GetRoleRequestDTO
                {
                    Id = employeeInfo.UserPrimaryRole?.RoleId ?? 0,
                    RoleType = roleInfo.Role.RoleType,
                    IsActive = true,
                    Prop = new()
                    {
                        TenantId = dto.TenantId
                    }
                };

                var roleTypeList = await _unitOfWork.RoleRepository.GetAsync(getRoleRequestDTO);
                var roleType = roleTypeList.Items.FirstOrDefault(r =>
                    r.Id == employeeInfo.UserPrimaryRole.RoleId && r.IsActive == true);

                if (dto.TenantId == 0)
                {
                    return new ApiResponse<LoginResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant information is invalid. Please contact admin."
                    };
                }

                employeeInfo.TenantName = tenant?.CompanyName ?? string.Empty;

                // =====================================================
                // STEP 11: Fresh token info DTO
                // =====================================================
                GetTokenInfoDTO getTokenInfoDTO = new GetTokenInfoDTO()
                {
                    TenantEncriptionKey = finalKey,
                    TenantId = encriptedTenantId,
                    UserId = loginId,
                    EmployeeId = encriptedEmployeeId.Trim(),
                    RoleId = employeeInfo.UserPrimaryRole.RoleId.ToString(),
                    RoleTypeId = employeeInfo.RoleTypeId.ToString() ?? "0",
                    RoleTypeName = employeeInfo.RoleTypeName ?? "",
                    EmployeeTypeId = employeeInfo.EmployeeTypeId.ToString() ?? "0",
                    GenderId = empMinimalResponse.GenderId.ToString(),
                    GenderName = empMinimalResponse.GenderName,
                    Email = loginId,
                    FullName = ((empMinimalResponse.FirstName ?? "") + "-" + (empMinimalResponse.LastName ?? "")).Trim('-'),
                    Expiry = DateTime.UtcNow.AddMinutes(15),
                    TokenPurpose = ConstantValues.Auth.ToString(),
                };

                // =====================================================
                // STEP 12: Generate new token pair
                // =====================================================
                var token = await _tokenService.GenerateToken(getTokenInfoDTO);

                var newRefreshToken = await _tokenService.GenerateRefreshToken();
                var newHashedRefreshToken = HashHelper.Sha256(newRefreshToken);

                // =====================================================
                // STEP 13: Rotate refresh token in transaction
                // =====================================================
                await _unitOfWork.BeginTransactionAsync();

                await _refreshTokenRepository.UpdateReplacedByTokenAsync(oldToken.Id, newHashedRefreshToken);
                await _refreshTokenRepository.RevokeAsync(oldToken.Id, request.DTO.IpAddress);

                bool isInserted = await _refreshTokenRepository.InsertAsync(new RefreshToken
                {
                    LoginId = loginId,
                    Token = newHashedRefreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    CreatedByIp = request.DTO.IpAddress,
                    IsRevoked = false
                });

                if (!isInserted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<LoginResponseDTO>.Fail("Unable to issue new refresh token.");
                }

                await _unitOfWork.CommitTransactionAsync();

                // =====================================================
                // STEP 14: Full login-style response
                // =====================================================
                var loginResponse = new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = newRefreshToken,
                    Success = ConstantValues.isSucceeded,
                    EmployeeInfo = employeeInfo,
                    CommonItems = CommonItems,
                    OperationalMenus = grouped,
                    Allroles = allRoleIdsCsv?.Trim()
                };

                return ApiResponse<LoginResponseDTO>.Success(loginResponse, "Token refreshed successfully.");
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch
                {
                }

                _logger.LogError(ex, "Error while refreshing token.");                          
                throw;
            }
        }
    }
}