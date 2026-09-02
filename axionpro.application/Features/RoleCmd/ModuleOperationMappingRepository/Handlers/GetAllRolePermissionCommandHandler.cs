// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles retrieval of module-operation permissions for a role.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to retrieve module-operation permissions for a role.
    /// </summary>
    public class GetRolePermissionCommand : IRequest<ApiResponse<List<RoleModuleAndPermission>>>
    {
        public GetAllActiveRoleModuleOperationsRequestByRoleIdDTO DTO { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolePermissionCommand"/> class.
        /// </summary>
        public GetRolePermissionCommand(GetAllActiveRoleModuleOperationsRequestByRoleIdDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Retrieves module-operation permissions for a validated tenant role.
    /// </summary>
    public class GetRolePermissionCommandHandler
        : IRequestHandler<GetRolePermissionCommand, ApiResponse<List<RoleModuleAndPermission>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRolePermissionCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolePermissionCommandHandler"/> class.
        /// </summary>
        public GetRolePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetRolePermissionCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves the existing module-operation assignments for the requested role.
        /// </summary>
        public async Task<ApiResponse<List<RoleModuleAndPermission>>> Handle(
            GetRolePermissionCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.DTO == null || request.DTO.RoleId <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            var role = await _unitOfWork.RoleRepository.GetByIdForTenantAsync(
                request.DTO.RoleId,
                validation.TenantId,
                cancellationToken);
            if (role == null)
            {
                throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            }

            var permissions = await _unitOfWork
                .UserRolesPermissionOnModuleRepository
                .GetByRoleIdAsync(
                    request.DTO.RoleId,
                    validation.TenantId,
                    cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} role permissions for tenant {TenantId}, role {RoleId}.",
                permissions.Count,
                validation.TenantId,
                request.DTO.RoleId);

            return ApiResponse<List<RoleModuleAndPermission>>.Success(
                permissions,
                AppConstants.SuccessMessages.RolePermissionsRetrieved);
        }

        #endregion
    }

    #endregion
}
