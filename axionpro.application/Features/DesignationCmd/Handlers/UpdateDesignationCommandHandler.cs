// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-scoped designations using trusted request context.
// ================================================================

using axionpro.application.DTOs.Designation;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to update a designation.
    /// </summary>
    public class UpdateDesignationCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateDesignationRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the update request.
        /// </summary>
        public UpdateDesignationCommand(UpdateDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles updates to designations owned by the authenticated tenant.
    /// </summary>
    public class UpdateDesignationCommandHandler : IRequestHandler<UpdateDesignationCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateDesignationCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public UpdateDesignationCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<UpdateDesignationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Updates client-editable fields while retaining tenant and creation audit values.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(
            UpdateDesignationCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                return ApiResponse<bool>.Fail(validation.ErrorMessage);

            if (request.DTO.Id <= 0)
                return ApiResponse<bool>.Fail("Invalid designation identifier.");

            var entity = await _unitOfWork.DesignationRepository.GetByIdForTenantAsync(
                request.DTO.Id,
                validation.TenantId,
                cancellationToken);
            if (entity == null)
                return ApiResponse<bool>.Fail("Designation not found.");

            if (!string.IsNullOrWhiteSpace(request.DTO.DesignationName))
                entity.DesignationName = request.DTO.DesignationName.Trim();
            if (request.DTO.DepartmentId > 0)
                entity.DepartmentId = request.DTO.DepartmentId;
            if (request.DTO.Description != null)
                entity.Description = request.DTO.Description;
            if (request.DTO.IsActive.HasValue)
                entity.IsActive = request.DTO.IsActive.Value;

            entity.UpdatedById = validation.LoggedInEmployeeId;
            entity.UpdatedDateTime = DateTime.UtcNow;

            var updated = await _unitOfWork.DesignationRepository.UpdateDesignationAsync(entity, cancellationToken);
            if (!updated)
                return ApiResponse<bool>.Fail("Designation update failed.");

            _logger.LogInformation("Designation updated. DesignationId: {DesignationId}", entity.Id);
            return ApiResponse<bool>.Success(true, "Designation updated successfully.");
        }

        #endregion
    }

    #endregion
}
