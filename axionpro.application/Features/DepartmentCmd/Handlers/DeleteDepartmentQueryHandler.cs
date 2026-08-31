// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes a tenant-scoped department using trusted request context.
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
    #region Query

    /// <summary>
    /// Represents a request to soft delete a department.
    /// </summary>
    public class DeleteDepartmentQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteDepartmentRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the delete request.
        /// </summary>
        public DeleteDepartmentQuery(DeleteDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles tenant-scoped department deletion requests.
    /// </summary>
    public class DeleteDepartmentQueryHandler : IRequestHandler<DeleteDepartmentQuery, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<DeleteDepartmentQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public DeleteDepartmentQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<DeleteDepartmentQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Soft deletes a department only within the authenticated tenant.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            DeleteDepartmentQuery request,
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
                    "Invalid Tenant authorization context while deleting Department. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);

                throw new UnauthorizedAccessException(
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Runtime Permission Validation

            // Current database role assignments are authoritative so a stale
            // JWT role cannot authorize a department deletion.
            var permissionResult =
                await _unitOfWork.StoreProcedureRepository
                    .CheckTenantEmployeePermissionAsync(
                        tenantId,
                        userEmployeeId,
                        tokenRoleId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId,
                        cancellationToken);

            switch (permissionResult.ResultCode)
            {
                case 1:
                    break;

                case -1:
                    _logger.LogWarning(
                        "Tenant authorization context changed while deleting Department. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                        tenantId,
                        userEmployeeId,
                        tokenRoleId);
                    throw new UnauthorizedAccessException(
                        AppConstants.ErrorMessages.Unauthorized);

                case -2:
                    _logger.LogWarning(
                        "Invalid Tenant role context while deleting Department. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                        tenantId,
                        userEmployeeId,
                        tokenRoleId);
                    throw new UnauthorizedAccessException(
                        AppConstants.ErrorMessages.Unauthorized);

                case 0:
                default:
                    _logger.LogWarning(
                        "Department delete permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                        tenantId,
                        userEmployeeId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId);
                    throw new UnauthorizedAccessException(
                        AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidIdentifier);

            // An inactive employee can be reactivated, so only a soft-deleted Employee
            // is safe to ignore while validating Department deletion dependencies.
            var hasEmployees = await _unitOfWork.DepartmentRepository
                .HasNonDeletedEmployeesAsync(
                    request.DTO.Id,
                    tenantId,
                    cancellationToken);

            if (hasEmployees)
            {
                _logger.LogWarning(
                    "Department deletion blocked because non-soft-deleted employees are assigned. DepartmentId: {DepartmentId}, TenantId: {TenantId}",
                    request.DTO.Id,
                    tenantId);

                throw new ConflictException(AppConstants.ErrorMessages.DepartmentHasEmployees);
            }

            var isDeleted = await _unitOfWork.DepartmentRepository.DeleteAsync(
                request.DTO.Id,
                tenantId,
                userEmployeeId,
                cancellationToken);

            if (!isDeleted)
            {
                _logger.LogWarning("Department deletion failed. DepartmentId: {DepartmentId}", request.DTO.Id);
                throw new NotFoundException(
                    AppConstants.ErrorMessages.ResourceNotFound);
            }

            return ApiResponse<bool>.Success(true, "Department deleted successfully.");
        }

        #endregion
    }

    #endregion
}
