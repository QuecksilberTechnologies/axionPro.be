// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates a tenant-scoped department using trusted request context.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to update a department.
    /// </summary>
    public class UpdateDepartmentCommad : IRequest<ApiResponse<bool>>
    {
        public UpdateDepartmentRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the update request with client-editable department values.
        /// </summary>
        public UpdateDepartmentCommad(UpdateDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles updates to departments owned by the authenticated tenant.
    /// </summary>
    public class UpdateDepartmentCommadHandler : IRequestHandler<UpdateDepartmentCommad, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateDepartmentCommadHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public UpdateDepartmentCommadHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<UpdateDepartmentCommadHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Updates client-editable department fields after enforcing tenant ownership.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            UpdateDepartmentCommad request,
            CancellationToken cancellationToken)
        {
            #region Tenant Request Validation

            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(
                    validation.ErrorMessage ??
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Trusted Request Context

            long userEmployeeId = validation.LoggedInEmployeeId;
            long tenantId = validation.TenantId;
            int tokenRoleId = validation.RoleId;

            if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
            {
                _logger.LogWarning(
                    "Invalid Tenant authorization context while updating Department. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);

                throw new UnauthorizedAccessException(
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidIdentifier);

            if (string.IsNullOrWhiteSpace(request.DTO.DepartmentName))
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidRequest);

            // Load the existing tenant-owned entity to retain immutable and omitted values.
            var department = await _unitOfWork.DepartmentRepository.GetByIdForTenantAsync(
                request.DTO.Id,
                tenantId,
                cancellationToken);

            if (department == null)
                throw new NotFoundException(
                    AppConstants.ErrorMessages.ResourceNotFound);

            // Inactive employees remain a dependency because they can later be
            // reactivated. Only employees already soft deleted are ignored.
            if (department.IsActive && request.DTO.IsActive == false &&
                await _unitOfWork.DepartmentRepository.HasNonDeletedEmployeesAsync(
                    department.Id,
                    tenantId,
                    cancellationToken))
            {
                _logger.LogWarning(
                    "Department deactivation blocked because non-soft-deleted employees are assigned. DepartmentId: {DepartmentId}, TenantId: {TenantId}",
                    department.Id,
                    tenantId);

                throw new ConflictException(
                    AppConstants.ErrorMessages.DepartmentHasEmployees,
                    AppConstants.ErrorCodes.DepartmentHasEmployeeDependencies);
            }

            // Apply client-editable fields and server-controlled audit values.
            department.DepartmentName = request.DTO.DepartmentName.Trim();
            if (request.DTO.Description != null)
                department.Description = request.DTO.Description;
            if (request.DTO.Remark != null)
                department.Remark = request.DTO.Remark;
            if (request.DTO.IsActive.HasValue)
                department.IsActive = request.DTO.IsActive.Value;

            department.UpdatedById = userEmployeeId;
            department.UpdatedDateTime = DateTime.UtcNow;

            // Persist the prepared domain entity.
            var isUpdated = await _unitOfWork.DepartmentRepository.UpdateAsync(department, cancellationToken);
            if (!isUpdated)
            {
                _logger.LogWarning("Department update failed. DepartmentId: {DepartmentId}", request.DTO.Id);
                throw new ConflictException(
                    AppConstants.ErrorMessages.ResourceConflict);
            }

            _logger.LogInformation("Department updated successfully. DepartmentId: {DepartmentId}", request.DTO.Id);
            return ApiResponse<bool>.Success(true, "Department updated successfully.");
        }

        #endregion
    }

    #endregion
}
