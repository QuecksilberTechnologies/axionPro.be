using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Token;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

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
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Common validation
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<GetBaseEmployeeResponseDTO>
                        .Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 2️⃣ Permission check (optional)
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                // 3️⃣ Check existing login
                var existingUser = await _unitOfWork.UserLoginRepository
                    .GetEmployeeIdByUserLogin(request.DTO.OfficialEmail);

                if (existingUser != null)
                    return ApiResponse<GetBaseEmployeeResponseDTO>
                        .Fail("User already exists.");
                var existingEmployee = await _unitOfWork.TenantEmployeeCodePatternRepository.GetTenantEmployeeCodePatternAsync(
                    validation.TenantId, true);
             

                // 3️⃣ Map Employee
                var employee = _mapper.Map<Employee>(request.DTO);
                employee.TenantId = validation.TenantId;
                employee.AddedById = validation.UserEmployeeId;
                employee.AddedDateTime = DateTime.UtcNow;
                employee.DateOfOnBoarding = DateTime.UtcNow;
                employee.IsActive = true;
                employee.IsInfoVerified = false;
                employee.IsEditAllowed = true;
                employee.IsSoftDeleted = false;
                employee.Remark = "Initial info created during employee creation";

                // 4️⃣ Generate Employee Code (FINAL ANSWER)
                employee.EmployementCode =
                    await _unitOfWork.TenantEmployeeCodePatternRepository
                        .GenerateEmployeeCodeAsync(validation.TenantId, employee.DepartmentId);



                // 5️⃣ LoginCredential
                var loginCredential = new LoginCredential
                {
                    TenantId = employee.TenantId,
                    LoginId = request.DTO.OfficialEmail,
                    Password = null,
                    HasFirstLogin = true,
                    IsPasswordChangeRequired = true,
                    IsActive = true,
                    IsSoftDeleted = false,
                    AddedById = employee.AddedById,
                    AddedDateTime = DateTime.UtcNow,
                    Employee = employee,
                    Remark = "Initial login credential created during employee creation"
                };

                // 6️⃣ Default Role
                if (request.DTO.RoleId <= 0)
                {
                    var roles = await _unitOfWork.RoleRepository
                        .GetRoleAsync(validation.TenantId,
                                      ConstantValues.RoleTypeEmployee,
                                      true);

                    var defaultRole = roles.FirstOrDefault();
                    if (defaultRole == null)
                        return ApiResponse<GetBaseEmployeeResponseDTO>
                            .Fail("Default employee role not configured.");

                    request.DTO.RoleId = defaultRole.Id;
                }

                var userRole = new UserRole
                {
                    Employee = employee,
                    RoleId = request.DTO.RoleId,
                    RoleStartDate = DateTime.UtcNow,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false,
                    IsPrimaryRole = true,
                    AssignedById = validation.UserEmployeeId,
                    AssignedDateTime = DateTime.UtcNow,
                    ApprovalRequired = false,
                    ApprovalStatus = "Approved",
                    Remark = "Initial role assignment during employee creation"
                };

                // 7️⃣ Save + COMMIT (🔥 ONLY DB)
                var savedEmployee = await _unitOfWork.Employees
                    .CreateEmployeeAsync(employee, loginCredential, userRole);



                await _unitOfWork.CommitTransactionAsync();
                // 7️⃣ Response DTO
                var responseDto =
                    ProjectionHelper.ToGetBaseInfoResponseDTO(
                        savedEmployee,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey);            


                // 9️⃣ Generate Set-Password Token (AS-IT-IS)
                var getTokenInfoDTO = new GetTokenInfoDTO
                {
                    EmployeeId = responseDto.Id,
                    Email = savedEmployee.OfficialEmail!,
                    FullName = $"{savedEmployee.FirstName} {savedEmployee.LastName}",
                    TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, ""),
                    IssuedAt = DateTime.UtcNow,
                    Expiry = DateTime.UtcNow.AddMinutes(30),
                    IsFirstLogin = true,
                    ClientType = "Web"

                };
                string token = await _tokenService.GenerateToken(getTokenInfoDTO);
                // 9️⃣ EMAIL (🔥 OUTSIDE TRANSACTION, FAILURE ≠ API FAILURE)

                await _unitOfWork.CommitTransactionAsync(); // DB ka kaam complete

                try
                {
                    string baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? string.Empty;
                    //    var emailService = _scopeFactory.CreateScope().ServiceProvider
                    //  .GetRequiredService<IEmailService>();

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


                return ApiResponse<GetBaseEmployeeResponseDTO>
                    .Success(responseDto, "Employee created successfully further process please check mail and click the link!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Employee creation failed");

                return ApiResponse<GetBaseEmployeeResponseDTO>
                    .Fail("Failed to create employee.");
            }
        }

    }


}

