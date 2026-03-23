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


    public class UpdateBaseEmployeeByAdminCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeRequestOfficialDTO DTO { get; }

        public UpdateBaseEmployeeByAdminCommand(UpdateEmployeeRequestOfficialDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateBaseEmployeeByAdminCommandHandler
        : IRequestHandler<UpdateBaseEmployeeByAdminCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBaseEmployeeByAdminCommandHandler> _logger;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateBaseEmployeeByAdminCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateBaseEmployeeByAdminCommandHandler> logger,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(
      UpdateBaseEmployeeByAdminCommand request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UpdateBaseEmployeeByAdmin started");

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

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (ADMIN UPDATE)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update employee.");

                // ===============================
                // 4️⃣ FETCH EXISTING
                // ===============================
                var employee =
                    await _unitOfWork.Employees.GetByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId,
                        true);

                if (employee == null)
                    throw new ApiException("Employee not found.", 404);

                // ===============================
                // 5️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 6️⃣ PATCH UPDATE
                // ===============================
                if (!string.IsNullOrWhiteSpace(request.DTO.FirstName))
                    employee.FirstName = request.DTO.FirstName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.MiddleName))
                    employee.MiddleName = request.DTO.MiddleName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.LastName))
                    employee.LastName = request.DTO.LastName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.Description))
                    employee.Description = request.DTO.Description.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.Remark))
                    employee.Remark = request.DTO.Remark.Trim();

                if (request.DTO.DesignationId.HasValue)
                    employee.DesignationId = request.DTO.DesignationId.Value;

                if (request.DTO.DepartmentId.HasValue)
                    employee.DepartmentId = request.DTO.DepartmentId.Value;

                if (request.DTO.EmployeeTypeId.HasValue)
                    employee.EmployeeTypeId = request.DTO.EmployeeTypeId.Value;

                if (request.DTO.CountryId.HasValue)
                    employee.CountryId = request.DTO.CountryId.Value;

                if (request.DTO.GenderId.HasValue)
                    employee.GenderId = request.DTO.GenderId.Value;

                if (request.DTO.DateOfBirth.HasValue)
                    employee.DateOfBirth = request.DTO.DateOfBirth.Value;

                if (request.DTO.DateOfOnBoarding.HasValue)
                    employee.DateOfOnBoarding = request.DTO.DateOfOnBoarding.Value;

                if (request.DTO.DateOfExit.HasValue)
                    employee.DateOfExit = request.DTO.DateOfExit.Value;

                if (request.DTO.HasPermanent.HasValue)
                    employee.HasPermanent = request.DTO.HasPermanent.Value;

                if (request.DTO.IsActive.HasValue)
                    employee.IsActive = request.DTO.IsActive.Value;

                if (request.DTO.IsEditAllowed.HasValue)
                    employee.IsEditAllowed = request.DTO.IsEditAllowed.Value;

                if (request.DTO.Relation.HasValue)
                    employee.Relation = request.DTO.Relation.Value;

                if (!string.IsNullOrWhiteSpace(request.DTO.EmergencyContactPerson))
                    employee.EmergencyContactPerson =
                        request.DTO.EmergencyContactPerson;

                if (!string.IsNullOrWhiteSpace(request.DTO.EmergencyContactNumber))
                    employee.EmergencyContactNumber =
                        request.DTO.EmergencyContactNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.MobileNumber))
                    employee.MobileNumber = request.DTO.MobileNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.BloodGroup))
                    employee.BloodGroup = request.DTO.BloodGroup;

                if (request.DTO.IsMarried.HasValue)
                    employee.IsMarried = request.DTO.IsMarried.Value;

                // ===============================
                // INFO VERIFIED
                // ===============================
                if (request.DTO.IsInfoVerified.HasValue)
                {
                    employee.IsInfoVerified =
                        request.DTO.IsInfoVerified.Value;

                    if (request.DTO.IsInfoVerified.Value)
                    {
                        employee.InfoVerifiedById =
                            validation.UserEmployeeId;
                        employee.InfoVerifiedDateTime =
                            DateTime.UtcNow;
                    }
                    else
                    {
                        employee.InfoVerifiedById = null;
                        employee.InfoVerifiedDateTime = null;
                    }
                }

                // ===============================
                // 7️⃣ AUDIT
                // ===============================
                employee.UpdatedById = validation.UserEmployeeId;
                employee.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 8️⃣ SAVE
                // ===============================
                await _unitOfWork.Employees.UpdateEmployeeAsync(
                    employee,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("UpdateBaseEmployeeByAdmin success");

                return ApiResponse<bool>.Success(
                    true,
                    "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error updating employee");

                throw; // 🚨 MUST
            }
        }
    }


}


