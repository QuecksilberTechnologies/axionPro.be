// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for deactivating a Tenant.
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
/// Represents the Host-side request to deactivate a Tenant.
/// </summary>
public sealed class DeactivateTenantCommand : IRequest<ApiResponse<HostTenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant deactivation request.</param>
    public DeactivateTenantCommand(DeactivateTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant deactivation request, including its administrative remark.
    /// </summary>
    public DeactivateTenantRequestDTO RequestDTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Deactivates a Host-managed Tenant and valid, non-soft-deleted Tenant login credentials atomically.
/// </summary>
public sealed class DeactivateTenantCommandHandler
    : IRequestHandler<DeactivateTenantCommand, ApiResponse<HostTenantResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IEncryptionService _encryptionService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateTenantCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    /// <param name="encryptionService">Protects Tenant identifiers at the Host API boundary.</param>
    public DeactivateTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _encryptionService = encryptionService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Deactivates a Tenant and its valid credentials in one transaction.
    /// </summary>
    /// <param name="request">The Tenant deactivation command.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The deactivated Tenant response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the deactivation request is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant is unavailable or soft deleted.</exception>
    public async Task<ApiResponse<HostTenantResponseDTO>> Handle(
        DeactivateTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (request?.RequestDTO is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            _commonRequestService,
            _unitOfWork.StoreProcedureRepository,
            request.RequestDTO.ModuleId,
            request.RequestDTO.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            request.RequestDTO.TenantId,
            hostContext.TenantEncryptionKey,
            _encryptionService);

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        var utcNow = DateTime.UtcNow;
        tenant.IsActive = false;
        tenant.UpdatedById = hostContext.HostUserId;
        tenant.UpdatedDateTime = utcNow;

        await PersistStatusAsync(tenant, hostContext.HostUserId, utcNow, cancellationToken);

        return ApiResponse<HostTenantResponseDTO>.Success(
            MapTenant(tenant, hostContext.TenantEncryptionKey),
            AppConstants.SuccessMessages.TenantDeactivatedSuccessfully);
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
            Id = HostTenantIdentifierProtector.Encrypt(tenant.Id, tenantEncryptionKey, _encryptionService),
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
