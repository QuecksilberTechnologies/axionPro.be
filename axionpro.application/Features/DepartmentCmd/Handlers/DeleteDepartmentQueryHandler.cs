// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes a tenant-scoped department using trusted request context.
// ================================================================

using axionpro.application.DTOs.Department;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
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
        private readonly IPermissionService _permissionService;
        private readonly ILogger<DeleteDepartmentQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public DeleteDepartmentQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            ILogger<DeleteDepartmentQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
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
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            if (request.DTO.Id <= 0)
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidIdentifier);

            // Retain the existing permission lookup without changing authorization behavior.
            var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
            if (!permissions.Contains("AddBankInfo"))
                await _unitOfWork.RollbackTransactionAsync();

            var isDeleted = await _unitOfWork.DepartmentRepository.DeleteAsync(
                request.DTO.Id,
                validation.TenantId,
                validation.LoggedInEmployeeId,
                cancellationToken);

            if (!isDeleted)
            {
                _logger.LogWarning("Department deletion failed. DepartmentId: {DepartmentId}", request.DTO.Id);
                throw new axionpro.application.Exceptions.NotFoundException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.ResourceNotFound);
            }

            return ApiResponse<bool>.Success(true, "Department deleted successfully.");
        }

        #endregion
    }

    #endregion
}
