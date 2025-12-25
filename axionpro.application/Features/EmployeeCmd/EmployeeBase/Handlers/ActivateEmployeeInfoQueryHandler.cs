using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class ActivateAllEmployeeQuery : IRequest<ApiResponse<bool>>
    {
      public ActivateAllEmployeeRequestDTO DTO;

        public ActivateAllEmployeeQuery(ActivateAllEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class ActivateEmployeeInfoQueryHandler
    : IRequestHandler<ActivateAllEmployeeQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivateEmployeeInfoQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IPermissionService _permissionService;

        public ActivateEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<ActivateEmployeeInfoQueryHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
            ActivateAllEmployeeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 1. COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // 🔹 2. Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService
                    );

                // 🔹 3. Permission Check (optional but clean)
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("ActivateEmployee"))
                {
                  //  return ApiResponse<bool>.Fail("You do not have permission to update employee status.");

                }

                // 🔹 4. Fetch Employee
                var employee = await _unitOfWork.Employees
                    .GetByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId,
                        true);

                if (employee == null)
                    return ApiResponse<bool>.Fail("Employee not found for current tenant.");

                // 🔹 5. Activate / Deactivate
                var isSuccess = await _unitOfWork.Employees
                    .ActivateAllEmployeeAsync(employee, request.DTO.IsActive);

                if (!isSuccess)
                    return ApiResponse<bool>.Fail("Failed to update employee status.");

                // 🔹 6. Success Response
                return ApiResponse<bool>.Success(
                    isSuccess,
                    request.DTO.IsActive
                        ? "Employee activated successfully."
                        : "Employee deactivated successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while activating/deactivating employee.");

                return ApiResponse<bool>.Fail(
                    "Something went wrong while updating employee status.",
                    new List<string> { ex.Message }
                );
            }
        }
    }



}


