using AutoMapper;
using axionpro.application.Common.SeedData;
using axionpro.application.Constants;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.Configruations;
using axionpro.application.DTOS.Token;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

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

        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateTenantCommandHandler> logger,
            IEmailService emailService,
            IStoreProcedureRepository commonRepository,
            IPasswordService passwordService,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IConfiguration configuration,
            IOptions<EmailConfig> emailConfig)
        {
            _tenantRepository = tenantRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService;
            _commonRepository = commonRepository;
            _passwordService = passwordService;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _configuration = configuration;
            _emailConfig = emailConfig.Value;
        }

        public async Task<ApiResponse<TenantCreateResponseDTO>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            long newTenantId = 0;

            try
            {
                var dto = request.TenantCreateRequestDTO;

                // =====================================================
                // STEP 1 : Validate request
                // =====================================================
                string prefix = dto.Prefix?.Trim().ToUpper() ?? string.Empty;
                string separator = dto.Separator?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(prefix))
                    return Fail("Prefix is required.");

                if (prefix.Length > 10)
                    return Fail("Prefix maximum length is 10 characters.");

                if (!Regex.IsMatch(prefix, "^[A-Z]+$"))
                    return Fail("Prefix must contain capital letters only. Example: QT, MSPL, BHEL.");

                if (string.IsNullOrWhiteSpace(separator) || !new[] { "_", "/", "-" }.Contains(separator))
                    return Fail("Separator must be one of these values: _, /, -");

                if (!int.TryParse(dto.RunningNumberLength, out int runningNumberLength))
                    return Fail("RunningNumberLength must be a valid number.");

                if (!new[] { 3, 4, 5, 6, 7 }.Contains(runningNumberLength))
                    return Fail("RunningNumberLength must be one of these values: 3, 4, 5, 6, 7.");

                if (string.IsNullOrWhiteSpace(dto.TenantEmail))
                    return Fail("Tenant email is required.");

                // =====================================================
                // STEP 2 : Duplicate checks
                // =====================================================
                bool isTenantEmailExists = await _unitOfWork.TenantRepository
                    .CheckTenantByEmailAsync(dto.TenantEmail);

                if (isTenantEmailExists)
                    return Fail("Tenant with this email already exists.");

                var existingUser = await _unitOfWork.UserLoginRepository
                    .GetEmployeeIdByUserLogin(dto.TenantEmail);

                if (existingUser != null)
                    return Fail("Tenant with this email already exists as an employee.");

                // =====================================================
                // STEP 3 : Prepare root entity
                // =====================================================
                var tenantEntity = _mapper.Map<Tenant>(dto);
                string? hashedPassword = null;

                // =====================================================
                // STEP 4 : Start transaction
                // =====================================================
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // =====================================================
                // STEP 5 : Create tenant
                // =====================================================
                await _unitOfWork.TenantRepository.AddTenantAsync(tenantEntity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                newTenantId = tenantEntity.Id;

                if (newTenantId <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Tenant creation failed.");
                }

                _logger.LogInformation("Tenant created successfully with TenantId: {TenantId}", newTenantId);

                // =====================================================
                // STEP 6 : Create tenant subscription
                // =====================================================
                var subscription = new TenantSubscription
                {
                    TenantId = newTenantId,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddDays(30),
                    IsActive = true,
                    IsTrial = true
                };

                var savedSub = await _unitOfWork.TenantSubscriptionRepository.AddTenantSubscriptionAsync(subscription);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                if (savedSub == null)
                {
                    await SafeRollbackAsync();
                    return Fail("Tenant subscription creation failed.");
                }

                // =====================================================
                // STEP 7 : Fetch subscription plan info
                // =====================================================
                var tenantSubscriptionPlan = await _unitOfWork.TenantSubscriptionRepository
                    .GetTenantSubscriptionPlanInfoAsync(new TenantSubscriptionPlanRequestDTO
                    {
                        TenantId = newTenantId,
                        SubscriptionPlanId = dto.SubscriptionPlanId,
                        IsTrial = true,
                        IsActive = true
                    });

                if (tenantSubscriptionPlan == null || !tenantSubscriptionPlan.Any())
                {
                    await SafeRollbackAsync();
                    return Fail("Tenant subscription plan details not found.");
                }

                // =====================================================
                // STEP 8 : Load subscribed modules
                // =====================================================
                List<Module> subscriptionModules = await _unitOfWork.PlanModuleMappingRepository
                    .GetAllSubscribedModuleAsync(dto.SubscriptionPlanId);

                if (subscriptionModules == null || !subscriptionModules.Any())
                {
                    await SafeRollbackAsync();
                    return Fail("No modules found for selected subscription plan.");
                }

                var leafNodeModules = subscriptionModules
                    .Where(m => m.IsLeafNode == true)
                    .ToList();

                var headerModules = subscriptionModules
                    .Where(m => m.IsLeafNode != true && m.ParentModule == null)
                    .ToList();

                // =====================================================
                // STEP 9 : Prepare tenant enabled modules
                // =====================================================
                List<TenantEnabledModule> tenantEnabledModules = headerModules
                    .Select(m => new TenantEnabledModule
                    {
                        TenantId = newTenantId,
                        ModuleId = m.Id,
                        IsEnabled = true,
                        ParentModuleId = m.ParentModuleId,
                        AddedById = newTenantId,
                        AddedDateTime = DateTime.UtcNow
                    })
                    .ToList();

                // =====================================================
                // STEP 10 : Prepare tenant enabled operations
                // =====================================================
                List<ModuleOperationMapping> allModuleOperations =
                    await _unitOfWork.ModuleOperationMappingRepository
                        .GetModuleOperationMappings(leafNodeModules);

                var tenantEnabledOperations = _mapper.Map<List<TenantEnabledOperation>>(allModuleOperations);

                tenantEnabledOperations.ForEach(x =>
                {
                    x.TenantId = newTenantId;
                });

                await _unitOfWork.TenantModuleConfigurationRepository.CreateByDefaultEnabledModulesAsync(
                    newTenantId,
                    tenantEnabledModules,
                    tenantEnabledOperations);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                // =====================================================
                // STEP 11 : Create tenant encryption key
                // =====================================================
                string encryptedTenantKey = _encryptionService.GenerateKey();

                var tenantEncryptionKey = new TenantEncryptionKey
                {
                    TenantId = newTenantId,
                    EncryptionKey = encryptedTenantKey,
                    IsActive = true
                };

                await _unitOfWork.TenantEncryptionKeyRepository.AddAsync(tenantEncryptionKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                // =====================================================
                // STEP 12 : Seed departments
                // =====================================================
                var departmentList = DepartmentSeedHelper.GetRuntimeDepartmentsToSeeds(
                    new Dictionary<int, string>(),
                    newTenantId,
                    dto.TenantIndustryId,
                    newTenantId);
                bool isDepartmentSeeded = await _unitOfWork.DepartmentRepository
                    .AutoCreateDepartmentSeedAsync(departmentList, cancellationToken);

                if (!isDepartmentSeeded)
                {
                    await SafeRollbackAsync();
                    return Fail("Department creation failed.");
                }
 
                // Save required here so seeded departments get real IDs
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                Dictionary<string, int> deptMap =
                    await _unitOfWork.DepartmentRepository.GetDepartmentNameIdMapAsync(newTenantId);

                if (deptMap == null || !deptMap.Any())
                {
                    await SafeRollbackAsync();
                    return Fail("Seeded departments not found.");
                }

                string executiveOfficeDepartmentName = departmentList
                    .FirstOrDefault(x => x.IsExecutiveOffice == true)?.DepartmentName ?? "Executive Office";

                if (!deptMap.TryGetValue(executiveOfficeDepartmentName, out int insertedAdminDepartment) || insertedAdminDepartment <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Executive Office department not found.");
                }

                // =====================================================
                // STEP 13 : Seed designations
                // =====================================================
                List<Designation> designations =
                    DesignationsSeedHelper.GetRuntimeDesignationsToSeed(
                        newTenantId,
                        newTenantId,
                        deptMap);

                int adminDesignationId =
                    await _unitOfWork.DesignationRepository.AutoCreateDesignationAsync(
                        designations,
                        insertedAdminDepartment);

                if (adminDesignationId <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Designation creation failed.");
                }

                // =====================================================
                // STEP 14 : Create employee code pattern
                // =====================================================
                var employeeCodePattern = new EmployeeCodePattern
                {
                    TenantId = newTenantId,
                    Prefix = prefix,
                    IncludeYear = dto.IncludeYear,
                    IncludeMonth = dto.IncludeMonth,
                    IncludeDepartment = dto.IncludeDepartment,
                    Separator = separator,
                    RunningNumberLength = runningNumberLength,
                    LastUsedNumber = 0,
                    IsActive = true,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.UtcNow
                };

                bool isEmpCodePatternCreated =
                    await _unitOfWork.TenantEmployeeCodePatternRepository.CreatePatternAsync(employeeCodePattern);

                if (!isEmpCodePatternCreated)
                {
                    await SafeRollbackAsync();
                    return Fail("Employee code pattern creation failed.");
                }

                // Save required here because next code generation may read active pattern from DB
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // =====================================================
                // STEP 15 : Generate employee code
                // =====================================================
                string? employmentCodeGenerated =
                    await _unitOfWork.TenantEmployeeCodePatternRepository
                        .GenerateEmployeeCodeAsync(newTenantId, insertedAdminDepartment);

                if (string.IsNullOrWhiteSpace(employmentCodeGenerated))
                {
                    await SafeRollbackAsync();
                    return Fail("Employee code generation failed.");
                }

                // =====================================================
                // STEP 16 : Create default roles
                // =====================================================
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
                        RoleType = roleType,
                        IsActive = true,
                        IsSoftDeleted = false,
                        IsSystemDefault = false,
                        AddedDateTime = DateTime.UtcNow,
                        AddedById = newTenantId
                    });
                }

                bool isRolesCreated = await _unitOfWork.RoleRepository.AutoCreatedForTenantRoleAsync(rolesToCreate);

                if (!isRolesCreated)
                {
                    await SafeRollbackAsync();
                    return Fail("Default role creation failed.");
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var adminRole = await _unitOfWork.RoleRepository.GetTenantAdminRoleAsync(newTenantId);

                if (adminRole == null || adminRole.Id <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Default admin role not found.");
                }

                int createdAdminRoleId = adminRole.Id;

                // =====================================================
                // STEP 17 : Create employee (tenant admin)
                // =====================================================
                var employee = new Employee
                {
                    TenantId = newTenantId,
                    FirstName = dto.ContactPersonName?.Trim(),
                    DepartmentId = insertedAdminDepartment,
                    DesignationId = adminDesignationId,
                    CountryId = dto.CountryId,
                    OfficialEmail = tenantEntity.TenantEmail,
                    EmployementCode = employmentCodeGenerated,
                    IsActive = true,
                    IsSoftDeleted = false,
                    IsEditAllowed = true,
                    EmployeeTypeId = ConstantValues.ParmanentEmployeeType,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.UtcNow
                };

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
                    Remark = "System Generated Account",
                    IsOnboard =true,
                    AddedById = newTenantId,
                    AddedDateTime = DateTime.UtcNow
                };

                var userRole = new UserRole
                {
                    Employee = employee,
                    RoleId = createdAdminRoleId,
                    IsPrimaryRole = true,
                    IsActive = true,
                    Remark = "Initial role assignment during employee creation",
                    AssignedDateTime = DateTime.UtcNow,
                    RoleStartDate = DateTime.UtcNow,
                    ApprovalRequired = false,
                    AddedDateTime = DateTime.UtcNow,
                    ApprovalStatus = ConstantValues.IsByDefaultTrue ? "Approved" : "Pending",
                    IsSoftDeleted = false,
                    AddedById = newTenantId
                };

                await _unitOfWork.Employees.AddEmployeeAggregateAsync(
                    employee,
                    loginCredential,
                    userRole,
                    cancellationToken);

                // Save required here because employeeId is needed
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                long employeeId = employee.Id;

                if (employeeId <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Employee creation failed.");
                }

                var createdEmployee = await _unitOfWork.Employees.GetCreatedEmployeeResponseAsync(employeeId, cancellationToken);

                if (createdEmployee == null || string.IsNullOrWhiteSpace(createdEmployee.Id))
                {
                    await SafeRollbackAsync();
                    return Fail("Employee creation failed.");
                }

                // =====================================================
                // STEP 18 : Create tenant profile
                // =====================================================
                var tenantProfile = new TenantProfile
                {
                    TenantId = newTenantId
                };

                await _unitOfWork.TenantRepository.AddTenantProfileAsync(tenantProfile);

                // =====================================================
                // STEP 19 : Create tenant email config
                // =====================================================
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
                        SecrateKey = _emailConfig.Secret
                    });

                // =====================================================
                // STEP 20 : Create role permission mapping
                // =====================================================
                await _unitOfWork.RoleRepository.AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(
                    newTenantId,
                    employeeId,
                    createdAdminRoleId);

                // =====================================================
                // STEP 21 : Final save before commit
                // =====================================================
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // =====================================================
                // STEP 22 : Prepare token
                // =====================================================
                string encryptedEmployeeId = _idEncoderService.EncodeId_long(employeeId, null);
                string encryptedTenantId = _idEncoderService.EncodeId_long(newTenantId, null);

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
                // STEP 23 : Generate token
                // =====================================================
                string token = await _tokenService.GenerateToken(getTokenInfoDTO);

                // =====================================================
                // STEP 24 : Commit transaction
                // =====================================================
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // =====================================================
                // STEP 25 : Send email after commit
                // =====================================================
                bool emailSent = false;

                try
                {
                    string baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? string.Empty;

                    await _emailService.SendTemplatedEmailAsync(
                        ConstantValues.WelcomeEmail,
                        employee.OfficialEmail!,
                        newTenantId,
                        new Dictionary<string, string>
                        {
                            ["UserName"] = employee.FirstName ?? string.Empty,
                            ["VerificationUrl"] = $"{baseUrl}/auth/set-password?token={token}",
                            ["LinkExpiryMinutes"] = "30"
                        });

                    emailSent = true;

                    _logger.LogInformation(
                        "Welcome email sent successfully | TenantId={TenantId}",
                        newTenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email send failed after tenant creation | TenantId={TenantId}", newTenantId);
                }

                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = true,
                    Message = emailSent
                        ? "Employee created successfully. Please check your email and set password."
                        : "Tenant created successfully, but welcome email could not be sent.",
                    Data = new TenantCreateResponseDTO
                    {
                        Success = true,
                        EmailSent = emailSent
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating tenant | TenantId={TenantId}", newTenantId);

                await SafeRollbackAsync();

                return new ApiResponse<TenantCreateResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while creating tenant. Please try again later.",
                    Data = new TenantCreateResponseDTO
                    {
                        Success = false,
                        EmailSent = false
                    }
                };
            }
        }

        private ApiResponse<TenantCreateResponseDTO> Fail(string message)
        {
            return new ApiResponse<TenantCreateResponseDTO>
            {
                IsSucceeded = false,
                Message = message,
                Data = new TenantCreateResponseDTO
                {
                    Success = false,
                    EmailSent = false
                }
            };
        }

        private async Task SafeRollbackAsync()
        {
            try
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Rollback transaction failed.");
            }
        }
    }
}