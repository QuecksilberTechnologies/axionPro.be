// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves tenant-scoped designation options using trusted context.
// ================================================================

using axionpro.application.DTOS.Designation;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to retrieve active designation options.
    /// </summary>
    public class GetDesignationOptionQuery : IRequest<ApiResponse<List<GetDesignationOptionResponseDTO>>>
    {
        public GetDesignationOptionRequestDTO OptionDTO { get; }

        /// <summary>
        /// Initializes the option query with client-supplied filters.
        /// </summary>
        public GetDesignationOptionQuery(GetDesignationOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles active designation-option queries for the authenticated tenant.
    /// </summary>
    public class GetDesignationOptionQueryHandler : IRequestHandler<GetDesignationOptionQuery, ApiResponse<List<GetDesignationOptionResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetDesignationOptionQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public GetDesignationOptionQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetDesignationOptionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves active options without using the request DTO as trusted context transport.
        /// </summary>
        public async Task<ApiResponse<List<GetDesignationOptionResponseDTO>>> Handle(
            GetDesignationOptionQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            var options = await _unitOfWork.DesignationRepository.GetOptionAsync(
                request.OptionDTO.DepartmentId,
                validation.TenantId,
                cancellationToken);

            _logger.LogInformation("Retrieved {Count} designation options for TenantId: {TenantId}", options.Count, validation.TenantId);
            return ApiResponse<List<GetDesignationOptionResponseDTO>>.Success(
                options,
                options.Count == 0 ? "No designations found for this tenant." : "Designation options fetched successfully.");
        }

        #endregion
    }

    #endregion
}
