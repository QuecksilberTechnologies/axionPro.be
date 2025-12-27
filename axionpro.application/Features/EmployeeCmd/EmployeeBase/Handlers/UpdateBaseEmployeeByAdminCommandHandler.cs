using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                /* =====================================================
                   1️⃣ COMMON VALIDATION
                   ===================================================== */
                var validation =
                    await _commonRequestService.ValidateRequestAsync(
                        request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                /* =====================================================
                   2️⃣ FETCH EXISTING EMPLOYEE
                   ===================================================== */
                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.TenantId,
                    true);

                if (employee == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Employee not found.");
                }

                /* =====================================================
                   3️⃣ PATCH UPDATE
                   (DTO value → overwrite
                    DTO null  → keep existing)
                   ===================================================== */

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

                if (!string.IsNullOrWhiteSpace(request.DTO.EmergencyContactNumber))
                    employee.EmergencyContactNumber =
                        request.DTO.EmergencyContactNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.MobileNumber))
                    employee.MobileNumber = request.DTO.MobileNumber;

                if (!string.IsNullOrWhiteSpace(request.DTO.BloodGroup))
                    employee.BloodGroup = request.DTO.BloodGroup;

                if (request.DTO.IsMarried.HasValue)
                    employee.IsMarried = request.DTO.IsMarried.Value;

                /* ---------- INFO VERIFIED (SAFE RESET) ---------- */
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

                /* =====================================================
                   4️⃣ AUDIT
                   ===================================================== */
                employee.UpdatedById = validation.UserEmployeeId;
                employee.UpdatedDateTime = DateTime.UtcNow;

                /* =====================================================
                   5️⃣ SAVE
                   ===================================================== */
                await _unitOfWork.Employees.UpdateEmployeeAsync(
                    employee,
                    request.DTO.Prop.TenantId);

                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.Success(
                    true,
                    "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating employee");

                return ApiResponse<bool>.Fail(
                    "Unexpected error occurred.",
                    new List<string> { ex.Message });
            }
        }
    }


}


