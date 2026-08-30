// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side query contract for Tenant management records.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Common.Helpers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Queries;

#region Query

/// <summary>
/// Represents the Host-side request to retrieve Tenant management records.
/// </summary>
public sealed class GetAllTenantsQuery : IRequest<ApiResponse<List<HostTenantResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllTenantsQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The optional Tenant management filters and paging values.</param>
    public GetAllTenantsQuery(GetAllTenantsRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the optional Tenant management filters and paging values.
    /// </summary>
    public GetAllTenantsRequestDTO? RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Retrieves Host-managed Tenants after current Host database permission validation.
/// </summary>
public sealed class GetAllTenantsQueryHandler
    : IRequestHandler<GetAllTenantsQuery, ApiResponse<List<HostTenantResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllTenantsQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant and stored-function persistence operations.</param>
    /// <param name="commonRequestService">Validates the trusted Host request context.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the API boundary.</param>
    public GetAllTenantsQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    /// <summary>
    /// Retrieves the requested Host-visible Tenant page.
    /// </summary>
    /// <param name="request">The Host Tenant list request.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A paged Host Tenant response with encrypted identifiers.</returns>
    public async Task<ApiResponse<List<HostTenantResponseDTO>>> Handle(
        GetAllTenantsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.RequestDTO ?? throw new ArgumentNullException(nameof(request.RequestDTO));
        var hostContext = await _commonRequestService.ValidateHostUserPermissionRequestAsync();

        var page = await _unitOfWork.TenantRepository
            .GetHostManagedTenantsAsync(filter, cancellationToken);
        var response = page.Data
            .Select(tenant => MapTenant(tenant, hostContext.TenantEncryptionKey))
            .ToList();

        return ApiResponse<List<HostTenantResponseDTO>>.SuccessPaginated(
            response,
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            "Tenants retrieved successfully.");
    }

    private HostTenantResponseDTO MapTenant(Tenant tenant, string tenantEncryptionKey) =>
        new()
        {
            Id = HostTenantIdentifierProtector.Encrypt(tenant.Id, tenantEncryptionKey, _idEncoderService),
            CompanyName = tenant.CompanyName,
            TenantCode = tenant.TenantCode,
            CompanyEmailDomain = tenant.CompanyEmailDomain,
            TenantEmail = tenant.TenantEmail,
            ContactPersonName = tenant.ContactPersonName,
            ContactNumber = tenant.ContactNumber,
            CountryId = tenant.CountryId,
            IsVerified = tenant.IsVerified,
            IsActive = tenant.IsActive
        };
}

#endregion
