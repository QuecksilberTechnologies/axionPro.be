// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles Host Super Admin queries for Tenant-enabled Parent and Sub-Parent Header Modules.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.Module.TenantParentModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.ModuleCmd.TenantParent.Queries;

#region Queries

/// <summary>
/// Represents the Host-managed request to retrieve a Tenant's entitled Header Module tree.
/// </summary>
public sealed class GetTenantParentModuleHeadersQuery
    : IRequest<ApiResponse<List<TenantParentModuleResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModuleHeadersQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The encrypted Tenant and Header Module filters.</param>
    public GetTenantParentModuleHeadersQuery(TenantParentModuleHeaderRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>Gets the encrypted Tenant and Header Module filters.</summary>
    public TenantParentModuleHeaderRequestDTO? RequestDTO { get; }
}

/// <summary>
/// Represents the Host-managed request for the Tenant Parent Module list endpoint.
/// </summary>
public sealed class GetTenantParentModulesQuery
    : IRequest<ApiResponse<List<TenantParentModuleResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModulesQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The minimal list filter and paging values.</param>
    public GetTenantParentModulesQuery(TenantParentModuleListRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>Gets the minimal list filter and paging values.</summary>
    public TenantParentModuleListRequestDTO? RequestDTO { get; }
}

/// <summary>
/// Represents the Host-managed request to retrieve one Tenant-entitled Header Module by global Module identifier.
/// </summary>
public sealed class GetTenantParentModuleByIdQuery
    : IRequest<ApiResponse<TenantParentModuleResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModuleByIdQuery"/> class.
    /// </summary>
    /// <param name="moduleId">The global Module identifier exposed by the API.</param>
    /// <param name="requestDTO">The encrypted Tenant and Module-scope filters.</param>
    public GetTenantParentModuleByIdQuery(int moduleId, TenantParentModuleByIdRequestDTO? requestDTO)
    {
        ModuleId = moduleId;
        RequestDTO = requestDTO;
    }

    /// <summary>Gets the global Module identifier exposed by the API.</summary>
    public int ModuleId { get; }

    /// <summary>Gets the encrypted Tenant and Module-scope filters.</summary>
    public TenantParentModuleByIdRequestDTO? RequestDTO { get; }
}

#endregion

#region Handlers

/// <summary>
/// Retrieves Tenant-entitled Header Module trees for an authenticated Host Super Admin.
/// </summary>
public sealed class GetTenantParentModuleHeadersQueryHandler
    : TenantParentModuleQueryHandlerBase,
      IRequestHandler<GetTenantParentModuleHeadersQuery, ApiResponse<List<TenantParentModuleResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModuleHeadersQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant entitlement and Tenant persistence operations.</param>
    /// <param name="commonRequestService">Validates the current Host Super Admin and Host encryption-key context.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    public GetTenantParentModuleHeadersQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IEncryptionService encryptionService)
        : base(unitOfWork, commonRequestService, encryptionService)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantParentModuleResponseDTO>>> Handle(
        GetTenantParentModuleHeadersQuery request,
        CancellationToken cancellationToken)
    {
        var dto = request?.RequestDTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
       
        var context = await ResolveTenantRequestAsync(dto.TenantId, cancellationToken);
        var headers = await UnitOfWork.TenantParentModuleRepository.GetHeaderTreeAsync(
            context.TenantId,
            dto.ModuleScope,
            dto.IsEnabled,
            cancellationToken);

        return ApiResponse<List<TenantParentModuleResponseDTO>>.Success(
            headers.Select(header => MapResponse(header, context.TenantEncryptionKey)).ToList(),
            "Tenant module headers retrieved successfully.");
    }
}

/// <summary>
/// Retrieves a paged all-Tenant list of Main Parent Headers for an authenticated Host Super Admin.
/// </summary>
public sealed class GetTenantParentModulesQueryHandler
    : TenantParentModuleQueryHandlerBase,
      IRequestHandler<GetTenantParentModulesQuery, ApiResponse<List<TenantParentModuleResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModulesQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant entitlement and Tenant persistence operations.</param>
    /// <param name="commonRequestService">Validates the current Host Super Admin and Host encryption-key context.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    public GetTenantParentModulesQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IEncryptionService encryptionService)
        : base(unitOfWork, commonRequestService, encryptionService)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<List<TenantParentModuleResponseDTO>>> Handle(
        GetTenantParentModulesQuery request,
        CancellationToken cancellationToken)
    {
        await ValidateHostSuperAdminRequestAsync();
        var tenantEncryptionKey = await GetTrustedHostTenantEncryptionKeyAsync();

        var dto = request?.RequestDTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var page = await UnitOfWork.TenantParentModuleRepository.GetPagedMainParentHeadersAsync(
            dto.IsActive,
            dto.PageNumber,
            dto.PageSize,
            cancellationToken);

        return ApiResponse<List<TenantParentModuleResponseDTO>>.SuccessPaginated(
            page.Data.Select(module => MapResponse(module, tenantEncryptionKey)).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            "Tenant Parent Modules retrieved successfully.");
    }
}

