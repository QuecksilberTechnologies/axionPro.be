// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines and handles retrieval of tenant Parent/Header Modules.
// ============================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Query

    /// <summary>
    /// Represents a read-only request for the authenticated tenant's Parent/Header Modules.
    /// </summary>
    public class GetParentModulesQuery : IRequest<ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModulesQuery"/> class.
        /// </summary>
        /// <param name="isActive">When supplied, filters results by active state.</param>
        public GetParentModulesQuery(bool? isActive)
        {
            IsActive = isActive;
        }

        /// <summary>
        /// Gets the optional active-state filter.
        /// </summary>
        public bool? IsActive { get; }
    }

    #endregion

    /// <summary>
    /// Handles Parent/Header Module list retrieval with tenant and scope isolation.
    /// </summary>
    public class GetParentModulesQueryHandler : IRequestHandler<GetParentModulesQuery, ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetParentModulesQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModulesQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="commonRequestService">Validates the authenticated tenant context.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public GetParentModulesQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetParentModulesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Retrieves Parent/Header Modules, including both states when no active-state filter is supplied.
        /// </summary>
        /// <param name="request">The read-only list request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The ordered Header Module list, which may be empty.</returns>
        public async Task<ApiResponse<List<GetParentModuleResponseDTO>>> Handle(
            GetParentModulesQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return ApiResponse<List<GetParentModuleResponseDTO>>.Fail("Parent Module request data is required.");
            }

            var context = await _commonRequestService.ValidateRequestAsync();
            if (!context.Success)
            {
                return ApiResponse<List<GetParentModuleResponseDTO>>.Fail(context.ErrorMessage);
            }

            try
            {
                var modules = await _unitOfWork.ModuleRepository.GetParentModulesAsync(
                    context.TenantId,
                    (short)AppConstants.TenantModuleScope,
                    request.IsActive,
                    cancellationToken);

                return ApiResponse<List<GetParentModuleResponseDTO>>.Success(
                    modules,
                    "Parent Modules retrieved successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to retrieve Parent Modules for tenant {TenantId}.", context.TenantId);
                return ApiResponse<List<GetParentModuleResponseDTO>>.Fail("Failed to retrieve Parent Modules.");
            }
        }

        #endregion
    }
}
