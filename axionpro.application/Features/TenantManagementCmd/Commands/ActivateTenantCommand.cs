// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for activating a Tenant.
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

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to activate a Tenant.
/// </summary>
public sealed class ActivateTenantCommand : IRequest<ApiResponse<HostTenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant activation request.</param>
    public ActivateTenantCommand(ActivateTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant activation request, including its administrative remark.
    /// </summary>
    public ActivateTenantRequestDTO RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Activates a Host-managed Tenant and valid, non-soft-deleted Tenant login credentials atomically.
/// </summary>
public sealed class ActivateTenantCommandHandler
    : IRequestHandler<ActivateTenantCommand, ApiResponse<HostTenantResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateTenantCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    public ActivateTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Activates a Tenant and its valid credentials in one transaction.
    /// </summary>
    /// <param name="request">The Tenant activation command.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The activated Tenant response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the activation request is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant is unavailable or soft deleted.</exception>
    public async Task<ApiResponse<HostTenantResponseDTO>> Handle(
        ActivateTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (request?.RequestDTO is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var hostContext = await _commonRequestService.ValidateHostUserPermissionRequestAsync();
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            request.RequestDTO.TenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        var utcNow = DateTime.UtcNow;
        tenant.IsActive = true;
        tenant.UpdatedById = hostContext.HostUserId;
        tenant.UpdatedDateTime = utcNow;

        await PersistStatusAsync(tenant, hostContext.HostUserId, utcNow, cancellationToken);

        return ApiResponse<HostTenantResponseDTO>.Success(
            MapTenant(tenant, hostContext.TenantEncryptionKey),
            AppConstants.SuccessMessages.TenantActivatedSuccessfully);
    }

    #endregion

    #region Helpers

    private async Task PersistStatusAsync(
        Tenant tenant,
        long hostUserId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        var transactionStarted = false;
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;
            await _unitOfWork.TenantRepository
                .SynchronizeTenantStatusAsync(tenant, hostUserId, utcNow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            transactionStarted = false;
        }
        catch
        {
            if (transactionStarted)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            }

            throw;
        }
    }

    private HostTenantResponseDTO MapTenant(Tenant tenant, string tenantEncryptionKey)
    {
        return new HostTenantResponseDTO
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
}

#endregion