/// <summary>
/// Retrieves one Tenant-entitled Header Module for an authenticated Host Super Admin.
/// </summary>
public sealed class GetTenantParentModuleByIdQueryHandler
    : TenantParentModuleQueryHandlerBase,
      IRequestHandler<GetTenantParentModuleByIdQuery, ApiResponse<TenantParentModuleResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantParentModuleByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant entitlement and Tenant persistence operations.</param>
    /// <param name="commonRequestService">Validates the current Host Super Admin and Host encryption-key context.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    public GetTenantParentModuleByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IEncryptionService encryptionService)
        : base(unitOfWork, commonRequestService, encryptionService)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantParentModuleResponseDTO>> Handle(
        GetTenantParentModuleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var dto = request?.RequestDTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (request.ModuleId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

      
        var context = await ResolveTenantRequestAsync(dto.TenantId, cancellationToken);
        var module = await UnitOfWork.TenantParentModuleRepository.GetHeaderByModuleIdAsync(
            context.TenantId,
            request.ModuleId,
            dto.ModuleScope,
            cancellationToken);

        if (module is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        return ApiResponse<TenantParentModuleResponseDTO>.Success(
            MapResponse(module, context.TenantEncryptionKey),
            "Tenant Parent Module retrieved successfully.");
    }
}

#endregion

#region Shared Host Request Validation

/// <summary>
/// Provides shared Host Super Admin validation, Tenant identifier protection, and response mapping for Tenant Parent Module queries.
/// </summary>
public abstract class TenantParentModuleQueryHandlerBase
{
    /// <summary>Provides the existing UnitOfWork repository access.</summary>
    protected IUnitOfWork UnitOfWork { get; }

    private readonly ICommonRequestService _commonRequestService;
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantParentModuleQueryHandlerBase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant entitlement and Tenant persistence operations.</param>
    /// <param name="commonRequestService">Validates the current Host request contexts.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    protected TenantParentModuleQueryHandlerBase(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IEncryptionService encryptionService)
    {
        UnitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Validates the current Host Super Admin, obtains its trusted Tenant encryption-key context, and resolves the submitted encrypted Tenant identifier.
    /// </summary>
    /// <param name="encryptedTenantId">The encrypted Tenant identifier from the request.</param>
    /// <param name="cancellationToken">A token used to cancel Tenant resolution.</param>
    /// <returns>The decrypted Tenant identifier and trusted encryption key.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the encrypted Tenant identifier is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant does not exist.</exception>
    protected async Task<TenantParentModuleRequestContext> ResolveTenantRequestAsync(
        string? encryptedTenantId,
        CancellationToken cancellationToken)
    {
        await _commonRequestService.ValidateHostSuperAdminRequestAsync();
        var hostKeyContext = await _commonRequestService.ValidateHostUserPermissionRequestAsync();
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            encryptedTenantId,
            hostKeyContext.TenantEncryptionKey,
            _encryptionService);
        var tenant = await UnitOfWork.TenantRepository.GetHostManagedTenantByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        return new TenantParentModuleRequestContext(tenantId, hostKeyContext.TenantEncryptionKey);
    }

    /// <summary>
    /// Validates the current trusted Host request, Host user type, and current Super Admin role.
    /// </summary>
    /// <returns>A task that completes when the Host Super Admin validation succeeds.</returns>
    protected Task ValidateHostSuperAdminRequestAsync()
    {
        return _commonRequestService.ValidateHostSuperAdminRequestAsync();
    }

    /// <summary>
    /// Obtains the trusted Host token Tenant encryption key without resolving a Tenant identifier.
    /// </summary>
    /// <returns>The trusted Host token Tenant encryption key.</returns>
    protected async Task<string> GetTrustedHostTenantEncryptionKeyAsync()
    {
        var hostKeyContext = await _commonRequestService.ValidateHostUserPermissionRequestAsync();
        return hostKeyContext.TenantEncryptionKey;
    }

    /// <summary>
     

    /// <summary>
    /// Maps one raw repository result to a Host-facing response with an encrypted Tenant identifier.
    /// </summary>
    /// <param name="module">The raw Tenant entitlement and Module metadata.</param>
    /// <param name="tenantEncryptionKey">The trusted Host token key.</param>
    /// <returns>The Host-facing Tenant Parent Module response.</returns>
    protected TenantParentModuleResponseDTO MapResponse(
        TenantParentModuleReadModel module,
        string tenantEncryptionKey)
    {
        return new TenantParentModuleResponseDTO
        {
            TenantId = HostTenantIdentifierProtector.Encrypt(module.TenantId, tenantEncryptionKey, _encryptionService),
            Id = module.Id,
            ModuleCode = module.ModuleCode,
            ModuleName = module.ModuleName,
            DisplayName = module.DisplayName,
            UrlPath = module.UrlPath,
            ImageIconWeb = module.ImageIconWeb,
            ImageIconMobile = module.ImageIconMobile,
            ItemPriority = module.ItemPriority,
            ParentModuleId = module.ParentModuleId,
            IsLeafNode = module.IsLeafNode,
            IsEnabled = module.IsEnabled,
            ModuleScope = module.ModuleScope,
            Children = module.Children
                .Select(child => MapResponse(child, tenantEncryptionKey))
                .ToList()
        };
    }
}

/// <summary>
/// Represents a decrypted Tenant identifier and its trusted Host token encryption key.
/// </summary>
/// <param name="TenantId">The repository-safe numeric Tenant identifier.</param>
/// <param name="TenantEncryptionKey">The trusted Host token encryption key.</param>
public sealed record TenantParentModuleRequestContext(long TenantId, string TenantEncryptionKey);

#endregion
