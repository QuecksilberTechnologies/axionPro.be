// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Creates Parent/Header Modules for authenticated Host users using
//               a validated request module scope.
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
    /// Represents the request to create a Parent/Header Module.
    /// </summary>
    public class CreateParentModuleCommand : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateParentModuleCommand"/> class.
        /// </summary>
        /// <param name="dto">The requested Header Module values.</param>
        public CreateParentModuleCommand(CreateParentModuleRequestDTO? dto)
        {
            DTO = dto;
        }

        /// <summary>
        /// Gets the requested Header Module values.
        /// </summary>
        public CreateParentModuleRequestDTO? DTO { get; }
    }

    #endregion

    /// <summary>
    /// Handles Host-authorized Parent/Header Module creation.
    /// </summary>
    public class CreateParentModuleCommandHandler : IRequestHandler<CreateParentModuleCommand, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateParentModuleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateParentModuleCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="httpContextAccessor">Provides the ASP.NET Core-authenticated principal.</param>
        /// <param name="logger">Records processing failures.</param>
        public CreateParentModuleCommandHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateParentModuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Creates a Header Module after Host authorization and requested-scope validation.
        /// </summary>
        /// <param name="request">The creation request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The created Header Module response.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the request does not have a valid Host principal.</exception>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            CreateParentModuleCommand request,
            CancellationToken cancellationToken)
        {
            var hostUserId = GetAuthenticatedHostUserId();

            if (request?.DTO == null)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("Parent Module data is required.");
            }

            var dto = request.DTO;
            if (string.IsNullOrWhiteSpace(dto.ModuleCode) || string.IsNullOrWhiteSpace(dto.ModuleName))
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("ModuleCode and ModuleName are required.");
            }

            if (!IsSupportedModuleScope(dto.ModuleScope))
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("ModuleScope must be Tenant or Host scope.");
            }

            try
            {
                var tenantId = ResolveTenantId(dto);
                var moduleCode = dto.ModuleCode.Trim();
                var duplicateExists = await _unitOfWork.ModuleRepository.ExistsParentModuleCodeAsync(
                    moduleCode,
                    tenantId,
                    dto.ModuleScope,
                    null,
                    cancellationToken);

                if (duplicateExists)
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail("A Parent Module with this ModuleCode already exists.");
                }

                var entity = new Module
                {
                    TenantId = tenantId,
                    ModuleScope = dto.ModuleScope,
                    ModuleCode = moduleCode,
                    ModuleName = dto.ModuleName.Trim(),
                    DisplayName = dto.DisplayName?.Trim(),
                    Urlpath = dto.URLPath?.Trim(),
                    ParentModuleId = null,
                    IsLeafNode = false,
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

                var created = await _unitOfWork.ModuleRepository.AddParentModuleAsync(entity, cancellationToken);

                return ApiResponse<GetParentModuleResponseDTO>.Success(
                    ToResponse(created),
                    "Parent Module created successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unable to create Parent Module for HostUserId {HostUserId} and ModuleScope {ModuleScope}.",
                    hostUserId,
                    dto.ModuleScope);
                return ApiResponse<GetParentModuleResponseDTO>.Fail("Failed to create Parent Module.");
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
                throw new UnauthorizedAccessException("Only Host users can create Parent Modules.");
            }

            var hostUserIdValue = principal.FindFirst(AppConstants.HostUserIdClaim)?.Value;
            if (!long.TryParse(hostUserIdValue, out var hostUserId) || hostUserId <= 0)
            {
                throw new UnauthorizedAccessException("A valid HostUserId claim is required.");
            }

            return hostUserId;
        }

        #endregion

        #region Private Methods

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

        /// <summary>
        /// Resolves tenant ownership using the existing create-request value only for Tenant-scope modules.
        /// </summary>
        /// <param name="dto">The validated create request.</param>
        /// <returns>The target tenant identifier, or <see langword="null"/> for Host or tenant master modules without a target tenant.</returns>
        private static long? ResolveTenantId(CreateParentModuleRequestDTO dto)
        {
            if (dto.ModuleScope == AppConstants.HostModuleScope)
            {
                return null;
            }

            return dto.TenantId > 0 ? dto.TenantId : null;
        }

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
