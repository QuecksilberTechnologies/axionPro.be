// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles assignment of module operations to a tenant role.
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
    #region Command

    /// <summary>
    /// Represents a request to assign module-operation permissions to a role.
    /// </summary>
    public class CreateRolePermissionCommand : IRequest<ApiResponse<int>>
    {
        public CreateModuleOperationRolePermissionsRequestDTO DTO { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRolePermissionCommand"/> class.
        /// </summary>
        public CreateRolePermissionCommand(CreateModuleOperationRolePermissionsRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Applies module-operation permission assignments for a tenant role.
    /// </summary>
    public class CreateRolePermissionCommandHandler
        : IRequestHandler<CreateRolePermissionCommand, ApiResponse<int>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRolePermissionCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRolePermissionCommandHandler"/> class.
        /// </summary>
        public CreateRolePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateRolePermissionCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Synchronizes the submitted module-operation assignments for the selected role.
        /// </summary>
        public async Task<ApiResponse<int>> Handle(
            CreateRolePermissionCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.DTO == null || request.DTO.RoleId <= 0 || request.DTO.ModuleOperations == null)
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

            var existingPermissions = await _unitOfWork
                .UserRolesPermissionOnModuleRepository
                .GetByRoleIdAsync(
                    request.DTO.RoleId,
                    validation.TenantId,
                    cancellationToken);
            var existingSet = existingPermissions
                .Select(x => (x.ModuleId, x.OperationId))
                .ToHashSet();
            var toInsert = new List<RoleModuleAndPermission>();
            var toDelete = new List<RoleModuleAndPermission>();

            foreach (var module in request.DTO.ModuleOperations)
            {
                if (module.Operations == null)
                {
                    continue;
                }

                foreach (var operation in module.Operations)
                {
                    var key = (module.ModuleId, operation.OperationId);
                    if (operation.HasAccess && !existingSet.Contains(key))
                    {
                        toInsert.Add(new RoleModuleAndPermission
                        {
                            RoleId = request.DTO.RoleId,
                            ModuleId = module.ModuleId,
                            OperationId = operation.OperationId,
                            HasAccess = true,
                            IsActive = true,
                            AddedById = validation.LoggedInEmployeeId,
                            AddedDateTime = DateTime.UtcNow,
                            Remark = "Assigned via role permission UI"
                        });
                    }
                    else if (!operation.HasAccess)
                    {
                        var existing = existingPermissions.FirstOrDefault(x =>
                            x.ModuleId == module.ModuleId &&
                            x.OperationId == operation.OperationId);
                        if (existing != null)
                        {
                            toDelete.Add(existing);
                        }
                    }
                }
            }

            if (toInsert.Count > 0)
            {
                await _unitOfWork.UserRolesPermissionOnModuleRepository.BulkInsertAsync(toInsert);
            }

            if (toDelete.Count > 0)
            {
                await _unitOfWork.UserRolesPermissionOnModuleRepository.BulkDeleteAsync(toDelete);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var affectedCount = toInsert.Count + toDelete.Count;
            _logger.LogInformation(
                "Updated role permissions for tenant {TenantId}, role {RoleId}: {Count} changes.",
                validation.TenantId,
                request.DTO.RoleId,
                affectedCount);

            return ApiResponse<int>.Success(
                affectedCount,
                AppConstants.SuccessMessages.RolePermissionsUpdated);
        }

        #endregion
    }

    #endregion
}
