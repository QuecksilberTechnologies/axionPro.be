// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side query contract for retrieving one Tenant.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Queries;

#region Query

/// <summary>
/// Represents the Host-side request to retrieve one Tenant for details or editing.
/// </summary>
public sealed class GetTenantByIdQuery : IRequest<ApiResponse<HostTenantDetailResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantByIdQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant identifier request.</param>
    public GetTenantByIdQuery(GetTenantByIdRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant identifier request.
    /// </summary>
    public GetTenantByIdRequestDTO RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Retrieves one Host-managed Tenant after validating current Host database permission.
/// </summary>
public sealed class GetTenantByIdQueryHandler
    : IRequestHandler<GetTenantByIdQuery, ApiResponse<HostTenantDetailResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant and stored-function persistence operations.</param>
    /// <param name="commonRequestService">Validates the trusted Host request context.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the API boundary.</param>
    public GetTenantByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    /// <summary>
    /// Retrieves the requested Host-visible Tenant.
    /// </summary>
    /// <param name="request">The encrypted Tenant identifier request.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The complete safe Host Tenant detail response with an encrypted identifier.</returns>
    /// <exception cref="NotFoundException">Thrown when the decrypted Tenant is unavailable or soft deleted.</exception>
    public async Task<ApiResponse<HostTenantDetailResponseDTO>> Handle(
        GetTenantByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request?.RequestDTO);
        var dto = request.RequestDTO;
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            _commonRequestService,
            _unitOfWork.StoreProcedureRepository,
            dto.ModuleId,
            dto.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            dto.TenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);
        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantDetailAsync(tenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        tenant.Id = HostTenantIdentifierProtector.Encrypt(
            tenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);

        return ApiResponse<HostTenantDetailResponseDTO>.Success(
            tenant,
            "Tenant retrieved successfully.");
    }
}

#endregion
