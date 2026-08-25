// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates a tenant-scoped department using trusted request context.
// ================================================================

using axionpro.application.DTOs.Department;
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
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            if (request.DTO.Id <= 0)
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidIdentifier);

            if (string.IsNullOrWhiteSpace(request.DTO.DepartmentName))
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidRequest);

            // Load the existing tenant-owned entity to retain immutable and omitted values.
            var department = await _unitOfWork.DepartmentRepository.GetByIdForTenantAsync(
                request.DTO.Id,
                validation.TenantId,
                cancellationToken);

            if (department == null)
                throw new axionpro.application.Exceptions.NotFoundException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.ResourceNotFound);

            // Apply client-editable fields and server-controlled audit values.
            department.DepartmentName = request.DTO.DepartmentName.Trim();
            if (request.DTO.Description != null)
                department.Description = request.DTO.Description;
            if (request.DTO.Remark != null)
                department.Remark = request.DTO.Remark;
            if (request.DTO.IsActive.HasValue)
                department.IsActive = request.DTO.IsActive.Value;

            department.UpdatedById = validation.LoggedInEmployeeId;
            department.UpdatedDateTime = DateTime.UtcNow;

            // Persist the prepared domain entity.
            var isUpdated = await _unitOfWork.DepartmentRepository.UpdateAsync(department, cancellationToken);
            if (!isUpdated)
            {
                _logger.LogWarning("Department update failed. DepartmentId: {DepartmentId}", request.DTO.Id);
                throw new axionpro.application.Exceptions.ConflictException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.ResourceConflict);
            }

            _logger.LogInformation("Department updated successfully. DepartmentId: {DepartmentId}", request.DTO.Id);
            return ApiResponse<bool>.Success(true, "Department updated successfully.");
        }

        #endregion
    }

    #endregion
}
