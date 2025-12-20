using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.SeedData;
using axionpro.application.Constants;
using axionpro.application.Constants.axionpro.application.Constants;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using axionpro.application.Features.RegistrationCmd.Commands;
using axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;


namespace axionpro.application.Features.RegistrationCmd.Handlers
{
    public class CreateTenantCommand : IRequest<ApiResponse<TenantCreateResponseDTO>>
    {
        public TenantCreateRequestDTO TenantCreateRequestDTO { get; set; }

        public CreateTenantCommand(TenantCreateRequestDTO createRequestDTO)
        {
            TenantCreateRequestDTO = createRequestDTO;
        }

    }
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ApiResponse<TenantCreateResponseDTO>>
    {
        private readonly IEmailService _emailService;
        private readonly ICommonRepository _commonRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTenantCommandHandler> _logger;    
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateTenantCommandHandler> logger, IEmailService emailService, ICommonRepository commonRepository,
            IPasswordService passwordService, IEncryptionService encryptionService, IIdEncoderService idEncoderService  
          , IConfiguration configuration, IServiceScopeFactory scopeFactory  )

        {
            _encryptionService = encryptionService;
            _emailService = emailService;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _commonRepository = commonRepository;
             _passwordService = passwordService;
            _idEncoderService = idEncoderService;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }
        public async Task<ApiResponse<TenantCreateResponseDTO>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1️⃣: Check if Tenant already exists by email

