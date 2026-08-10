// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Retrieves modules available in the Host application scope.
// ============================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HostCmd.Handler
{
    #region Query

    /// <summary>
    /// Represents the request to retrieve Host-scope modules with an optional active-state filter.
    /// </summary>
    public class GetHostModulesQuery : IRequest<ApiResponse<List<GetHostModuleResponseDTO>>>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetHostModulesQuery"/> class.
        /// </summary>
        /// <param name="isActive">When supplied, filters Host modules by their active state.</param>
        public GetHostModulesQuery(bool? isActive)
        {
            IsActive = isActive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the optional active-state filter.
        /// </summary>
        public bool? IsActive { get; }

        #endregion
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles retrieval of modules that belong to the Host application scope.
    /// </summary>
    public class GetHostModulesQueryHandler
        : IRequestHandler<GetHostModulesQuery, ApiResponse<List<GetHostModuleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetHostModulesQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetHostModulesQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work used to retrieve modules.</param>
        /// <param name="logger">The logger used to record handler activity.</param>
        public GetHostModulesQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetHostModulesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves Host-scope modules and returns a successful response even when none match the filter.
        /// </summary>
        /// <param name="request">The query containing the optional active-state filter.</param>
        /// <param name="cancellationToken">A token to observe while handling the query.</param>
        /// <returns>A response containing the matching Host modules.</returns>
        public async Task<ApiResponse<List<GetHostModuleResponseDTO>>> Handle(
            GetHostModulesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving Host modules. IsActive: {IsActive}", request.IsActive);

            var modules = await _unitOfWork.ModuleRepository
                .GetHostModulesAsync(request.IsActive);

            return ApiResponse<List<GetHostModuleResponseDTO>>.Success(
                modules,
                "Host modules retrieved successfully.");
        }

        #endregion
    }

    #endregion
}
