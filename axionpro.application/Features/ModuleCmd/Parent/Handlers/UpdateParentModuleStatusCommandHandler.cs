// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Updates Parent/Header Module status for authenticated Host users.
// ============================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to change a Parent/Header Module active state in a required scope.
    /// </summary>
    public class UpdateParentModuleStatusCommand : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommand"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="dto">The target active state and required module scope.</param>
        public UpdateParentModuleStatusCommand(int id, UpdateParentModuleStatusRequestDTO? dto)
        {
            Id = id;
            DTO = dto;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the target active state and required module scope.</summary>
        public UpdateParentModuleStatusRequestDTO? DTO { get; }
    }

    #endregion

    /// <summary>
    /// Handles Host-authorized non-destructive Parent/Header Module status changes.
    /// </summary>
    public class UpdateParentModuleStatusCommandHandler : IRequestHandler<UpdateParentModuleStatusCommand, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateParentModuleStatusCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public UpdateParentModuleStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateParentModuleStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Changes a scoped Header Module state while preventing deactivation that would hide active direct children.
        /// </summary>
        /// <param name="request">The status change request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The Header Module response after a permitted status change.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the scoped Header Module does not exist.</exception>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            UpdateParentModuleStatusCommand request,
            CancellationToken cancellationToken)
        {
            var hostUserId = GetAuthenticatedHostUserId();

            if (request == null || request.Id <= 0 || request.DTO == null)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("A valid Parent Module identifier and status are required.");
            }

            var dto = request.DTO;
            if (!IsSupportedModuleScope(dto.ModuleScope))
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("ModuleScope must be Tenant or Host scope.");
            }

            try
            {
                var entity = await _unitOfWork.ModuleRepository.GetParentModuleForUpdateAsync(
                    request.Id,
                    dto.ModuleScope,
                    cancellationToken);

                if (entity == null)
                {
                    throw new KeyNotFoundException("Parent Module was not found in the requested ModuleScope.");
                }

                if (entity.IsActive && !dto.IsActive && await _unitOfWork.ModuleRepository.HasChildrenAsync(
                    entity.Id,
                    entity.ModuleScope,
                    cancellationToken))
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail(
                        "Deactivate active child modules before deactivating this Parent Module.");
                }

                entity.ParentModuleId = null;
                entity.IsLeafNode = false;
                entity.IsActive = dto.IsActive;
                entity.UpdatedById = hostUserId;
                entity.UpdatedDateTime = DateTime.UtcNow;

                var updated = await _unitOfWork.ModuleRepository.UpdateParentModuleAsync(entity, cancellationToken);

                return ApiResponse<GetParentModuleResponseDTO>.Success(
                    ToResponse(updated),
                    "Parent Module status updated successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update Parent Module {ModuleId} status in ModuleScope {ModuleScope}.", request.Id, dto.ModuleScope);
                throw;
            }
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Verifies that the ASP.NET Core-authenticated principal is a Host user and returns its actor identifier.
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
                throw new UnauthorizedAccessException("Only Host users can update Parent Modules.");
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

        #region Private Methods

        /// <summary>
        /// Maps a persisted Header Module to the CRUD response.
        /// </summary>
        /// <param name="module">The persisted Header Module.</param>
        /// <returns>The response model.</returns>
        private static GetParentModuleResponseDTO ToResponse(Module module)
        {
            return new GetParentModuleResponseDTO
            {
                Id = module.Id,
                ModuleCode = module.ModuleCode,
                ModuleName = module.ModuleName,
                DisplayName = module.DisplayName,
                URLPath = module.Urlpath,
                ParentModuleId = module.ParentModuleId,
                IsLeafNode = module.IsLeafNode,
                IsModuleDisplayInUI = module.IsModuleDisplayInUI,
                IsCommonMenu = module.IsCommonMenu,
                ModuleScope = module.ModuleScope,
                IsActive = module.IsActive,
                ImageIconWeb = module.ImageIconWeb,
                ImageIconMobile = module.ImageIconMobile,
                ItemPriority = module.ItemPriority,
                Remark = module.Remark,
                AddedById = module.AddedById,
                AddedDateTime = module.AddedDateTime,
                UpdatedById = module.UpdatedById,
                UpdatedDateTime = module.UpdatedDateTime
            };
        }

        #endregion
    }
}
