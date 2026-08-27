// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles Host Super Admin Tenant Parent Module status cascades.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.TenantParentModule;
using axionpro.application.Exceptions;
using axionpro.application.Features.ModuleCmd.TenantParent.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.ModuleCmd.TenantParent.Handlers;

#region Status Command

/// <summary>
/// Represents the Host-managed request to update one Tenant Parent Module status cascade.
/// </summary>
public sealed class UpdateTenantParentModuleStatusCommand
    : IRequest<ApiResponse<TenantParentModuleResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTenantParentModuleStatusCommand"/> class.
    /// </summary>
    /// <param name="moduleId">The global Module identifier exposed by the API.</param>
    /// <param name="requestDTO">The encrypted Tenant identifier and requested enabled state.</param>
    public UpdateTenantParentModuleStatusCommand(
        int moduleId,
        UpdateTenantParentModuleStatusRequestDTO? requestDTO)
    {
        ModuleId = moduleId;
        RequestDTO = requestDTO;
    }

    /// <summary>Gets the global Module identifier exposed by the API.</summary>
    public int ModuleId { get; }

    /// <summary>Gets the encrypted Tenant identifier and requested enabled state.</summary>
    public UpdateTenantParentModuleStatusRequestDTO? RequestDTO { get; }
}

#endregion

#region Status Command Handler

/// <summary>
/// Updates a Tenant-entitled Main Parent or Sub-Parent Header Module and all of its descendants for an authenticated Host Super Admin.
/// </summary>
public sealed class UpdateTenantParentModuleStatusCommandHandler
    : TenantParentModuleQueryHandlerBase,
      IRequestHandler<UpdateTenantParentModuleStatusCommand, ApiResponse<TenantParentModuleResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTenantParentModuleStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant entitlement persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host Super Admin and Host encryption-key context.</param>
    /// <param name="idEncoderService">Encodes Tenant identifiers at the Host API boundary.</param>
    public UpdateTenantParentModuleStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
        : base(unitOfWork, commonRequestService, idEncoderService)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TenantParentModuleResponseDTO>> Handle(
        UpdateTenantParentModuleStatusCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request?.RequestDTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var context = await ResolveTenantRequestAsync(dto.TenantId, cancellationToken);
        if (request.ModuleId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        await UnitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var module = await UnitOfWork.TenantParentModuleRepository.StageStatusCascadeAsync(
                context.TenantId,
                request.ModuleId,
                dto.IsActive,
                // Tenant entitlement rows use the Tenant identifier as their audit actor.
                context.TenantId,
                DateTime.UtcNow,
                cancellationToken);

            if (module is null)
            {
                throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResponse<TenantParentModuleResponseDTO>.Success(
                MapResponse(module, context.TenantEncryptionKey),
                "Tenant Parent Module status updated successfully.");
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

#endregion