                bool isEmailExists = await _unitOfWork.TenantRepository.CheckTenantByEmail(request.TenantCreateRequestDTO.TenantEmail);
                if (isEmailExists)
                {
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant with this email already exists.",
                        Data = null
                    };
                }

                // Inside Handle() method
                string? hashedPassword = null;
                // Step 3️⃣: Map DTO to Entity
                var tenantEntity = _mapper.Map<Tenant>(request.TenantCreateRequestDTO);

                // Step 4️⃣: Begin Transaction
                await _unitOfWork.BeginTransactionAsync();

                // Step 5️⃣: Add Tenant
                long newTenantId = await _unitOfWork.TenantRepository.AddTenantAsync(tenantEntity);

                if (newTenantId <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant creation failed.",
                        Data = null
                    };
                }

              
                // Step 9️⃣: Save Subscription
                TenantSubscription savedSub = await _unitOfWork.TenantSubscriptionRepository.AddTenantSubscriptionAsync(new TenantSubscription
                {
                    TenantId = newTenantId,
                    SubscriptionPlanId = request.TenantCreateRequestDTO.SubscriptionPlanId,
                    SubscriptionStartDate = DateTime.Now,
                    SubscriptionEndDate = DateTime.Now.AddDays(30),
                    IsActive = true,
                    IsTrial = true
                });

                if (savedSub == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant Subscription failed.",
                        Data = null
                    };
                }

                // Step 🔟: Fetch Subscription Plan Details
                var tenantSubscriptionPlan = await _unitOfWork.TenantSubscriptionRepository.GetTenantSubscriptionPlanInfoAsync(
                    new TenantSubscriptionPlanRequestDTO
                    {
                        TenantId = newTenantId,
                        SubscriptionPlanId = request.TenantCreateRequestDTO.SubscriptionPlanId,
                        IsTrial = true,
                        IsActive = true
                    });

                if (tenantSubscriptionPlan == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant Subscription plan details not found.",
                        Data = null
                    };
                }

                // Step 11️⃣: Get All Subscribed Modules for this Tenant
                List<domain.Entity.Module> subscriptionPlans =
                    await _unitOfWork.PlanModuleMappingRepository.GetAllSubscribedModuleAsync(request.TenantCreateRequestDTO.SubscriptionPlanId);

                // ✅ Filter leaf node and header modules
                var leafNodeModules = subscriptionPlans.Where(m => m.IsLeafNode == true).ToList();
                var HeaderModules = subscriptionPlans.Where(m => m.IsLeafNode == false && m.ParentModule==null).ToList();

                // Step 12️⃣: Map Header Modules into TenantEnabledModules
                List<TenantEnabledModule> TenantEnabledModules = HeaderModules.Select(m => new TenantEnabledModule
                {
                    TenantId = newTenantId,
                    ModuleId = m.Id,
                    IsEnabled = true,
                    ParentModuleId = m.ParentModuleId,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.Now
                }).ToList();

                 //_unitOfWork.TenantModuleConfigurationRepository.CreateByDefaultEnabledModulesAsync(TenantEnabledModules);

                // Step 13️⃣: Get all module operations for Tenant
                List<ModuleOperationMapping> allModuleOperations = await _unitOfWork.ModuleOperationMappingRepository
                    .GetModuleOperationMappings(leafNodeModules);

                List<TenantEnabledOperationRequestDTO> tenantEnabledOperationsTemp =
                    _mapper.Map<List<TenantEnabledOperationRequestDTO>>(allModuleOperations);
                List<TenantEnabledOperation> tenantEnabledOperations =
                    _mapper.Map<List<TenantEnabledOperation>>(tenantEnabledOperationsTemp);

                // Step 14️⃣: Create Default Enabled Modules and Operations
                await _unitOfWork.TenantModuleConfigurationRepository.CreateByDefaultEnabledModulesAsync(
                    newTenantId, TenantEnabledModules, tenantEnabledOperations);

                // Step 15️⃣: Get Subscribed Modules for Department Creation
                List<SubscribedModuleResponseDTO> getDepartnames =
                    _unitOfWork.CommonRepository.GetSubscribedModulesByTenantAsync(newTenantId).Result;

                if (getDepartnames == null || getDepartnames.Count == 0)
                {
                    _logger.LogWarning("No subscribed modules found for TenantId: {TenantId}", newTenantId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while adding department.",
                        Data = null
                    };
                }

                   //string convertedTenantId= newTenantId.ToString();
                   string EncriptedTenantKey = _encryptionService.GenerateKey();

                TenantEncryptionKey tenantEncryptionKey = new TenantEncryptionKey()
                {
                    IsActive = true,
                    TenantId = newTenantId,
                    EncryptionKey = EncriptedTenantKey,

                };
                   var  insertedRecord = _unitOfWork.TenantEncryptionKeyRepository.AddAsync(tenantEncryptionKey);
                if (insertedRecord.Id < 0)
                {
                    _logger.LogWarning("Tenant encryption key insertion failed for TenantId: {TenantId}", newTenantId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while adding encryption key.",
                        Data = null
                    };
                }
              

                    Dictionary<int,string> departmentDict = getDepartnames.ToDictionary(x => x.ModuleId, x => x.ModuleName);

                List<Department> departmentList =
                    DepartmentSeedHelper.GetRuntimeDepartmentsToSeeds(departmentDict, newTenantId, request.TenantCreateRequestDTO.TenantIndustryId, newTenantId);

                int insertedAdminDepartment = await _unitOfWork.DepartmentRepository.AutoCreateDepartmentSeedAsync(departmentList);

                if (insertedAdminDepartment <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while adding department.",
                        Data = null
                    };
                }

                Dictionary<string, int> deptMap = await _unitOfWork.DepartmentRepository.GetDepartmentNameIdMapAsync(newTenantId);

                List<Designation> designations =
                    DesignationsSeedHelper.GetRuntimeDesignationsToSeed(newTenantId, newTenantId, deptMap);

                int adminDesignationId = await _unitOfWork.DesignationRepository.AutoCreateDesignationAsync(designations, insertedAdminDepartment);

                if (adminDesignationId <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while adding designation.",
                        Data = null
                    };
                }

                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var timePart = DateTime.UtcNow.ToString("HHmmss");
                var randomSuffix = Path.GetRandomFileName().Replace(".", "").Substring(0, 4).ToUpper();

                // Step 5️⃣: Create Employee for Tenant
                var employee = new Employee
                {
                     
                    // 🔒 Encrypt Official Email before saving
                    OfficialEmail = tenantEntity.TenantEmail,
                    TenantId = newTenantId,
                    HasPermanent = ConstantValues.IsByDefaultTrue,
                    FirstName = request.TenantCreateRequestDTO.ContactPersonName.Trim(),
                    IsActive = ConstantValues.IsByDefaultTrue,
                    DepartmentId = insertedAdminDepartment,
                    EmployementCode = $"{newTenantId}/{datePart}/{timePart}-{randomSuffix}",
                    DesignationId = adminDesignationId,
                    IsEditAllowed = ConstantValues.IsByDefaultTrue,
                    IsInfoVerified = ConstantValues.IsByDefaultFalse,
                    GenderId = request.TenantCreateRequestDTO.GenderId,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDateTime = null,
                    SoftDeletedById = null,
                    DeletedDateTime = null,
                    IsSoftDeleted = null,
                };
                // Step 8️⃣: Create LoginCredential
                var loginCredential = new LoginCredential
                {
                    TenantId = newTenantId,

                    // 🔒 Encrypt LoginId (Email) before saving
                    LoginId = tenantEntity.TenantEmail,

                    Employee = employee,
                    IsActive = ConstantValues.IsByDefaultTrue,

                    // 🔒 Encrypt Default Password before saving (for secure storage)
                    Password = hashedPassword,
                    HasFirstLogin = ConstantValues.IsByDefaultTrue,
                    IsSoftDeleted = ConstantValues.IsByDefaultFalse,
                    IsPasswordChangeRequired = ConstantValues.IsByDefaultTrue,
                    Remark = ConstantValues.TenantAllRoleRemark,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDateTime = null,
                    SoftDeletedById = null,
                    DeletedDateTime = null
                };
                // Step 7️⃣: Create Admin, HR, and Employee Roles
                var rolesToCreate = new List<Role>();
                var roleConfigs = new List<(string RoleName, int RoleType)>
                            {
                             (ConstantValues.TenantAdminRoleName, ConstantValues.RoleTypeAdmin),
                              (ConstantValues.TenantHRManagerRoleName, ConstantValues.RoleTypeManager),
                               (ConstantValues.TenantEmployeeRoleName, ConstantValues.RoleTypeEmployee)
                             };

                foreach (var (roleName, roleType) in roleConfigs)
                {
                    var role = new Role
                    {
                        TenantId = newTenantId,
                        RoleName = roleName,
                        RoleType = roleType,
                        IsSystemDefault = false,
                        IsActive = ConstantValues.IsByDefaultTrue,
                        IsSoftDeleted = false,
                        Remark = ConstantValues.TenantAllRoleRemark,
                        AddedById = newTenantId,
                        AddedDateTime = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDateTime = null,
                        SoftDeletedById = null,
                        DeletedDateTime = null
                    };

                    rolesToCreate.Add(role);
                }

                var createdAdminRole = await _unitOfWork.RoleRepository.AutoCreatedForTenantRoleAsync(rolesToCreate);

                if (createdAdminRole <= 0)
                {
                    _logger.LogWarning("Role creation failed. Rolling back transaction.");
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Role creation failed.",
                        Data = null
                    };
                }


                // Step 9️⃣: Assign Role to Employee
                UserRole userRole = new UserRole
                {
                   Employee = employee,
                    RoleId = createdAdminRole,
                    IsPrimaryRole = ConstantValues.IsByDefaultTrue,
                    IsActive = ConstantValues.IsByDefaultTrue,
                    AddedById = 0,
                    AddedDateTime = DateTime.Now,
                    AssignedDateTime = DateTime.Now,
                    Remark = ConstantValues.TenantAllRoleRemark,
                    AssignedById = 0,
                    RoleStartDate = DateTime.Now,
                    ApprovalRequired = ConstantValues.IsByDefaultFalse
                };

                //int roleId = (int)await _unitOfWork.UserRoleRepository.AddUserRoleAsync(userRole);

                //if (roleId <= 0)
                //{
                //    await _unitOfWork.RollbackTransactionAsync();
                //    return new ApiResponse<TenantCreateResponseDTO>
                //    {
                //        IsSucceeded = false,
                //        Message = "User role assignment failed.",
                //        Data = null
                //    };
                //}
                var createdEmployee = await _unitOfWork.Employees.CreateEmployeeAsync(employee, loginCredential, userRole);
              
                string EncriptedTenantId = _idEncoderService.EncodeId_long(newTenantId, null);
                //string EncriptedTenantId = _encryptionService.Encrypt(convertedTenantId, EncriptedTenantKey);
                //string EncriptedEmployeeId = _encryptionService.Encrypt(employeeId.ToString(), EncriptedTenantKey);
                //if (string.IsNullOrEmpty(EncriptedTenantId))
                //{
                //    _logger.LogWarning("Tenant ID encryption failed for TenantId: {TenantId}", newTenantId);
                //    await _unitOfWork.RollbackTransactionAsync();
                //    return new ApiResponse<TenantCreateResponseDTO>
                //    {
                //        IsSucceeded = false,
                //        Message = "An error occurred while encrypting tenant ID.",
                //        Data = null
                //    };
                //}

                // Step 6️⃣: Create TenantProfile
                var tenantProfile = new TenantProfile
                {
                    TenantId = newTenantId
                };
                long newTenantProfileId = await _unitOfWork.TenantRepository.AddTenantProfileAsync(tenantProfile);

                if (!long.TryParse(createdEmployee.Id, out long employeeId) || employeeId <= 0)
                {
                    _logger.LogError("Error occurred while creating employee.");
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "An error occurred while creating the employee.",
                        Data = null
                    };
                }


                //  long newLoginId = await _unitOfWork.UserLoginRepository.CreateUser(loginCredential);


                var UserRoleAndPermissionId =
                    await _unitOfWork.RoleRepository.AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(
                        newTenantId, employeeId, createdAdminRole);

                if (UserRoleAndPermissionId <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "User role and permission mapping failed.",
                        Data = null
                    };
                }
                string roletype = ConstantValues.RoleTypeAdmin.ToString();
                string EmplType = ConstantValues.ParmanentEmployeeType.ToString();
                GetTokenInfoDTO getTokenInfoDTO = new GetTokenInfoDTO()
                {
                    // ✅ Email is string already
                    UserId = tenantEntity.TenantEmail,       // ✅ Email is string already
                    EmployeeId = EncriptedTenantId,                 // ✅ Keep as long
                    RoleId = createdAdminRole.ToString(),               // ✅ Keep as long
                    RoleTypeId = ConstantValues.RoleTypeAdmin.ToString(),                            // ✅ string
                    EmployeeTypeId = ConstantValues.ParmanentEmployeeType.ToString(),                 // ✅ string
                    TenantId = EncriptedTenantId,                  // ✅ Keep as long
                    Email = tenantEntity.TenantEmail,
                    FullName = $"{request.TenantCreateRequestDTO.ContactPersonName}", // ✅ Construct properly
                    Expiry = DateTime.UtcNow.AddMinutes(30), // ✅ Token expiry after 30 minutes                  
                    IsFirstLogin = true,
                    IssuedAt = DateTime.UtcNow,
                    TokenPurpose = ConstantValues.SetPassword.ToString()


                };

                string token = await _tokenService.GenerateToken(getTokenInfoDTO);
                // 9️⃣ EMAIL (🔥 OUTSIDE TRANSACTION, FAILURE ≠ API FAILURE)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        string baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? string.Empty;
                        var emailService = _scopeFactory.CreateScope().ServiceProvider
                        .GetRequiredService<IEmailService>();

                       
                            await emailService.SendTemplatedEmailAsync(
                            ConstantValues.WelcomeEmail,
                            request.TenantCreateRequestDTO.TenantEmail!,
                            newTenantId,
                            new Dictionary<string, string>
                            {
                                ["UserName"] = request.TenantCreateRequestDTO.ContactPersonName,
                                ["VerificationUrl"] = $"{baseUrl}/auth/set-password?token={token}",
                                ["LinkExpiryMinutes"] = "30"
                            });
                        // Step 12️⃣: Commit Transaction

                        //

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Email send failed (non-blocking)");
                    }
                });

                await _unitOfWork.CommitTransactionAsync();

                // Step 13️⃣: Return Response (Decrypted Email for display)
                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Employee registration successful.",
                    Data = new TenantCreateResponseDTO
                    {
                        Success = true,                   
                        EmailSent = true,
 
                    }
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating tenant.");
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while creating the tenant.",
                    Data = null
                };
            }
        }
    }
}