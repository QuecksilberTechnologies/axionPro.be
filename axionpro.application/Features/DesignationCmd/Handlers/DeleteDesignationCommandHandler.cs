// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes tenant-scoped designations using trusted request context.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.Designation;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to soft delete a designation.
    /// </summary>
    public class DeleteDesignationQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteDesignationRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the delete request.
        /// </summary>
        public DeleteDesignationQuery(DeleteDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles soft-deletion requests for designations owned by the authenticated tenant.
    /// </summary>
    public class DeleteDesignationQueryHandler : IRequestHandler<DeleteDesignationQuery, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<DeleteDesignationQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public DeleteDesignationQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<DeleteDesignationQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Soft deletes a designation using trusted tenant and actor identifiers.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            DeleteDesignationQuery request,
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
                    "Invalid Tenant authorization context while deleting Designation. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);

                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Runtime Permission Validation

            // Current database role assignments are authoritative so a stale
            // JWT role cannot authorize a designation deletion.
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
                        "Tenant authorization context changed while deleting Designation. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                        tenantId,
                        userEmployeeId,
                        tokenRoleId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);

                case -2:
                    _logger.LogWarning(
                        "Invalid Tenant role context while deleting Designation. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                        tenantId,
                        userEmployeeId,
                        tokenRoleId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);

                case 0:
                default:
                    _logger.LogWarning(
                        "Designation delete permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                        tenantId,
                        userEmployeeId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidIdentifier);

            var deleted = await _unitOfWork.DesignationRepository.DeleteDesignationAsync(
                request.DTO.Id,
                tenantId,
                userEmployeeId,
                cancellationToken);

            if (!deleted)
                throw new NotFoundException(
                    AppConstants.ErrorMessages.ResourceNotFound);

            _logger.LogInformation("Designation deleted. DesignationId: {DesignationId}", request.DTO.Id);
            return ApiResponse<bool>.Success(true, "Designation deleted successfully.");
        }

        #endregion
    }

    #endregion
}
