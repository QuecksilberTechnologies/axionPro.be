// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides common authenticated Tenant context and response helpers for TenantConfiguration handlers.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Handlers;

/// <summary>Provides common Tenant authentication and paging-response behavior for this module.</summary>
public abstract class TenantConfigurationHandlerBase
{
    private readonly IIdEncoderService? _idEncoderService;

    /// <summary>Initializes common handler dependencies.</summary>
    protected TenantConfigurationHandlerBase(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        ILogger<TenantConfigurationHandlerBase> logger,
        IIdEncoderService? idEncoderService = null)
    {
        UnitOfWork = unitOfWork;
        CommonRequestService = commonRequestService;
        Logger = logger;
        _idEncoderService = idEncoderService;
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
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        return (validation.TenantId, validation.LoggedInEmployeeId);
    }

    /// <summary>
    /// Resolves the trusted tenant request context for handlers that first need to load a
    /// record and then verify access against that record's owning employee.
    /// </summary>
    protected async Task<CommonDecodedResult> ValidateTenantDataAccessContextAsync()
    {
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success || validation.TenantId <= 0 || validation.LoggedInEmployeeId <= 0)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        return validation;
    }

    /// <summary>
    /// Enforces the central employee ownership rule after a target employee has been decoded or
    /// resolved from a tenant-scoped record. A denied target always returns the standard 403 path.
    /// </summary>
    protected async Task EnsureEmployeeDataAccessAsync(
        CommonDecodedResult validation,
        long targetEmployeeId,
        EmployeeDataAccessRequirement requirement,
        CancellationToken cancellationToken)
    {
        var hasAccess = await CommonRequestService.CanAccessEmployeeDataAsync(
            validation,
            targetEmployeeId,
            requirement,
            cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
    }

    /// <summary>
    /// Validates the tenant principal and decodes a required client-facing employee identifier.
    /// The encoded identifier is never passed to entities or repositories.
    /// </summary>
    protected async Task<(long TenantId, long ActorId, long EmployeeId)> ValidateTenantAndDecodeEmployeeIdAsync(
        string? encodedEmployeeId,
        EmployeeDataAccessRequirement requirement = EmployeeDataAccessRequirement.PersonalDetails,
        CancellationToken cancellationToken = default)
    {
        var context = await ValidateTenantAndDecodeOptionalEmployeeIdAsync(
            encodedEmployeeId,
            requirement,
            cancellationToken);
        if (!context.EmployeeId.HasValue)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        return (context.TenantId, context.ActorId, context.EmployeeId.Value);
    }

    /// <summary>
    /// Validates the tenant principal and decodes an optional employee filter when the client supplied one.
    /// </summary>
    protected async Task<(long TenantId, long ActorId, long? EmployeeId)> ValidateTenantAndDecodeOptionalEmployeeIdAsync(
        string? encodedEmployeeId,
        EmployeeDataAccessRequirement requirement = EmployeeDataAccessRequirement.PersonalDetails,
        CancellationToken cancellationToken = default)
    {
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success ||
            validation.TenantId <= 0 ||
            validation.LoggedInEmployeeId <= 0 ||
            string.IsNullOrWhiteSpace(validation.Claims.TenantEncriptionKey))
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        if (string.IsNullOrWhiteSpace(encodedEmployeeId))
        {
            return (validation.TenantId, validation.LoggedInEmployeeId, null);
        }

        if (_idEncoderService is null)
        {
            throw new InvalidOperationException("The employee identifier decoder is not configured.");
        }

        try
        {
            var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                encodedEmployeeId,
                validation.Claims.TenantEncriptionKey,
                _idEncoderService);

            if (employeeId <= 0)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
            }

            await EnsureEmployeeDataAccessAsync(
                validation,
                employeeId,
                requirement,
                cancellationToken);

            return (validation.TenantId, validation.LoggedInEmployeeId, employeeId);
        }
        catch (ValidationErrorException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }
    }

    /// <summary>
    /// Resolves the authenticated Tenant context and enforces the requested
    /// module operation using the current database role assignments.
    /// </summary>
    /// <param name="request">The request carrying the client-supplied module and operation identifiers.</param>
    /// <param name="cancellationToken">Token used to cancel the authorization operation.</param>
    /// <returns>The trusted Tenant identifier and employee audit actor.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the Tenant context is invalid, stale, or denied.</exception>
    protected async Task<(long TenantId, long ActorId)> ValidateTenantPermissionAsync(
        PermissionRequestDTO request,
        CancellationToken cancellationToken)
    {
        var validation = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        long tenantId = validation.TenantId;
        long actorId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || actorId <= 0 || tokenRoleId <= 0)
        {
            Logger.LogWarning(
                "Invalid Tenant authorization context. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId,
                actorId,
                tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var permissionResult = await UnitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                actorId,
                tokenRoleId,
                request.ModuleId,
                request.OperationId,
                cancellationToken);

        TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);
        return (tenantId, actorId);
    }

    /// <summary>Creates a flattened paginated API response without data nesting.</summary>
    protected static ApiResponse<List<TResponse>> Paged<TResponse>(List<TResponse> data, int pageNumber, int pageSize, int totalRecords, string message) =>
        ApiResponse<List<TResponse>>.SuccessPaginated(data, pageNumber, pageSize, totalRecords, (int)Math.Ceiling(totalRecords / (double)pageSize), message);
}
