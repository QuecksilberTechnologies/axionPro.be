// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes tenant roles using trusted request context.
// ================================================================

using axionpro.application.DTOs.Role;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to soft delete a tenant role.
    /// </summary>
    public class DeleteRoleQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRoleRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the delete request.
        /// </summary>
        public DeleteRoleQuery(DeleteRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles role deletion requests for the authenticated tenant.
    /// </summary>
    public class DeleteRoleQueryHandler : IRequestHandler<DeleteRoleQuery, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<DeleteRoleQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public DeleteRoleQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<DeleteRoleQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Soft deletes a role using trusted tenant and actor identifiers.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            DeleteRoleQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            long tenantId = validation.TenantId;
            long userEmployeeId = validation.LoggedInEmployeeId;
            int tokenRoleId = validation.RoleId;
            if (tenantId <= 0 || userEmployeeId <= 0 || tokenRoleId <= 0)
            {
                _logger.LogWarning("Invalid Tenant authorization context while deleting Role. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}", tenantId, userEmployeeId, tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            var permissionResult = await _unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(tenantId, userEmployeeId, tokenRoleId, request.DTO.ModuleId, request.DTO.OperationId, cancellationToken);
            TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException("Invalid role identifier.");

            // An inactive assignment or permission can be reactivated later, so only a
            // soft-deleted dependency is safe to ignore when deleting the Role.
            var hasDependencies = await _unitOfWork.RoleRepository
                .HasNonDeletedDependenciesAsync(request.DTO.Id, cancellationToken);

            if (hasDependencies)
            {
                _logger.LogWarning(
                    "Role deletion blocked because dependent UserRole or RoleModuleAndPermission records exist. RoleId: {RoleId}, TenantId: {TenantId}",
                    request.DTO.Id,
                    tenantId);

                throw new ConflictException(AppConstants.ErrorMessages.RoleHasDependencies);
            }

            var deleted = await _unitOfWork.RoleRepository.DeleteAsync(
                request.DTO.Id,
                tenantId,
                userEmployeeId,
                cancellationToken);
            if (!deleted)
                throw new ApiException("Role not found or already deleted.", 404);

            _logger.LogInformation("Role deleted. RoleId: {RoleId}", request.DTO.Id);
            return ApiResponse<bool>.Success(true, "Role deleted successfully.");
        }

        #endregion
    }

    #endregion
}
