// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for Tenant soft deletion.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to place a Tenant into the existing soft-delete lifecycle.
/// </summary>
public sealed class DeleteTenantCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant deletion request.</param>
    public DeleteTenantCommand(DeleteTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant deletion request.
    /// </summary>
    public DeleteTenantRequestDTO RequestDTO { get; }
}

#endregion

#region Host-Managed Route Command

/// <summary>
/// Represents the Host-managed route request to soft delete one Tenant.
/// </summary>
public sealed class DeleteHostManagedTenantCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteHostManagedTenantCommand"/> class.
    /// </summary>
    /// <param name="encryptedTenantId">The encrypted Tenant identifier from the route or request body.</param>
    /// <param name="permissionRequest">The Host module-operation permission metadata.</param>
    public DeleteHostManagedTenantCommand(
        string encryptedTenantId,
        PermissionRequestDTO? permissionRequest)
    {
        EncryptedTenantId = encryptedTenantId;
        ModuleId = permissionRequest?.ModuleId ?? 0;
        OperationId = permissionRequest?.OperationId ?? 0;
    }

    /// <summary>
    /// Gets the encrypted Tenant identifier from the route or request body.
    /// </summary>
    public string EncryptedTenantId { get; }

    /// <summary>
    /// Gets the requested Host module identifier.
    /// </summary>
    public int ModuleId { get; }

    /// <summary>
    /// Gets the requested Host operation identifier.
    /// </summary>
    public int OperationId { get; }
}

#endregion

#region Host-Managed Route Handler

/// <summary>
/// Soft deletes a Host-managed Tenant and deactivates related login credentials in one transaction.
/// </summary>
public sealed class DeleteHostManagedTenantCommandHandler
    : IRequestHandler<DeleteHostManagedTenantCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IEncryptionService _encryptionService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteHostManagedTenantCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    /// <param name="encryptionService">Decrypts the Host-facing Tenant identifier before persistence.</param>
    public DeleteHostManagedTenantCommandHandler(
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
    /// Soft deletes a Tenant and deactivates all related login credentials atomically.
    /// </summary>
    /// <param name="request">The Host-managed Tenant deletion command.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A successful response when the Tenant lifecycle transition completes.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the route identifier is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant is unavailable or already soft deleted.</exception>
    public async Task<ApiResponse<bool>> Handle(
        DeleteHostManagedTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            _commonRequestService,
            _unitOfWork.StoreProcedureRepository,
            request.ModuleId,
            request.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            request.EncryptedTenantId,
            hostContext.TenantEncryptionKey,
            _encryptionService);

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        var utcNow = DateTime.UtcNow;
        tenant.IsSoftDeleted = true;
        tenant.IsActive = false;
        tenant.SoftDeletedById = hostContext.HostUserId;
        tenant.DeletedDateTime = utcNow;
        tenant.UpdatedById = hostContext.HostUserId;
        tenant.UpdatedDateTime = utcNow;

        var transactionStarted = false;
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;

            // Keep the Tenant and every related credential state transition atomic.
            await _unitOfWork.TenantRepository
            .SoftDeleteTenantAndDeactivateCredentialsAsync(tenant, hostContext.HostUserId, utcNow, cancellationToken);
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

        return ApiResponse<bool>.Success(
            true,
            AppConstants.SuccessMessages.TenantDeletedSuccessfully);
    }

    #endregion
}

#endregion
