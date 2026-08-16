// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes tenant-scoped designations using trusted request context.
// ================================================================

using axionpro.application.DTOs.Designation;
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
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            if (request.DTO.Id <= 0)
                throw new axionpro.application.Exceptions.ValidationErrorException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.InvalidIdentifier);

            var deleted = await _unitOfWork.DesignationRepository.DeleteDesignationAsync(
                request.DTO.Id,
                validation.TenantId,
                validation.LoggedInEmployeeId,
                cancellationToken);

            if (!deleted)
                throw new axionpro.application.Exceptions.NotFoundException(
                    axionpro.application.Constants.AppConstants.ErrorMessages.ResourceNotFound);

            _logger.LogInformation("Designation deleted. DesignationId: {DesignationId}", request.DTO.Id);
            return ApiResponse<bool>.Success(true, "Designation deleted successfully.");
        }

        #endregion
    }

    #endregion
}
