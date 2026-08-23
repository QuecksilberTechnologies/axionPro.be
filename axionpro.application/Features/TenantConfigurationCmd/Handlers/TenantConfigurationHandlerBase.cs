// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides common authenticated Tenant context and response helpers for TenantConfiguration handlers.
// ================================================================

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Handlers;

/// <summary>Provides common Tenant authentication and paging-response behavior for this module.</summary>
public abstract class TenantConfigurationHandlerBase
{
    /// <summary>Initializes common handler dependencies.</summary>
    protected TenantConfigurationHandlerBase(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<TenantConfigurationHandlerBase> logger)
    {
        UnitOfWork = unitOfWork;
        CommonRequestService = commonRequestService;
        Logger = logger;
    }

    /// <summary>Gets the unit of work used by TenantConfiguration handlers.</summary>
    protected IUnitOfWork UnitOfWork { get; }

    /// <summary>Gets the service that validates the authenticated Tenant principal.</summary>
    protected ICommonRequestService CommonRequestService { get; }

    /// <summary>Gets the structured logger for TenantConfiguration activity.</summary>
    protected ILogger<TenantConfigurationHandlerBase> Logger { get; }

    /// <summary>Resolves the trusted Tenant and employee audit actor from the current request.</summary>
    /// <returns>The authenticated Tenant identifier and employee identifier.</returns>
    protected async Task<(long TenantId, long ActorId)> ValidateTenantAsync()
    {
        var validation = await CommonRequestService.ValidateRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        return (validation.TenantId, validation.LoggedInEmployeeId);
    }

    /// <summary>Creates a flattened paginated API response without data nesting.</summary>
    protected static ApiResponse<List<TResponse>> Paged<TResponse>(List<TResponse> data, int pageNumber, int pageSize, int totalRecords, string message) =>
        ApiResponse<List<TResponse>>.SuccessPaginated(data, pageNumber, pageSize, totalRecords, (int)Math.Ceiling(totalRecords / (double)pageSize), message);
}
