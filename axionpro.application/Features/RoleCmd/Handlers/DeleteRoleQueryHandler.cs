// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes tenant roles using trusted request context.
// ================================================================

using axionpro.application.DTOs.Role;
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
            switch (permissionResult.ResultCode)
            {
                case 1: break;
                case -1:
                    _logger.LogWarning("Tenant authorization context changed while deleting Role. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}", tenantId, userEmployeeId, tokenRoleId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
                case -2:
                    _logger.LogWarning("Invalid Tenant role context while deleting Role. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}", tenantId, userEmployeeId, tokenRoleId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
                case 0:
                default:
                    _logger.LogWarning("Role deletion permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}", tenantId, userEmployeeId, request.DTO.ModuleId, request.DTO.OperationId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException("Invalid role identifier.");

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
