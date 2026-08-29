// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Adds missing Tenant entitlement snapshot rows from the current active subscription plan.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

/// <summary>
/// Represents the explicit Host request to add missing entitlement snapshot records for a Tenant's active plan.
/// </summary>
public sealed class SynchronizeTenantPlanEntitlementsCommand(
    SynchronizeTenantPlanEntitlementsRequestDTO? requestDTO)
    : IRequest<ApiResponse<SynchronizeTenantPlanEntitlementsResponseDTO>>
{
    /// <summary>Gets the submitted encrypted Tenant selection and Host permission metadata.</summary>
    public SynchronizeTenantPlanEntitlementsRequestDTO? RequestDTO { get; } = requestDTO;
}

/// <summary>
/// Handles the explicit, additive synchronization from active plan mappings to Tenant entitlement snapshot tables.
/// </summary>
public sealed class SynchronizeTenantPlanEntitlementsCommandHandler
    : IRequestHandler<SynchronizeTenantPlanEntitlementsCommand, ApiResponse<SynchronizeTenantPlanEntitlementsResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    /// <summary>Initializes a new instance of the entitlement synchronization handler.</summary>
    public SynchronizeTenantPlanEntitlementsCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    /// <summary>
    /// Validates Host access and adds only missing active plan module and operation snapshot rows in one transaction.
    /// </summary>
    public async Task<ApiResponse<SynchronizeTenantPlanEntitlementsResponseDTO>> Handle(
        SynchronizeTenantPlanEntitlementsCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request?.RequestDTO
            ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);

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
            .GetHostManagedTenantByIdAsync(tenantId, cancellationToken);
        if (tenant is null || !tenant.IsActive)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var syncResult = await _unitOfWork.TenantModuleConfigurationRepository
                .SynchronizeMissingActivePlanEntitlementsAsync(
                    tenantId,
                    hostContext.HostUserId,
                    cancellationToken)
                ?? throw new NotFoundException("No active subscription plan was found for this Tenant.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResponse<SynchronizeTenantPlanEntitlementsResponseDTO>.Success(
                new SynchronizeTenantPlanEntitlementsResponseDTO
                {
                    TenantId = HostTenantIdentifierProtector.Encrypt(
                        tenantId,
                        hostContext.TenantEncryptionKey,
                        _idEncoderService),
                    SubscriptionPlanId = syncResult.SubscriptionPlanId,
                    SourceModuleCount = syncResult.SourceModuleCount,
                    AddedModuleCount = syncResult.AddedModuleCount,
                    ExistingModuleCount = syncResult.ExistingModuleCount,
                    SourceOperationCount = syncResult.SourceOperationCount,
                    AddedOperationCount = syncResult.AddedOperationCount,
                    ExistingOperationCount = syncResult.ExistingOperationCount,
                    Modules = syncResult.Modules.ToList(),
                    Operations = syncResult.Operations.ToList()
                },
                AppConstants.SuccessMessages.TenantPlanEntitlementsSynchronizedSuccessfully);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
