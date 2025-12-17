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
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public CreateBaseEmployeeInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateBaseEmployeeInfoCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<GetBaseEmployeeResponseDTO>> Handle(CreateBaseEmployeeInfoCommand request,CancellationToken cancellationToken)
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

                // 2️⃣ Permission check
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddEmployee"))
                {
                   // return ApiResponse<GetBaseEmployeeResponseDTO>
                    //    .Fail("You do not have permission to add employee.");
                }

                // 3️⃣ Check existing login
                var existingUser =
                    await _unitOfWork.UserLoginRepository
                        .GetEmployeeIdByUserLogin(request.DTO.OfficialEmail);

                if (existingUser != null)
                    return ApiResponse<GetBaseEmployeeResponseDTO>
                        .Fail("User already exists.");

                // 4️⃣ Map Employee
                var employee = _mapper.Map<Employee>(request.DTO);
                employee.TenantId = request.DTO.Prop.TenantId;
                employee.AddedById = request.DTO.Prop.UserEmployeeId;
                employee.AddedDateTime = DateTime.UtcNow;              
                employee.Remark = "Initial info created during employee creation";
                employee.IsActive = true;
                employee.IsInfoVerified = false;
                employee.IsEditAllowed = true;
                employee.IsSoftDeleted = false;
                employee.DateOfOnBoarding = DateTime.UtcNow;


                employee.EmployementCode = $"EMP-{employee.TenantId}-{DateTime.UtcNow:yy/MM/dd}-{Random.Shared.Next(1000, 9999)}";

                // 5️⃣ Create LoginCredential (FK relation)
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
                    Employee = employee, // 🔥 FK handled automatically
                    Remark = "Initial login credential created during employee creation",
                };

                if (request.DTO.RoleId <= 0)
                {
                    var roleinfo = await _unitOfWork.RoleRepository
                        .GetRoleAsync(request.DTO.Prop.TenantId,
                                      ConstantValues.RoleTypeEmployee,
                                      true);

                    var defaultRole = roleinfo.FirstOrDefault();

                    if (defaultRole == null)
                        return ApiResponse<GetBaseEmployeeResponseDTO>
                            .Fail("Default employee role not configured.");

                    request.DTO.RoleId = defaultRole.Id;
                }


                UserRole userRole = new UserRole
                {
                    Employee = employee,
                    RoleId = request.DTO.RoleId,   
                    RoleStartDate = DateTime.UtcNow,
                    AddedById =request.DTO.Prop.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false,
                    IsPrimaryRole = true,
                    AssignedById = request.DTO.Prop.UserEmployeeId,
                    AssignedDateTime = DateTime.UtcNow,
                    ApprovalRequired = false,
                    ApprovalStatus = "Approved",
                    Remark = "Initial role assignment during employee creation",
                    



                };


                // 6️⃣ Save
                var savedEmployee =  await _unitOfWork.Employees.CreateEmployeeAsync(employee, loginCredential, userRole);

                await _unitOfWork.CommitTransactionAsync();
                // 5️⃣ Encrypt Result Data
                var encryptedList = ProjectionHelper.ToGetBaseInfoResponseDTO(savedEmployee, _idEncoderService, validation.Claims.TenantEncriptionKey);


                // 7️⃣ Response
                return ApiResponse<GetBaseEmployeeResponseDTO>.Success(
                   encryptedList,
                    "Employee created successfully."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while creating employee");

                return ApiResponse<GetBaseEmployeeResponseDTO>
                    .Fail("Failed to create employee.");
            }
        }
    }


}


