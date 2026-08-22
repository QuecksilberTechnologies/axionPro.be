// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for activating a Tenant.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to activate a Tenant.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c>,
/// set <c>Tenant.IsActive</c> and all corresponding <c>LoginCredential.IsActive</c> values to
/// <see langword="true"/>, and persist the change atomically. The request intentionally supplies
/// no client-controlled status or actor identifier.
/// </remarks>
public sealed class ActivateTenantCommand : IRequest<ApiResponse<TenantResponseDTO>>
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
    : IRequestHandler<ActivateTenantCommand, ApiResponse<TenantResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateTenantCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    public ActivateTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
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
    public async Task<ApiResponse<TenantResponseDTO>> Handle(
        ActivateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();

        if (request?.RequestDTO is null || request.RequestDTO.TenantId <= 0 || hostUserId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(request.RequestDTO.TenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        var utcNow = DateTime.UtcNow;
        tenant.IsActive = true;
        tenant.UpdatedById = hostUserId;
        tenant.UpdatedDateTime = utcNow;

        await PersistStatusAsync(tenant, hostUserId, utcNow, cancellationToken);

        return ApiResponse<TenantResponseDTO>.Success(
            MapTenant(tenant),
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

    private static TenantResponseDTO MapTenant(Tenant tenant)
    {
        return new TenantResponseDTO
        {
            Id = tenant.Id,
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
