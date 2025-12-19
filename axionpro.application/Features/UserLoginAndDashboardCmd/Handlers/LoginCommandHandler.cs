using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
 
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
 
using axionpro.domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
 
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class LoginCommand : IRequest<ApiResponse<LoginResponseDTO>>
    {
        public LoginRequestDTO DTO { get; set; }


        public LoginCommand(LoginRequestDTO loginRequestDTO)
        {
            DTO = loginRequestDTO;
        }



    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICommonRepository _iCommonRepository;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;
        private readonly  IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public LoginCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, ILogger<LoginCommandHandler> logger, ICommonRepository iCommonRepository, IPasswordService passwordService,
            IConfiguration configuration, IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _iCommonRepository = iCommonRepository;
            _passwordService = passwordService;
            _configuration = configuration;
            _encryptionService = encryptionService; // 👈 same name use karo
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
     
        }
        public async Task<ApiResponse<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
              
                if (string.IsNullOrEmpty(request.DTO.LoginId) && string.IsNullOrEmpty(request.DTO.Password))
                {
                    _logger.LogWarning(" LoginId and Password is not null or empty.");
                    return ApiResponse<LoginResponseDTO>.Fail("LoginId and Password cann't be empty");

                }
                string? savedFullPath = null;  // 📂 File full path track karne ke liye

                // 🔐 Step 1: Validate if user exists
                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(request.DTO.LoginId);
                _logger.LogInformation("Validation result for LoginId {LoginId}: EmployeeId = {empId}", request.DTO.LoginId, empId);

                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for LoginId: {LoginId}", request.DTO.LoginId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<LoginResponseDTO>.Fail("User is not authenticated or authorized to perform this action.");
                }
                             


                var loginRequest = new LoginRequestDTO
                {
                    LoginId = request.DTO.LoginId,
                    Password = request.DTO.Password,
                    IpAddressLocal = request.DTO.IpAddressLocal,
                    IpAddressPublic = request.DTO.IpAddressPublic,
                    LoginDevice = request.DTO.LoginDevice,
                    MacAddress = request.DTO.MacAddress,
                    Latitude = request.DTO.Latitude,
                    Longitude = request.DTO.Longitude,  
                };
               
                // 🔐 Authenticate user
                var user = await _unitOfWork.UserLoginRepository.AuthenticateUser(loginRequest.LoginId);

                if (user == null || string.IsNullOrWhiteSpace(user.Password))
                {
                    return ApiResponse<LoginResponseDTO>.Fail(ConstantValues.invalidCredential);
                }

                // 🔑 Verify password
                if (!_passwordService.VerifyPassword(user.Password, loginRequest.Password))
                {
                    return ApiResponse<LoginResponseDTO>.Fail(ConstantValues.invalidCredential);
                }
                
                //if (!passwordMatch)
                //{
                //    _logger.LogWarning("🚫 Login failed: Incorrect password for LoginId: {LoginId}", loginRequest.LoginId);
                //    return null;
                //}



                _logger.LogInformation("✅ User authenticated successfully for LoginId: {LoginId}", loginRequest.LoginId);

                bool? IsPasswordChange = user.IsPasswordChangeRequired;


                // 👨‍💼 Step 5: Fetch Employee Info
                // var employee = await _unitOfWork.Employees.GetEmployeeInfoForLoginByIdAsync(empId);
                GetMinimalEmployeeResponseDTO empMinimalResponse = await _unitOfWork.Employees.GetSingleRecordAsync(empId,true);
                TenantSubscriptionPlanRequestDTO dto = new TenantSubscriptionPlanRequestDTO();
                


                if (empMinimalResponse == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("Employee may not active or deleted, please contact admin. LoginId: {LoginId}", loginRequest.LoginId);

                    return new ApiResponse<LoginResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Employee not active. Please contact admin."
                    };
                }

                // Null check with fallback to 0
                dto.TenantId = empMinimalResponse.TenantId;
                var subscriptionInfo = await _unitOfWork.TenantSubscriptionRepository
                     .GetValidateTenantPlan(dto);

                // ✅ Check if subscription info exists and EndDate > today
                if (subscriptionInfo == null ||
                             !subscriptionInfo.SubscriptionEndDate.HasValue ||
                             subscriptionInfo.SubscriptionEndDate.Value.Date < DateTime.Today)
                             {
                               await _unitOfWork.RollbackTransactionAsync();

                                  _logger.LogWarning("Subscription expired or missing for tenant {TenantId}", dto.TenantId);

                            return new ApiResponse<LoginResponseDTO>
                              {
                                 IsSucceeded = false,
                                  Message = "Your subscription has expired. Please contact admin to renew the plan."
                         };
                }

                // Get Active true and Isdeleted false employee

                // 👥 Step 6: Fetch all roles
                var userRoles = await _unitOfWork.UserRoleRepository.GetEmployeeRolesWithDetailsByIdAsync(empId, empMinimalResponse.TenantId);
                var roleInfo = userRoles.FirstOrDefault(x => x.IsPrimaryRole == true);


                //bool isAdmin = userRoles.Any(ur => ur.Role.RoleType == ConstantValues.TenantAdminRoleType &&   ur.Role.IsSystemDefault == ConstantValues.IsByDefaultFalse &&
                // ur.Role.RoleCode.Equals(ConstantValues.TenantAdminRoleCode, StringComparison.OrdinalIgnoreCase) &&
                // ur.Role.IsActive == true &&
                // ur.Role.IsSoftDeleted == false);

                bool isAdmin = true;
                if (isAdmin)
                  {
                      UpdateTenantEnabledOperationFromModuleOperationRequestDTO updateTenantEnabledOperationFromModuleOperationRequestDTO = new UpdateTenantEnabledOperationFromModuleOperationRequestDTO();
                      updateTenantEnabledOperationFromModuleOperationRequestDTO.TenantId = empMinimalResponse.TenantId;
                      var updatedDone = _unitOfWork.CommonRepository.UpdateTenantEnabledOperationFromModuleOperationRequestDTO(updateTenantEnabledOperationFromModuleOperationRequestDTO);
                  }
                List<UserRoleDTO>? userRoleDTOs = null;
                string? allRoleIdsCsv = null;
                UserRoleDTO? primaryRole = null;
                if (userRoles.Count !=0)
                {
                     allRoleIdsCsv = userRoles?
                    .Where(r => r.RoleId != null)
                    .Select(r => r.RoleId.ToString())
                    .Aggregate((a, b) => $"{a},{b}");

                    if (!string.IsNullOrEmpty(allRoleIdsCsv))
                        _logger.LogInformation("Fetched Role IDs for LoginId {LoginId}: {Roles}", loginRequest.LoginId, allRoleIdsCsv);
                    else
                        _logger.LogWarning("No roles found for LoginId {LoginId}", loginRequest.LoginId);


                    // 🧠 Step 7: Map roles to DTOs
                      userRoleDTOs = _mapper.Map<List<UserRoleDTO>>(userRoles);
                    // ✅ Find & separate primary role
                        primaryRole = userRoleDTOs.FirstOrDefault(ur => ur.IsPrimaryRole && ur.IsActive);

                    if (primaryRole != null)
                        userRoleDTOs.Remove(primaryRole); // Remove primary from list

                    // 🎯 Extract secondary role IDs
                    List<int> secondaryRoleIds = userRoleDTOs?.Where(ur => ur.RoleId > 0).Select(ur => ur.RoleId).Distinct().ToList()?? new List<int>();


                    string secondaryRolesCsv = secondaryRoleIds.Any()
                        ? string.Join(",", secondaryRoleIds)
                        : "NULL";
                    // 🧾 Step 8: Map Employee Info
                 
                }

                
              //  employeeInfo.EmployeeTypeId = empInfo.EmployeeTypeId;


                // Getting Tenant Enabled module list


                // 🧩 Step 9: Load Common Itaxionpro
                //  var commonItaxionpro = await _unitOfWork.CommonRepository.GetCommonItemAsync();

                // ✅ Step 1: Get CommonMenuParent first (wait until it completes)
                var parent = await _unitOfWork.ModuleRepository.GetCommonMenuParentAsync();
                if (parent == null) return null;

                // ✅ Step 2: ONLY AFTER FIRST IS DONE, call second method
                List<ModuleDTO> CommonItems = await _unitOfWork.ModuleRepository.GetCommonMenuTreeAsync(parent.Id);

                // ✅ Step 3: THEN call RolePermissions API
                var requestDto = new GetActiveRoleModuleOperationsRequestDTO
                {
                    RoleIds = allRoleIdsCsv,
                    TenantId = empMinimalResponse.TenantId
                };


                


                var rolePermissions = await _unitOfWork.CommonRepository.GetActiveRoleModuleOperationsAsync(requestDto);




                var grouped = rolePermissions
             .GroupBy(m => new { m.MainModuleId, m.MainModuleName }) // 🔷 Group by Main Module
             .Select(main => new MainModuleDto
             {
                 MainModuleId = main.Key.MainModuleId,
                 MainModuleName = main.Key.MainModuleName,

                 SubModules = main
                     .GroupBy(sm => new { sm.ParentModuleId, sm.SubModuleName }) // 🔷 Sub Module Group
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
                                                       // ✅ Newly Added
                                 mod.URLPath,                  // ✅ From ModuleOperationMapping
                                                 // ✅ From ModuleOperationMapping
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
                                 SubModuleURL = mod.Key.URLPath,              // ✅
                                                       // ✅
                                                  
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
                var TenantEnabledModulesWithOperationData = await _unitOfWork.TenantModuleConfigurationRepository.GetAllTenantEnabledModulesWithOperationsAsync(empMinimalResponse.TenantId);
                // ✅ Step 1: Safe TenantId extraction
                
                long tempTenantId = empMinimalResponse?.TenantId ?? 0;
                long tempEmployeeId = empMinimalResponse?.Id ?? 0;
                string convertedTenantId = tempTenantId.ToString();

                // ✅ Step 2: Get encryption key
                var tenantEncryptionKey = await _unitOfWork.TenantEncryptionKeyRepository
                    .GetActiveKeyByTenantIdAsync(tempTenantId);


                if (tenantEncryptionKey == null || string.IsNullOrEmpty(tenantEncryptionKey.EncryptionKey))
                {
                    throw new Exception("Tenant encryption key not found or invalid.");

                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantEncryptionKey.EncryptionKey);
                string encriptedEmployeeId = _idEncoderService.EncodeId_long(tempEmployeeId, finalKey);
                string encriptedTenantId = _idEncoderService.EncodeId_long(tempTenantId, finalKey);
                // ✅ Decrypt same encrypted string             
          
                string? ProfileImagePath = $"{_configuration["FileSettings:BaseUrl"] ?? string.Empty}{await _unitOfWork.Employees.ProfileImage(empId) ?? null}";

                //string Decrut = (_encryptionService.Decrypt(tempEmployeeId.ToString(), tenantEncryptionKey.EncryptionKey));

                GetEmployeeLoginInfoResponseDTO? employeeInfo = _mapper.Map<GetEmployeeLoginInfoResponseDTO>(empMinimalResponse);
                employeeInfo.IsPasswordChangeRequired = IsPasswordChange;
                employeeInfo.UserPrimaryRole = primaryRole;
                employeeInfo.RoleTypeId = roleInfo.Role.RoleType;
                employeeInfo.RoleTypeName = roleInfo.Role.RoleName;
                employeeInfo.EmployeeId = encriptedEmployeeId.Trim();
                employeeInfo.UserSecondryRoles = userRoleDTOs;
                employeeInfo.ProfileImageLink = ProfileImagePath;
                var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(dto.TenantId, true );
                GetRoleRequestDTO getRoleRequestDTO = new GetRoleRequestDTO
                {
                    Id = employeeInfo.UserPrimaryRole?.RoleId ?? 0,
                    RoleType = roleInfo.Role.RoleType,
                    IsActive = true,

                    Prop = new ()
                    {
                        TenantId = dto.TenantId
                    }

                };
                 
                // ✅ Get role list (filtered by roleId)
                var roleTypeList = await _unitOfWork.RoleRepository.GetAsync(getRoleRequestDTO);

                // ✅ Select specific role
                var roleType = roleTypeList.Items.FirstOrDefault(r => r.Id == employeeInfo.UserPrimaryRole.RoleId && r.IsActive == true);
                 
                // ✅ Get tenant info (await lagana mat bhoolna)

                // ✅ Assign tenant name

                if(dto.TenantId==0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<LoginResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant information is invalid. Please contact admin."
                    };
                }




                // ✅ Step 4: Prepare employee info 
                employeeInfo.TenantName = tenant?.CompanyName ?? string.Empty;

                    
                // ✅ Step 5: Prepare token info DTO
                GetTokenInfoDTO getTokenInfoDTO = new GetTokenInfoDTO()
                {
                     TenantEncriptionKey = finalKey,
                    TenantId = encriptedTenantId,
                    UserId = request.DTO.LoginId,
                    EmployeeId = encriptedEmployeeId.Trim().ToString(), // long
                    RoleId = employeeInfo.UserPrimaryRole.RoleId.ToString(), // long
                    RoleTypeId = employeeInfo.RoleTypeId.ToString() ?? "0",
                    RoleTypeName = employeeInfo.RoleTypeName ?? "",
                    EmployeeTypeId =  employeeInfo.EmployeeTypeId.ToString()??"0",
                    GenderId = empMinimalResponse.GenderId.ToString(),
                    GenderName = empMinimalResponse.GenderName,                  
                    Email = request.DTO.LoginId ,
                    FullName = empMinimalResponse.FirstName + ("-" + empMinimalResponse.LastName) ?? " Unknown",
                    Expiry = DateTime.UtcNow.AddMinutes(15),
                    TokenPurpose = ConstantValues.Auth.ToString(),
                    
                   
                };
            


               // 🔐 Step 3: Generate tokens
                 var token = await _tokenService.GenerateToken(getTokenInfoDTO);
                 var refreshToken = await _tokenService.GenerateRefreshToken();
                 await _refreshTokenRepository.SaveOrUpdateRefreshToken(
                    loginRequest.LoginId.ToString(),
                    token,
                    ConstantValues.ExpireTokenDate,
                    ConstantValues.IP
                );


                // 🔄 Step 4: Update login audit
                bool updated = await _unitOfWork.CommonRepository.UpdateLoginCredential(loginRequest);
                if (updated)
                    _logger.LogInformation("LoginCredential updated successfully for LoginId: {LoginId}", loginRequest.LoginId);
                else
                    _logger.LogWarning("Failed to update LoginCredential for LoginId: {LoginId}", loginRequest.LoginId);

                // 🔐 Step 10: Fetch Permissions

                //  var permissionList = new List<List<RoleModulePermission>> { rolePermissions };

                // 🚀 Step 11: Final Response Object
                var loginResponse = new LoginResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Success = ConstantValues.isSucceeded,
                    EmployeeInfo = employeeInfo,
                    CommonItems = CommonItems,
                    OperationalMenus = grouped,
                    Allroles = allRoleIdsCsv?.Trim()
                };


            

                // ✅ Final Return
                return ApiResponse<LoginResponseDTO>.Success(loginResponse, "Login successful.");

                

              //  return new ApiResponse<LoginResponseDTO>(loginResponse, ConstantValues.successMessage, ConstantValues.isSucceeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LoginCommandHandler.Handle method.");

                await _unitOfWork.RollbackTransactionAsync();  // ✅ Rollback Transaction in case of error

                return new ApiResponse<LoginResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing the login request. Please try again later.",
                    Data = null
                };
            }
        }



    }

}
