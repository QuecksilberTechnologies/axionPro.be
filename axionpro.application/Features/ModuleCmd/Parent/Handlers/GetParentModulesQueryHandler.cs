// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves scope-filtered Parent/Header Module lists for Host users.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Query

    /// <summary>
    /// Represents a read-only request for Parent/Header Modules in a required scope.
    /// </summary>
    public class GetParentModulesQuery : IRequest<ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModulesQuery"/> class.
        /// </summary>
        /// <param name="moduleScope">The requested module scope.</param>
        /// <param name="isActive">When supplied, filters results by active state.</param>
        public GetParentModulesQuery(short moduleScope, bool? isActive)
        {
            ModuleScope = moduleScope;
            IsActive = isActive;
        }

        /// <summary>Gets the requested module scope.</summary>
        public short ModuleScope { get; }

        /// <summary>Gets the optional active-state filter.</summary>
        public bool? IsActive { get; }
    }

    #endregion

    /// <summary>
    /// Handles Host-authorized Parent/Header Module list retrieval within a requested scope.
    /// </summary>
    public class GetParentModulesQueryHandler : IRequestHandler<GetParentModulesQuery, ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetParentModulesQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModulesQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        /// <param name="commonRequestService">Provides common request validation functionality.</param>
        public GetParentModulesQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetParentModulesQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Retrieves all Header Modules in the requested scope, optionally filtered by active state.
        /// </summary>
        /// <param name="request">The read-only list request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The ordered Header Module list, which may be empty.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        public async Task<ApiResponse<List<GetParentModuleResponseDTO>>> Handle(
            GetParentModulesQuery request,
            CancellationToken cancellationToken)
        {

            var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();

            if (request == null)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            if (!IsSupportedModuleScope(request.ModuleScope))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            try
            {
                var modules = await _unitOfWork.ModuleRepository.GetParentModulesAsync(
                    request.ModuleScope,
                    request.IsActive,
                    cancellationToken);

                return ApiResponse<List<GetParentModuleResponseDTO>>.Success(
                    modules,
                    "Parent Modules retrieved successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to retrieve Parent Modules in ModuleScope {ModuleScope}.", request.ModuleScope);
                throw;
            }
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Verifies that the ASP.NET Core-authenticated principal is a Host user.
        /// </summary>
        /// <returns>The authenticated Host user identifier.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the principal is missing, unauthenticated, non-Host, or lacks a valid Host user identifier.</exception>
       
        #endregion

        #region Validation

        /// <summary>
        /// Determines whether the requested scope is one of the two supported application module scopes.
        /// </summary>
        /// <param name="moduleScope">The requested module scope.</param>
        /// <returns><see langword="true"/> when the scope is supported.</returns>
        private static bool IsSupportedModuleScope(short moduleScope)
        {
            return moduleScope == AppConstants.TenantModuleScope ||
                   moduleScope == AppConstants.HostModuleScope;
        }

        #endregion
    }
}
