using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class CreateBaseEmployeeInfoCommand : IRequest<ApiResponse<GetBaseEmployeeResponseDTO>>
    {
        public CreateBaseEmployeeRequestDTO DTO { get; set; }

        public CreateBaseEmployeeInfoCommand(CreateBaseEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateBaseEmployeeInfoCommandHandler
   : IRequestHandler<CreateBaseEmployeeInfoCommand, ApiResponse<GetBaseEmployeeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBaseEmployeeInfoCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;


        public CreateBaseEmployeeInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateBaseEmployeeInfoCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService, ITokenService tokenService, IEmailService emailService, IConfiguration configuration
             )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;

        }

        public async Task<ApiResponse<GetBaseEmployeeResponseDTO>> Handle(
      CreateBaseEmployeeInfoCommand request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("CreateEmployee started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION (YOUR PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to create employee.");

                // ===============================
                // 4️⃣ CHECK DUPLICATE USER
                // ===============================
                var existingUser =
                    await _unitOfWork.UserLoginRepository
                        .GetEmployeeIdByUserLogin(request.DTO.OfficialEmail);

                if (existingUser != null)
                    throw new ApiException("User already exists.", 409);

                // ===============================
                // 5️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 6️⃣ MAP EMPLOYEE
                // ===============================
                var employee = _mapper.Map<Employee>(request.DTO);

                employee.TenantId = validation.TenantId;
                employee.AddedById = validation.UserEmployeeId;
                employee.AddedDateTime = DateTime.UtcNow;
                employee.DateOfOnBoarding = DateTime.UtcNow;
                employee.IsActive = true;
                employee.IsInfoVerified = false;
                employee.IsEditAllowed = true;
                employee.IsSoftDeleted = false;

                // ===============================
                // 7️⃣ GENERATE EMPLOYEE CODE
                // ===============================
                employee.EmployementCode =
                    await _unitOfWork.TenantEmployeeCodePatternRepository
                        .GenerateEmployeeCodeAsync(
                            validation.TenantId,
                            employee.DepartmentId);

                // ===============================
                // 8️⃣ DEFAULT ROLE
                // ===============================
                if (request.DTO.RoleId <= 0)
                {
                    var roles = await _unitOfWork.RoleRepository
                        .GetRoleAsync(validation.TenantId,
                                      ConstantValues.RoleTypeEmployee,
                                      true);

                    var defaultRole = roles.FirstOrDefault();

                    if (defaultRole == null)
                        throw new ValidationErrorException("Default employee role not configured.");

                    request.DTO.RoleId = defaultRole.Id;
                }

                // ===============================
                // 9️⃣ CREATE RELATED ENTITIES
                // ===============================
                var loginCredential = new LoginCredential
                {
                    TenantId = employee.TenantId,
                    LoginId = request.DTO.OfficialEmail,
                    HasFirstLogin = true,
                    IsPasswordChangeRequired = true,
                    IsActive = true,
                    Employee = employee
                };

                var userRole = new UserRole
                {
                    Employee = employee,
                    RoleId = request.DTO.RoleId,
                    RoleStartDate = DateTime.UtcNow,
                    IsPrimaryRole = true,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                // ===============================
                // 🔟 SAVE
                // ===============================
                var savedEmployee =
                    await _unitOfWork.Employees
                        .CreateEmployeeAsync(employee, loginCredential, userRole);

                if (savedEmployee == null)
                    throw new ApiException("Employee creation failed.", 500);

                // ===============================
                // 1️⃣1️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 1️⃣2️⃣ RESPONSE
                // ===============================
                var responseDto =
                    ProjectionHelper.ToGetBaseInfoResponseDTO(
                        savedEmployee,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey);

                // ===============================
                // 1️⃣3️⃣ EMAIL (NON-BLOCKING)
                // ===============================
                try
                {
                    var token = await _tokenService.GenerateToken(new GetTokenInfoDTO
                    {
                        EmployeeId = responseDto.Id,
                        Email = savedEmployee.OfficialEmail!,
                        FullName = $"{savedEmployee.FirstName} {savedEmployee.LastName}",
                        TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, ""),
                        IssuedAt = DateTime.UtcNow,
                        Expiry = DateTime.UtcNow.AddMinutes(30),
                        IsFirstLogin = true,
                        ClientType = "Web"
                    });

                    string baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? "";

                    await _emailService.SendTemplatedEmailAsync(
                        ConstantValues.WelcomeEmail,
                        savedEmployee.OfficialEmail!,
                        validation.TenantId,
                        new Dictionary<string, string>
                        {
                            ["UserName"] = savedEmployee.FirstName,
                            ["VerificationUrl"] = $"{baseUrl}/auth/set-password?token={token}",
                            ["LinkExpiryMinutes"] = "30"
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email send failed (non-blocking)");
                }

                _logger.LogInformation("CreateEmployee success");

                return ApiResponse<GetBaseEmployeeResponseDTO>
                    .Success(responseDto, "Employee created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Employee creation failed");

                throw; // 🚨 MUST
            }
        }
    }


}

