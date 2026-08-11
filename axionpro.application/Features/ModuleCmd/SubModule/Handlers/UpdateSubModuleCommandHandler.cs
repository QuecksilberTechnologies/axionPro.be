// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Updates direct SubModules for authenticated Host users.
// ============================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.ModuleCmd.SubModule.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update a direct child SubModule.
    /// </summary>
    public class UpdateSubModuleCommand : IRequest<ApiResponse<GetSubModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSubModuleCommand"/> class.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="dto">The editable SubModule values and current scope.</param>
        public UpdateSubModuleCommand(int id, UpdateSubModuleRequestDTO? dto)
        {
            Id = id;
            DTO = dto;
        }

        /// <summary>Gets the SubModule identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the editable SubModule values and current scope.</summary>
        public UpdateSubModuleRequestDTO? DTO { get; }
    }

    #endregion
}

namespace axionpro.application.Features.ModuleCmd.SubModule.Handlers
{
    /// <summary>
    /// Handles Host-authorized direct SubModule updates without changing inherited tenant ownership or scope.
    /// </summary>
    public class UpdateSubModuleCommandHandler : IRequestHandler<UpdateSubModuleCommand, ApiResponse<GetSubModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateSubModuleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSubModuleCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public UpdateSubModuleCommandHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateSubModuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Updates editable direct-child values and permits a move only to a compatible Header Module.
        /// </summary>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The updated direct child response.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the scoped child or target Header Module does not exist.</exception>
        public async Task<ApiResponse<GetSubModuleResponseDTO>> Handle(
            UpdateSubModuleCommand request,
            CancellationToken cancellationToken)
        {
            var hostUserId = GetAuthenticatedHostUserId();

            if (request == null || request.Id <= 0 || request.DTO == null || request.DTO.ParentModuleId <= 0)
            {
                return ApiResponse<GetSubModuleResponseDTO>.Fail("A valid SubModule identifier, ParentModuleId, and data are required.");
            }

            var dto = request.DTO;
            if (string.IsNullOrWhiteSpace(dto.ModuleCode) || string.IsNullOrWhiteSpace(dto.ModuleName))
            {
                return ApiResponse<GetSubModuleResponseDTO>.Fail("ModuleCode and ModuleName are required.");
            }

            if (!IsSupportedModuleScope(dto.ModuleScope))
            {
                return ApiResponse<GetSubModuleResponseDTO>.Fail("ModuleScope must be Tenant or Host scope.");
            }

            try
            {
                var entity = await _unitOfWork.ModuleRepository.GetSubModuleForUpdateAsync(
                    request.Id,
                    dto.ModuleScope,
                    cancellationToken);

                if (entity == null)
                {
                    throw new KeyNotFoundException("SubModule was not found in the requested ModuleScope.");
                }

                var parentModule = await _unitOfWork.ModuleRepository.GetParentModuleForSubModuleAsync(
                    dto.ParentModuleId,
                    dto.ModuleScope,
                    cancellationToken);

                if (parentModule == null)
                {
                    throw new KeyNotFoundException("Parent Module was not found in the requested ModuleScope.");
                }

                if (parentModule.TenantId != entity.TenantId)
                {
                    return ApiResponse<GetSubModuleResponseDTO>.Fail(
                        "A SubModule can only move to a Parent Module with compatible tenant ownership.");
                }

                if (dto.IsActive && !parentModule.IsActive)
                {
                    return ApiResponse<GetSubModuleResponseDTO>.Fail("An active SubModule requires an active Parent Module.");
                }

                var moduleCode = dto.ModuleCode.Trim();
                var duplicateExists = await _unitOfWork.ModuleRepository.ExistsSubModuleCodeAsync(
                    moduleCode,
                    entity.TenantId,
                    entity.ModuleScope,
                    entity.Id,
                    cancellationToken);

                if (duplicateExists)
                {
                    return ApiResponse<GetSubModuleResponseDTO>.Fail("A SubModule with this ModuleCode already exists.");
                }

                entity.ModuleCode = moduleCode;
                entity.ModuleName = dto.ModuleName.Trim();
                entity.DisplayName = dto.DisplayName?.Trim();
                entity.Urlpath = dto.URLPath?.Trim();
                entity.ParentModuleId = parentModule.Id;
                entity.IsLeafNode = true;
                entity.IsModuleDisplayInUI = dto.IsModuleDisplayInUI;
                entity.IsCommonMenu = dto.IsCommonMenu;
                entity.IsActive = dto.IsActive;
                entity.ImageIconWeb = dto.ImageIconWeb?.Trim();
                entity.ImageIconMobile = dto.ImageIconMobile?.Trim();
                entity.ItemPriority = dto.ItemPriority;
                entity.Remark = dto.Remark?.Trim();
                entity.UpdatedById = hostUserId;
                entity.UpdatedDateTime = DateTime.UtcNow;

                // ModuleScope and TenantId remain immutable because they are inherited from the existing module design.
                var updated = await _unitOfWork.ModuleRepository.UpdateSubModuleAsync(entity, cancellationToken);

                return ApiResponse<GetSubModuleResponseDTO>.Success(
                    ToResponse(updated, parentModule),
                    "SubModule updated successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update SubModule {ModuleId} in ModuleScope {ModuleScope}.", request.Id, dto.ModuleScope);
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
                throw new UnauthorizedAccessException("Only Host users can update SubModules.");
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
        /// Maps a persisted direct child and its validated Header Module to the CRUD response.
        /// </summary>
        /// <param name="module">The persisted direct child.</param>
        /// <param name="parentModule">The validated Header Module.</param>
        /// <returns>The direct-child response.</returns>
        private static GetSubModuleResponseDTO ToResponse(Module module, Module parentModule)
        {
            return new GetSubModuleResponseDTO
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
                UpdatedDateTime = module.UpdatedDateTime,
                ParentModule = new ParentModuleSummaryDTO
                {
                    Id = parentModule.Id,
                    ModuleCode = parentModule.ModuleCode,
                    ModuleName = parentModule.ModuleName,
                    DisplayName = parentModule.DisplayName,
                    ModuleScope = parentModule.ModuleScope,
                    IsActive = parentModule.IsActive
                }
            };
        }

        #endregion
    }
}
