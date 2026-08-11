// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Creates direct SubModules for authenticated Host users.
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
    /// Represents the request to create a direct child SubModule.
    /// </summary>
    public class CreateSubModuleCommand : IRequest<ApiResponse<GetSubModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubModuleCommand"/> class.
        /// </summary>
        /// <param name="dto">The requested direct child Module values.</param>
        public CreateSubModuleCommand(CreateSubModuleRequestDTO? dto)
        {
            DTO = dto;
        }

        /// <summary>Gets the requested direct child Module values.</summary>
        public CreateSubModuleRequestDTO? DTO { get; }
    }

    #endregion
}

namespace axionpro.application.Features.ModuleCmd.SubModule.Handlers
{
    /// <summary>
    /// Handles Host-authorized creation of a direct child SubModule.
    /// </summary>
    public class CreateSubModuleCommandHandler : IRequestHandler<CreateSubModuleCommand, ApiResponse<GetSubModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateSubModuleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubModuleCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public CreateSubModuleCommandHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateSubModuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Creates a direct child only under a compatible Header Module in the requested scope.
        /// </summary>
        /// <param name="request">The creation request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The created direct child response.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the scoped Header Module does not exist.</exception>
        public async Task<ApiResponse<GetSubModuleResponseDTO>> Handle(
            CreateSubModuleCommand request,
            CancellationToken cancellationToken)
        {
            var hostUserId = GetAuthenticatedHostUserId();

            if (request?.DTO == null || request.DTO.ParentModuleId <= 0)
            {
                return ApiResponse<GetSubModuleResponseDTO>.Fail("Valid SubModule data and a ParentModuleId are required.");
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
                var parentModule = await _unitOfWork.ModuleRepository.GetParentModuleForSubModuleAsync(
                    dto.ParentModuleId,
                    dto.ModuleScope,
                    cancellationToken);

                if (parentModule == null)
                {
                    throw new KeyNotFoundException("Parent Module was not found in the requested ModuleScope.");
                }

                if (dto.IsActive && !parentModule.IsActive)
                {
                    return ApiResponse<GetSubModuleResponseDTO>.Fail("An active SubModule requires an active Parent Module.");
                }

                var moduleCode = dto.ModuleCode.Trim();
                var duplicateExists = await _unitOfWork.ModuleRepository.ExistsSubModuleCodeAsync(
                    moduleCode,
                    parentModule.TenantId,
                    dto.ModuleScope,
                    null,
                    cancellationToken);

                if (duplicateExists)
                {
                    return ApiResponse<GetSubModuleResponseDTO>.Fail("A SubModule with this ModuleCode already exists.");
                }

                var entity = new Module
                {
                    TenantId = parentModule.TenantId,
                    ModuleScope = dto.ModuleScope,
                    ModuleCode = moduleCode,
                    ModuleName = dto.ModuleName.Trim(),
                    DisplayName = dto.DisplayName?.Trim(),
                    Urlpath = dto.URLPath?.Trim(),
                    ParentModuleId = parentModule.Id,
                    IsLeafNode = true,
                    IsModuleDisplayInUI = dto.IsModuleDisplayInUI,
                    IsCommonMenu = dto.IsCommonMenu,
                    IsActive = dto.IsActive,
                    ImageIconWeb = dto.ImageIconWeb?.Trim(),
                    ImageIconMobile = dto.ImageIconMobile?.Trim(),
                    ItemPriority = dto.ItemPriority,
                    Remark = dto.Remark?.Trim(),
                    AddedById = hostUserId,
                    AddedDateTime = DateTime.UtcNow
                };

                var created = await _unitOfWork.ModuleRepository.AddSubModuleAsync(entity, cancellationToken);

                return ApiResponse<GetSubModuleResponseDTO>.Success(
                    ToResponse(created, parentModule),
                    "SubModule created successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unable to create SubModule for ParentModuleId {ParentModuleId} and ModuleScope {ModuleScope}.",
                    dto.ParentModuleId,
                    dto.ModuleScope);
                throw;
            }
        }

        #endregion

        #region Host Authorization

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
                throw new UnauthorizedAccessException("Only Host users can create SubModules.");
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
