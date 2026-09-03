// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant registrations and validates their selected subscription plans.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.SeedData;
using axionpro.application.Constants;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOS.Configruations;
using axionpro.application.DTOS.Token;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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
    /// <summary>
    /// Represents a request to create a tenant and its initial subscription.
    /// </summary>
    public class CreateTenantCommand : IRequest<ApiResponse<TenantCreateResponseDTO>>
    {
        public TenantCreateRequestDTO TenantCreateRequestDTO { get; set; }

        public CreateTenantCommand(TenantCreateRequestDTO createRequestDTO)
        {
            TenantCreateRequestDTO = createRequestDTO;
        }
    }

    /// <summary>
    /// Creates a tenant registration after validating its selected subscription plan.
    /// </summary>
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ApiResponse<TenantCreateResponseDTO>>
    {
        private const int TenantOnboardingLinkExpiryMinutes = 30;

        private readonly IEmailService _emailService;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTenantCommandHandler> _logger;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IConfiguration _configuration;
        private readonly EmailConfig _emailConfig;
        private readonly ICommonRequestService _commonRequestService;

        public CreateTenantCommandHandler(
            ITokenService tokenService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateTenantCommandHandler> logger,
            IEmailService emailService,
            IStoreProcedureRepository commonRepository,
            IPasswordService passwordService,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IConfiguration configuration,
            IOptions<EmailConfig> emailConfig,
            ICommonRequestService commonRequestService)
        {
            _tokenService = tokenService;
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
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<TenantCreateResponseDTO>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            long newTenantId = 0;
            var dto = request?.TenantCreateRequestDTO
                ?? throw new ArgumentNullException(nameof(request.TenantCreateRequestDTO));
            var onboardingRequest = dto as INewTenantOnboardingConfiguration;

            // The public registration endpoint has no authenticated request context. Only the
            // Host-side onboarding bridge carries the extended configuration and must validate
            // the caller's current Host module-operation permission.
            if (onboardingRequest is not null)
            {
                await HostRuntimePermissionValidator.ValidateAsync(
                    _commonRequestService,
                    _commonRepository,
                    dto.ModuleId,
                    dto.OperationId,
                    cancellationToken);
            }

            try
            {
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

                #region Subscription Plan Validation

                // Prevent a new tenant from being assigned to a soft-deleted subscription plan.
                var subscriptionPlan = await _unitOfWork.SubscriptionRepository
                    .GetNonDeletedSubscriptionPlanByIdAsync(dto.SubscriptionPlanId, cancellationToken);

                if (subscriptionPlan is null)
                    return Fail(AppConstants.ErrorMessages.SubscriptionPlanNotFound);

                #endregion

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

                if (onboardingRequest is not null)
                {
                    var location = onboardingRequest.InitialLocation;
                    if (location.LocationType is < 1 or > 4 ||
                        string.IsNullOrWhiteSpace(location.LocationCode) ||
                        string.IsNullOrWhiteSpace(location.TimeZoneId))
                    {
                        await SafeRollbackAsync();
                        return Fail("A valid initial Tenant location is required.");
                    }

                    await _unitOfWork.TenantLocationRepository.AddAsync(new TenantLocation
                    {
                        TenantId = newTenantId,
                        LocationCode = location.LocationCode.Trim(),
                        LocationName = string.IsNullOrWhiteSpace(location.LocationName) ? tenantEntity.CompanyName : location.LocationName.Trim(),
                        LocationType = location.LocationType,
                        CountryId = dto.CountryId,
                        StateId = location.StateId,
                        CityId = location.CityId,
                        Address = location.Address,
                        Landmark = location.Landmark,
                        PostalCode = location.PostalCode,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        GeoFenceRadiusMeters = location.GeoFenceRadiusMeters,
                        TimeZoneId = location.TimeZoneId.Trim(),
                        IsHeadOffice = location.LocationType == 1,
                        IsGeoFenceEnabled = location.IsGeoFenceEnabled,
                        IsAttendanceAllowed = location.IsAttendanceAllowed,
                        IsBiometricEnabled = location.IsBiometricEnabled,
                        IsActive = true,
                        IsSoftDeleted = false,
                        AddedById = newTenantId,
                        AddedDateTime = DateTime.UtcNow
                    }, cancellationToken);
                }

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

                // =====================================================
                // STEP 9 : Prepare tenant enabled modules
                // =====================================================
                List<TenantEnabledModule> tenantEnabledModules = subscriptionModules
                    .Select(m => new TenantEnabledModule
                    {
                        TenantId = newTenantId,
                        ModuleId = m.Id,
                        ParentModuleId = m.ParentModuleId,
                        IsLeafNode = m.IsLeafNode,
                        IsEnabled = true,
                        AddedById = newTenantId,
                        AddedDateTime = DateTime.UtcNow
                    })
                    .ToList();

                // =====================================================
                // STEP 10 : Prepare tenant enabled operations
                // =====================================================
                List<ModuleOperationMapping> allModuleOperations =
                    await _unitOfWork.UserRolesPermissionOnModuleRepository
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

                var tenantEncryptionKey = new TenantEncryptionKeys
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
                    ConstantValues.TenantManagerRoleName,
                    ConstantValues.TenantEmployeeRoleName
                                          })
                {
                    int roleType = roleName switch
                    {
                        var r when r == ConstantValues.TenantAdminRoleName => ConstantValues.RoleTypeAdmin,
                        var r when r == ConstantValues.TenantManagerRoleName => ConstantValues.RoleTypeManager,
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
                // =====================================================
                // STEP 16.1 : Create Role Permissions 🔥
                // =====================================================
               

                // 🔥 IMPORTANT: rolesToCreate me Id reliable nahi hota → DB se fetch karo
                var adminRole = await _unitOfWork.RoleRepository.GetTenantAdminRoleAsync(newTenantId);

                if (adminRole == null || adminRole.Id <= 0)
                {
                    await SafeRollbackAsync();
                    return Fail("Admin role not found for permission setup.");
                }
 
                // Get enabled modules + operations (tenant specific)
                TenantEnabledOperation tenantEnabledOperation = new TenantEnabledOperation
                {
                    TenantId = newTenantId
                };

                var moduleOperations = await _unitOfWork.UserRolesPermissionOnModuleRepository.GetAllTenantModuleWithOperation(tenantEnabledOperation);

                // Prepare permissions (ONLY ADMIN)
                var rolePermissions = new List<RoleModuleAndPermission>();

                if (moduleOperations?.Modules != null)
                {
                    foreach (var module in moduleOperations.Modules)
                    {
                        if (module.Operations == null) continue;

                        foreach (var op in module.Operations)
                        {
                            rolePermissions.Add(new RoleModuleAndPermission
                            {
                                RoleId = adminRole.Id, // 🔥 ONLY ADMIN
                                ModuleId = module.Id,
                                OperationId = op.Id,
                                HasAccess = true,      // 🔥 FULL ACCESS
                                IsActive = true,
                                AddedById = newTenantId,
                                AddedDateTime = DateTime.UtcNow,
                                Remark = "Auto-assigned during tenant creation for admin role"


                            });
                        }
                    }
                }

                // 4️⃣ Bulk insert
                int insertedCount = await _unitOfWork.UserRolesPermissionOnModuleRepository.BulkInsertAsync(rolePermissions);
                // 5️⃣ Save
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                        "Admin has been assigned permissions | TenantId={TenantId}, InsertedCount={InsertedCount}",
                        newTenantId, insertedCount);



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

                // ===============================
                //  CREATE POLICY TYPE
                // ===============================
                var policyTypes = new List<PolicyType>
                        {
                            new PolicyType
                            {
                                TenantId = newTenantId,
                                PolicyName = ConstantValues.DefaultInsurancePolicy,
                                Description = "System Generated Predefined Insurance Policy",
                                IsActive = true,
                                IsStructured = true,
                                IsSoftDelete = false,
                                AddedById = newTenantId,
                                AddedDateTime = DateTime.UtcNow
                            },
                        new PolicyType
                        {
                            TenantId = newTenantId,
                            PolicyName = ConstantValues.DefaultLeavePolicy,
                            Description = "System Generated Predefined Leave Policy",
                            IsActive = true,
                            IsStructured = true,
                            IsSoftDelete = false,
                            AddedById = newTenantId,
                            AddedDateTime = DateTime.UtcNow
                        }
                    };
                    
                // INSERT POLICY TYPES
                // ===============================
                var insertedPolicies = await _unitOfWork.PolicyTypeRepository
                    .AutoCreatePolicyTypesAsync(policyTypes);
              

                // ✅ VALIDATE INSERT RESULT
                if (insertedPolicies == null || !insertedPolicies.Any())
                {
                    await SafeRollbackAsync();
                    throw new Exception("Policy creation failed. Insert operation returned empty.");
                }

                // ===============================
                // FINAL COMMIT (SINGLE TRANSACTION)
                // ===============================
                await _unitOfWork.SaveChangesAsync(cancellationToken);


                // =====================================================
                // STEP 18 : Create tenant profile
                // =====================================================
                var tenantProfile = new TenantProfile
                {
                    TenantId = newTenantId,
                    Address = onboardingRequest?.Profile.Address,
                    LogoUrl = onboardingRequest?.Profile.LogoUrl,
                    ThemeColor = onboardingRequest?.Profile.ThemeColor,
                    BusinessType = onboardingRequest?.Profile.BusinessType,
                    Industry = onboardingRequest?.Profile.Industry,
                    TotalEmployees = onboardingRequest?.Profile.TotalEmployees,
                    TotalBranches = onboardingRequest?.Profile.TotalBranches,
                    FoundedYear = onboardingRequest?.Profile.FoundedYear,
                    WebsiteUrl = onboardingRequest?.Profile.WebsiteUrl
                };

                await _unitOfWork.TenantRepository.AddTenantProfileAsync(tenantProfile);

                // =====================================================
                // STEP 19 : Create tenant email config
                // =====================================================
                await _unitOfWork.TenantEmailConfigRepository.InsertEmailConfigAsync(
                    BuildTenantEmailConfiguration(newTenantId, onboardingRequest));

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
                // STEP 23 : Prepare token
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
                    Expiry = DateTime.UtcNow.AddMinutes(TenantOnboardingLinkExpiryMinutes),
                    IsFirstLogin = true,
                    ClientType = "Web"
                };

                // =====================================================
                // STEP 24 : Generate a valid onboarding token before committing.
                // =====================================================
                string token = await _tokenService.GenerateTenantToken(getTokenInfoDTO);
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogError(
                        "Tenant onboarding token generation failed | TenantId={TenantId}",
                        newTenantId);
                    await SafeRollbackAsync();
                    return Fail("Tenant onboarding link could not be generated.");
                }

                // =====================================================
                // STEP 25 : Commit the Tenant aggregate before attempting external SMTP work.
                // =====================================================
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // =====================================================
                // STEP 26 : Send email after commit. A mail failure must not undo a valid Tenant.
                // =====================================================
                var emailSent = await SendTenantWelcomeEmailAsync(
                    newTenantId,
                    employee.OfficialEmail!,
                    employee.FirstName,
                    token);

                return CreateSuccessResponse(emailSent);
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

        private TenantEmailConfig BuildTenantEmailConfiguration(
            long tenantId,
            INewTenantOnboardingConfiguration? onboardingRequest)
        {
            var emailConfiguration = onboardingRequest?.EmailConfiguration;
            var smtpPassword = GetConfiguredValue(
                emailConfiguration?.SmtpPasswordEncrypted,
                _emailConfig.Secret);

            return new TenantEmailConfig
            {
                TenantId = tenantId,
                SmtpHost = GetConfiguredValue(
                    emailConfiguration?.SmtpHost,
                    ConstantValues.DefaultSmtpHost),
                SmtpPort = emailConfiguration?.SmtpPort ?? ConstantValues.DefaultSmtpPort,
                SmtpUsername = GetConfiguredValue(
                    emailConfiguration?.SmtpUsername,
                    GetConfiguredValue(_emailConfig.SMTPUserName, ConstantValues.DefaultSmtpUserName)),
                SmtpPasswordEncrypted = smtpPassword,
                FromEmail = GetConfiguredValue(
                    emailConfiguration?.FromEmail,
                    ConstantValues.DefaultFromEmail),
                FromName = GetConfiguredValue(
                    emailConfiguration?.FromName,
                    ConstantValues.DefaultFromName),
                IsActive = emailConfiguration?.IsActive ?? true,
                SecrateKey = GetConfiguredValue(emailConfiguration?.SecrateKey, smtpPassword)
            };
        }

        private static string? GetConfiguredValue(string? suppliedValue, string? fallbackValue)
        {
            return string.IsNullOrWhiteSpace(suppliedValue)
                ? fallbackValue
                : suppliedValue.Trim();
        }

        private async Task<bool> SendTenantWelcomeEmailAsync(
            long tenantId,
            string recipientEmail,
            string? recipientName,
            string token)
        {
            var baseUrl = _configuration["FrontEndWebURL:BaseUrl"]?.Trim();
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            {
                _logger.LogError(
                    "Tenant welcome email was not sent because FrontEndWebURL:BaseUrl is invalid | TenantId={TenantId}",
                    tenantId);
                return false;
            }

            var verificationUrl = $"{baseUrl.TrimEnd('/')}/auth/set-password?token={Uri.EscapeDataString(token)}";

            try
            {
                var emailSent = await _emailService.SendTemplatedEmailAsync(
                    ConstantValues.WelcomeEmail,
                    recipientEmail,
                    tenantId,
                    new Dictionary<string, string>
                    {
                        ["UserName"] = recipientName ?? string.Empty,
                        ["VerificationUrl"] = verificationUrl,
                        ["LinkExpiryMinutes"] = TenantOnboardingLinkExpiryMinutes.ToString()
                    });

                if (emailSent)
                {
                    _logger.LogInformation(
                        "Tenant welcome email accepted by SMTP | TenantId={TenantId}",
                        tenantId);
                }
                else
                {
                    _logger.LogWarning(
                        "Tenant welcome email was not accepted by SMTP | TenantId={TenantId}",
                        tenantId);
                }

                return emailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Tenant welcome email dispatch failed after Tenant creation | TenantId={TenantId}",
                    tenantId);
                return false;
            }
        }

        private static ApiResponse<TenantCreateResponseDTO> CreateSuccessResponse(bool emailSent)
        {
            return new ApiResponse<TenantCreateResponseDTO>
            {
                IsSucceeded = true,
                Message = emailSent
                    ? "Tenant created successfully. Please check your email and set password."
                    : "Tenant created successfully, but welcome email could not be sent.",
                Data = new TenantCreateResponseDTO
                {
                    Success = true,
                    EmailSent = emailSent
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
