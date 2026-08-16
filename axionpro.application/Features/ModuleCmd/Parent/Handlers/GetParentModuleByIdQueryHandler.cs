// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves a scope-filtered Parent/Header Module for Host users.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Query

    /// <summary>
    /// Represents a read-only request for one Parent/Header Module in a required scope.
    /// </summary>
    public class GetParentModuleByIdQuery : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleByIdQuery"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="moduleScope">The requested module scope.</param>
        public GetParentModuleByIdQuery(int id, short moduleScope)
        {
            Id = id;
            ModuleScope = moduleScope;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the requested module scope.</summary>
        public short ModuleScope { get; }
    }

    #endregion

    /// <summary>
    /// Handles Host-authorized lookup of a Parent/Header Module within its requested scope.
    /// </summary>
    public class GetParentModuleByIdQueryHandler : IRequestHandler<GetParentModuleByIdQuery, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetParentModuleByIdQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public GetParentModuleByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetParentModuleByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Retrieves a Header Module only when its identifier and requested scope both match.
        /// </summary>
        /// <param name="request">The read-only request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The matching Header Module response.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the scoped Header Module does not exist.</exception>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            GetParentModuleByIdQuery request,
            CancellationToken cancellationToken)
        {
            GetAuthenticatedHostUserId();

            if (request == null || request.Id <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            if (!IsSupportedModuleScope(request.ModuleScope))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            try
            {
                var module = await _unitOfWork.ModuleRepository.GetParentModuleByIdAsync(
                    request.Id,
                    request.ModuleScope,
                    cancellationToken);

                if (module == null)
                {
                    throw new KeyNotFoundException("Parent Module was not found in the requested ModuleScope.");
                }

                return ApiResponse<GetParentModuleResponseDTO>.Success(module, "Parent Module retrieved successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to retrieve Parent Module {ModuleId} in ModuleScope {ModuleScope}.", request.Id, request.ModuleScope);
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
        private long GetAuthenticatedHostUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("An authenticated Host principal is required.");
            }

            var userType = principal.FindFirst(AppConstants.UserTypeClaim)?.Value;
            if (!string.Equals(userType, AppConstants.HostUserType, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Only Host users can retrieve Parent Modules.");
            }

            var hostUserIdValue = principal.FindFirst(AppConstants.HostUserIdClaim)?.Value;
            if (!long.TryParse(hostUserIdValue, out var hostUserId) || hostUserId <= 0)
            {
                throw new UnauthorizedAccessException("A valid HostUserId claim is required.");
            }

            return hostUserId;
        }

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
