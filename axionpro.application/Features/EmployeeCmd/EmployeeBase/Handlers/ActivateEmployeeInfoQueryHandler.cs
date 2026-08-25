// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles ActivateEmployeeInfoQueryHandler requests using authenticated tenant context.
// ================================================================

using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;


namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{

    #region Query

    /// <summary>
    /// Represents the ActivateAllEmployeeQuery application component.
    /// </summary>
    public class ActivateAllEmployeeQuery : IRequest<ApiResponse<bool>>
    {
      public ActivateAllEmployeeRequestDTO DTO;

        public ActivateAllEmployeeQuery(ActivateAllEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    /// <summary>
    /// Handles authenticated tenant requests for this feature.
    /// </summary>
        #endregion

    #region Handler

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
                _logger.LogInformation("ActivateAllEmployee started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                #region Tenant Request Validation
                var validation = await _commonRequestService
                    .ValidateTenantUserRequestAsync();
                #endregion

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");
                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (YOUR FIXED PATTERN )
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update employee status.");

                // ===============================
                // 4️⃣ FETCH EMPLOYEE
                // ===============================
                var employee = await _unitOfWork.Employees
                    .GetByIdAsync(
                        employeeId,
                        validation.TenantId,
                        true);

                if (employee == null)
                    throw new ApiException("Employee not found.", 404);

                // ===============================
                // 5️⃣ ACTIVATE / DEACTIVATE
                // ===============================
                var isSuccess = await _unitOfWork.Employees
                    .ActivateAllEmployeeAsync(employee, request.DTO.IsActive);

                if (!isSuccess)
                    throw new ApiException("Failed to update employee status.", 500);

                _logger.LogInformation("ActivateAllEmployee success | Id: {Id}", employeeId);

                // ===============================
                // 6️⃣ SUCCESS RESPONSE
                // ===============================
                return ApiResponse<bool>.Success(
                    true,
                    request.DTO.IsActive
                        ? "Employee activated successfully."
                        : "Employee deactivated successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating employee status | Id: {Id}",
                    request.DTO?.EmployeeId);

                throw; //  MUST
            }
        }
    }



    #endregion
}


