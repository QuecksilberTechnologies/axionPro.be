using AutoMapper;
using axionpro.application.Common.SeedData;
using axionpro.application.Constants;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.Configruations;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace axionpro.application.Features.RegistrationCmd.Handlers
{
    // Command definition for creating tenant
    public class CreateTenantCommand : IRequest<ApiResponse<TenantCreateResponseDTO>>
    {
        // DTO received from API
        public TenantCreateRequestDTO TenantCreateRequestDTO { get; set; }

        public CreateTenantCommand(TenantCreateRequestDTO createRequestDTO)
        {
            TenantCreateRequestDTO = createRequestDTO;
        }
    }
    
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ApiResponse<TenantCreateResponseDTO>>
    {
        // ===============================
        // Dependencies injected via DI
        // ===============================

        private readonly IEmailService _emailService;
        private readonly IStoreProcedureRepository _commonRepository;
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
        private readonly EmailConfig _emailConfig;


        // Constructor injection
        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository, ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateTenantCommandHandler> logger,
            IEmailService emailService,
            IStoreProcedureRepository commonRepository,
            IPasswordService passwordService,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IConfiguration configuration, IOptions<EmailConfig> emailConfig
             )
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
            _emailConfig = emailConfig.Value;

        }

        public async Task<ApiResponse<TenantCreateResponseDTO>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // STEP 1 : Check whether tenant already exists
                // =====================================================

                string prefix = request.TenantCreateRequestDTO.Prefix?.Trim().ToUpper() ?? string.Empty;
                string separator = request.TenantCreateRequestDTO.Separator?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(prefix))
                {
                    throw new Exception("Prefix is required.");
                }

                if (prefix.Length > 10)
                {
                    throw new Exception("Prefix maximum length is 10 characters.");
                }

                if (!Regex.IsMatch(prefix, "^[A-Z]+$"))
                {
                    throw new Exception("Prefix must contain capital letters only. Example: QT, MSPL, BHEL.");
                }

                if (string.IsNullOrWhiteSpace(separator) || !new[] { "_", "/", "-" }.Contains(separator))
                {
                    throw new Exception("Separator must be one of these values: _, /, -");
                }

                if (!int.TryParse(request.TenantCreateRequestDTO.RunningNumberLength, out int runningNumberLength))
                {
                    throw new Exception("RunningNumberLength must be a valid number.");
                }

                if (!new[] { 3, 4, 5, 6, 7 }.Contains(runningNumberLength))
                {
                    throw new Exception("RunningNumberLength must be one of these values: 3, 4, 5, 6, 7.");
                }

                bool isTenantEmailExists = await _unitOfWork.TenantRepository
                    .CheckTenantByEmail(request.TenantCreateRequestDTO.TenantEmail);

                if (isTenantEmailExists)
                {
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant with this email already exists."
                    };
                }

                // 3️⃣ Check existing login
                var existingUser = await _unitOfWork.UserLoginRepository
                    .GetEmployeeIdByUserLogin(request.TenantCreateRequestDTO.TenantEmail);

                if (existingUser != null)
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant with this email already exists as an employee."
                    };

                // Password placeholder
                string? hashedPassword = null;

                // =====================================================
                // STEP 2 : Map DTO → Tenant Entity
                // =====================================================
                var tenantEntity = _mapper.Map<Tenant>(request.TenantCreateRequestDTO);

                // =====================================================
                // STEP 3 : Start DB transaction
                // =====================================================
                await _unitOfWork.BeginTransactionAsync();

                // =====================================================
                // STEP 4 : Insert Tenant
                // =====================================================
                long newTenantId = await _unitOfWork.TenantRepository
                    .AddTenantAsync(tenantEntity);
                await _unitOfWork.SaveChangesAsync();

                long newTenantId1 = tenantEntity.Id;

                if (newTenantId <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant creation failed."
                    };
                }

                // =====================================================
                // STEP 5 : Create Tenant Subscription
                // =====================================================
                TenantSubscription savedSub =
                    await _unitOfWork.TenantSubscriptionRepository
                    .AddTenantSubscriptionAsync(new TenantSubscription
                    {
                        TenantId = newTenantId,
                        SubscriptionPlanId = request.TenantCreateRequestDTO.SubscriptionPlanId,
                        SubscriptionStartDate = DateTime.UtcNow,
                        SubscriptionEndDate = DateTime.UtcNow.AddDays(30),
                        IsActive = true,
                        IsTrial = true
                    });

                if (savedSub == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Tenant Subscription failed."
                    };
                }

                // =====================================================
                // STEP 6 : Fetch subscription plan details
                // =====================================================
                var tenantSubscriptionPlan =
                    await _unitOfWork.TenantSubscriptionRepository
                    .GetTenantSubscriptionPlanInfoAsync(
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
                        Message = "Tenant Subscription plan details not found."
                    };
                }

                // =====================================================
                // STEP 7 : Get modules included in subscription plan
                // =====================================================
                List<domain.Entity.Module> subscriptionPlans =
                    await _unitOfWork.PlanModuleMappingRepository
                    .GetAllSubscribedModuleAsync(
                        request.TenantCreateRequestDTO.SubscriptionPlanId);

                // Split modules
                var leafNodeModules = subscriptionPlans.Where(m => (bool)m.IsLeafNode).ToList();
                var headerModules = subscriptionPlans
                    .Where(m => m.IsLeafNode != true && m.ParentModule == null)
                    .ToList();

                // =====================================================
                // STEP 8 : Enable header modules for tenant
                // =====================================================
                List<TenantEnabledModule> TenantEnabledModules =
                    headerModules.Select(m => new TenantEnabledModule
                    {
                        TenantId = newTenantId,
                        ModuleId = m.Id,
                        IsEnabled = true,
                        ParentModuleId = m.ParentModuleId,
                        AddedById = newTenantId,
                        AddedDateTime = DateTime.UtcNow
                    }).ToList();

                // =====================================================
                // STEP 9 : Enable operations for modules
                // =====================================================
                List<ModuleOperationMapping> allModuleOperations =
                    await _unitOfWork.ModuleOperationMappingRepository
                    .GetModuleOperationMappings(leafNodeModules);


                var tenantEnabledOperations = _mapper.Map<List<TenantEnabledOperation>>(allModuleOperations);

                tenantEnabledOperations.ForEach(x => { x.TenantId = newTenantId; });

                await _unitOfWork.TenantModuleConfigurationRepository
                    .CreateByDefaultEnabledModulesAsync(
                        newTenantId,
                        TenantEnabledModules,
                        tenantEnabledOperations);

                // =====================================================
                // STEP 10 : Create encryption key for tenant
                // =====================================================
                string encryptedTenantKey = _encryptionService.GenerateKey();

                TenantEncryptionKey tenantEncryptionKey = new()
                {
                    TenantId = newTenantId,
                    EncryptionKey = encryptedTenantKey,
                    IsActive = true
                };

                var insertedRecord =
                    _unitOfWork.TenantEncryptionKeyRepository
                    .AddAsync(tenantEncryptionKey);

                // =====================================================
                // STEP 11 : Seed Departments
                // =====================================================
                var departmentList =
                    DepartmentSeedHelper.GetRuntimeDepartmentsToSeeds(
                        new Dictionary<int, string>(),
                        newTenantId,
                        request.TenantCreateRequestDTO.TenantIndustryId,
                        newTenantId);

                int insertedAdminDepartment =
                    await _unitOfWork.DepartmentRepository
                    .AutoCreateDepartmentSeedAsync(departmentList);

                if (insertedAdminDepartment <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Department creation failed."
                    };
                }

                // =====================================================
                // STEP 12 : Seed Designations
                // =====================================================
                Dictionary<string, int> deptMap =
                    await _unitOfWork.DepartmentRepository
                    .GetDepartmentNameIdMapAsync(newTenantId);

                List<Designation> designations =
                    DesignationsSeedHelper.GetRuntimeDesignationsToSeed(
                        newTenantId,
                        newTenantId,
                        deptMap);

                int adminDesignationId =
                    await _unitOfWork.DesignationRepository
                    .AutoCreateDesignationAsync(
                        designations,
                        insertedAdminDepartment);


                EmployeeCodePattern employeeCodePattern = new()
                {
                    TenantId = newTenantId,
                    Prefix = prefix,
                    IncludeYear = request.TenantCreateRequestDTO.IncludeYear,
                    IncludeMonth = request.TenantCreateRequestDTO.IncludeMonth,
                    IncludeDepartment = request.TenantCreateRequestDTO.IncludeDepartment,
                    Separator = separator,
                    RunningNumberLength = runningNumberLength,
                    LastUsedNumber = 0,
                    IsActive = true,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.UtcNow,
                };

                bool isEmpCodeGenrated =
                     await _unitOfWork.TenantEmployeeCodePatternRepository.CreatePatternAsync(employeeCodePattern);

                if (!isEmpCodeGenrated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Employee code pattern creation failed."
                    };
                }

                // 4️⃣ Generate Employee Code
                string? employementCodeGenerated =
                    await _unitOfWork.TenantEmployeeCodePatternRepository
                        .GenerateEmployeeCodeAsync(newTenantId, insertedAdminDepartment);

                if (string.IsNullOrWhiteSpace(employementCodeGenerated))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<TenantCreateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Employee code generation failed."
                    };
                }

                // STEP 13 : Create Employee (Tenant Admin)
                var employee = new Employee
                {
                    TenantId = newTenantId,
                    FirstName = request.TenantCreateRequestDTO.ContactPersonName.Trim(),
                    DepartmentId = insertedAdminDepartment,
                    DesignationId = adminDesignationId,
                    CountryId = request.TenantCreateRequestDTO.CountryId,
                    OfficialEmail = tenantEntity.TenantEmail,
                    EmployementCode = employementCodeGenerated,
                    IsActive = true,
                    IsSoftDeleted = false,
                    IsEditAllowed = true,
                    EmployeeTypeId = ConstantValues.ParmanentEmployeeType,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.UtcNow,
                };

                // =====================================================
                // STEP 14 : Create login credentials
                // =====================================================
                var loginCredential = new LoginCredential
                {
                    TenantId = newTenantId,
                    LoginId = tenantEntity.TenantEmail,
                    Employee = employee,
                    Password = hashedPassword,
                    IsActive = true,
                    IsSoftDeleted = false,
                    IsPasswordChangeRequired = true,
                    HasFirstLogin = true,
                    Remark = "System Genrated Account",
                    AddedById = employee.Id,
                    AddedDateTime = DateTime.UtcNow,    
                    


                };

                // =====================================================
                // STEP 15 : Create default roles
                var rolesToCreate = new List<Role>();

                foreach (var roleName in new[]
                {
    ConstantValues.TenantAdminRoleName,
    ConstantValues.TenantHRManagerRoleName,
    ConstantValues.TenantEmployeeRoleName
})
                {
                    int roleType = roleName switch
                    {
                        var r when r == ConstantValues.TenantAdminRoleName => ConstantValues.RoleTypeAdmin,
                        var r when r == ConstantValues.TenantHRManagerRoleName => ConstantValues.RoleTypeManager,
                        var r when r == ConstantValues.TenantEmployeeRoleName => ConstantValues.RoleTypeEmployee,
                        _ => 0
                    };

                    rolesToCreate.Add(new Role
                    {
                        TenantId = newTenantId,
                        RoleName = roleName,
                        RoleType = roleType, // ✅ FIXED

                        IsActive = true,
                        IsSoftDeleted = false,
                        IsSystemDefault = false,

                        AddedDateTime = DateTime.UtcNow,
                        AddedById = employee.Id,
                    });
                }

                int createdAdminRole =
                await _unitOfWork.RoleRepository
                .AutoCreatedForTenantRoleAsync(rolesToCreate);

                // =====================================================
                // STEP 16 : Assign role to employee
                // =====================================================
                UserRole userRole = new()
                {
                    Employee = employee,
                    RoleId = createdAdminRole,
                    IsPrimaryRole = true,
                    IsActive = true,
                    Remark = "Initial role assignment during employee creation",
                    AssignedDateTime = DateTime.UtcNow,
                    RoleStartDate = DateTime.UtcNow,
                    ApprovalRequired = false,
                    AddedDateTime = DateTime.UtcNow,
                    ApprovalStatus = ConstantValues.IsByDefaultTrue ? "Approved" : "Pending",
                    IsSoftDeleted =false,
                    AddedById = employee.Id,

                     
                };

                var createdEmployee =
                await _unitOfWork.Employees
                .CreateEmployeeAsync(employee, loginCredential, userRole);

                // =====================================================
                // STEP 17 : Create tenant profile
                // =====================================================
                var tenantProfile = new TenantProfile
                {
                    TenantId = newTenantId
                };

                await _unitOfWork.TenantRepository
                .AddTenantProfileAsync(tenantProfile);

                await _unitOfWork.TenantEmailConfigRepository.InsertEmailConfigAsync(
                            new TenantEmailConfig
                            {
                                TenantId = newTenantId,
                                SmtpHost = ConstantValues.DefaultSmtpHost,
                                SmtpPort = ConstantValues.DefaultSmtpPort,
                                SmtpUsername = ConstantValues.DefaultSmtpUserName,
                                SmtpPasswordEncrypted = _emailConfig.Secret,
                                FromEmail = ConstantValues.DefaultFromEmail,
                                FromName = ConstantValues.DefaultFromName,
                                IsActive = true,
                                SecrateKey = _emailConfig.Secret,
                            });

                // =====================================================
                // STEP 18 : Create role permission mapping
                // =====================================================
                await _unitOfWork.RoleRepository
                .AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(
                    newTenantId,
                    long.Parse(createdEmployee.Id),
                    createdAdminRole);

                // =====================================================
                // STEP 19 : Prepare Token Information
                // =====================================================

                long employeeId = long.Parse(createdEmployee.Id);

                string encryptedEmployeeId =
                _idEncoderService.EncodeId_long(employeeId, null);

                string encryptedTenantId =
                _idEncoderService.EncodeId_long(newTenantId, null);

                var getTokenInfoDTO = new GetTokenInfoDTO
                {
                    EmployeeId = encryptedEmployeeId,
                    TenantId = encryptedTenantId,
                    Email = employee.OfficialEmail!,
                    FullName = employee.FirstName,
                    TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, ""),
                    IssuedAt = DateTime.UtcNow,
                    Expiry = DateTime.UtcNow.AddMinutes(30),
                    IsFirstLogin = true,
                    ClientType = "Web"
                };

                // =====================================================
                // STEP 20 : Generate Token
                // =====================================================

                string token = await _tokenService.GenerateToken(getTokenInfoDTO);

                // =====================================================
                // STEP 21 : Commit Transaction FIRST
                // =====================================================


                await _unitOfWork.CommitTransactionAsync();

                // =====================================================
                // STEP 22 : Send Welcome Email
                // =====================================================

                try
                {
                    string baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? string.Empty;

                    await _emailService.SendTemplatedEmailAsync(
                        ConstantValues.WelcomeEmail,
                        employee.OfficialEmail!,
                        newTenantId,
                        new Dictionary<string, string>
                        {
                            ["UserName"] = employee.FirstName,
                            ["VerificationUrl"] = $"{baseUrl}/auth/set-password?token={token}",
                            ["LinkExpiryMinutes"] = "30"
                        });

                    _logger.LogInformation(
                        "Welcome email sent successfully | TenantId={TenantId}",
                        newTenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email send failed (non-blocking)");
                }

                // =====================================================
                // FINAL RESPONSE
                // =====================================================

                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Employee created successfully. Please check your email and set password.",
                    Data = new TenantCreateResponseDTO
                    {
                        Success = true,
                        EmailSent = true
                    }
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating tenant");
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while creating tenant. Please try again later."
                };
            }
        }
    }
}



