// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant roles using trusted request context.
// ================================================================

using axionpro.application.DTOs.Role;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to update a tenant role.
    /// </summary>
    public class UpdateRoleCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateRoleRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the update request.
        /// </summary>
        public UpdateRoleCommand(UpdateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles role updates for the authenticated tenant.
    /// </summary>
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public UpdateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Updates client-editable role values while preserving tenant and creation audit fields.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            UpdateRoleCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            if (request.DTO.Id <= 0 || string.IsNullOrWhiteSpace(request.DTO.RoleName))
                throw new ValidationErrorException("A valid role identifier and name are required.");

            var entity = await _unitOfWork.RoleRepository.GetByIdForTenantAsync(
                request.DTO.Id,
                validation.TenantId,
                cancellationToken);
            if (entity == null)
                throw new ApiException("Role not found.", 404);

            entity.RoleName = request.DTO.RoleName.Trim();
            entity.RoleType = request.DTO.RoleType;
            entity.Remark = request.DTO.Remark;
            if (request.DTO.IsActive.HasValue)
                entity.IsActive = request.DTO.IsActive.Value;
            entity.UpdatedById = validation.LoggedInEmployeeId;
            entity.UpdatedDateTime = DateTime.UtcNow;

            var updated = await _unitOfWork.RoleRepository.UpdateAsync(entity, cancellationToken);
            if (!updated)
                throw new ApiException("Role was not updated.", 409);

            _logger.LogInformation("Role updated. RoleId: {RoleId}", entity.Id);
            return ApiResponse<bool>.Success(true, "Role updated successfully.");
        }

        #endregion
    }

    #endregion
}
