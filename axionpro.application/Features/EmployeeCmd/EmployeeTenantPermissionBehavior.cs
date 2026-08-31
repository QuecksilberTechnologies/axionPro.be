// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Enforces the trusted Tenant role-permission contract for EmployeeCmd requests.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd;

/// <summary>
/// Applies the existing Tenant request and database-backed role permission
/// validation consistently to every EmployeeCmd command and query.
/// </summary>
/// <typeparam name="TRequest">The MediatR command or query.</typeparam>
/// <typeparam name="TResponse">The command or query response.</typeparam>
public sealed class EmployeeTenantPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    ILogger<EmployeeTenantPermissionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // This behavior is deliberately limited to EmployeeCmd. Host pages do
        // not use these endpoints and therefore cannot receive a Host bypass.
        var requestNamespace = typeof(TRequest).Namespace;
        if (requestNamespace?.StartsWith(
                "axionpro.application.Features.EmployeeCmd",
                StringComparison.Ordinal) != true)
        {
            return await next();
        }

        var permissionRequest = ResolvePermissionRequest(request);
        if (permissionRequest is null)
        {
            logger.LogWarning(
                "Employee request {EmployeeRequest} did not provide ModuleId and OperationId.",
                typeof(TRequest).Name);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var expectedModuleCode = ResolveExpectedModuleCode();
        if (string.IsNullOrWhiteSpace(expectedModuleCode))
        {
            logger.LogWarning(
                "No expected module-code mapping exists for Employee request {EmployeeRequest}.",
                typeof(TRequest).FullName);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var activeModuleCode = await commonRequestService
            .GetActiveModuleCodeAsync(permissionRequest.ModuleId);
        if (!string.Equals(activeModuleCode, expectedModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Employee module-code mismatch for {EmployeeRequest}. ModuleId: {ModuleId}, ActiveModuleCode: {ActiveModuleCode}, ExpectedModuleCode: {ExpectedModuleCode}",
                typeof(TRequest).Name,
                permissionRequest.ModuleId,
                activeModuleCode,
                expectedModuleCode);
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }

        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        long tenantId = validation.TenantId;
        long userEmployeeId = validation.LoggedInEmployeeId;
        int tokenRoleId = validation.RoleId;
        if (tenantId <= 0 || userEmployeeId <= 0 || tokenRoleId <= 0)
        {
            logger.LogWarning(
                "Invalid Tenant authorization context for {EmployeeRequest}. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                typeof(TRequest).Name,
                tenantId,
                userEmployeeId,
                tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // The stored procedure uses the current role assignments; the JWT role
        // is only a context value and is never the final authorization source.
        var permissionResult = await unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                userEmployeeId,
                tokenRoleId,
                permissionRequest.ModuleId,
                permissionRequest.OperationId,
                cancellationToken);

        return permissionResult.ResultCode switch
        {
            1 => await next(),
            0 => throw CreatePermissionDeniedException(),
            -1 or -2 => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized),
            _ => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized)
        };
    }

    /// <summary>
    /// Creates the standard forbidden response after logging a database-confirmed
    /// permission denial. Invalid or stale authentication contexts remain 401.
    /// </summary>
    private static ForbiddenAccessException CreatePermissionDeniedException() =>
        new(AppConstants.ErrorMessages.PermissionDenied);

    /// <summary>
    /// Retrieves the DTO carrying ModuleId and OperationId without changing
    /// existing endpoint routes or response contracts.
    /// </summary>
    private static PermissionRequestDTO? ResolvePermissionRequest(TRequest request)
    {
        foreach (var propertyName in new[] { "DTO", "Filter", "PermissionRequest" })
        {
            var property = typeof(TRequest).GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property?.GetValue(request) is PermissionRequestDTO permissionRequest)
            {
                return permissionRequest;
            }

            var field = typeof(TRequest).GetField(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            if (field?.GetValue(request) is PermissionRequestDTO fieldPermissionRequest)
            {
                return fieldPermissionRequest;
            }
        }

        return null;
    }

    /// <summary>
    /// Resolves the Employee leaf-module code that owns the request. Header modules are never
    /// used for authorization; every request binds to the UI leaf module from the approved list.
    /// </summary>
    private static string? ResolveExpectedModuleCode()
    {
        var requestNamespace = typeof(TRequest).Namespace ?? string.Empty;

        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.BankInfo", StringComparison.Ordinal))
            return "EMP_BANK";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.Contact", StringComparison.Ordinal))
            return "EMP_CONTACT";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.DependentInfo", StringComparison.Ordinal))
            return "EMP_DEPENDENTS";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.EducationInfo", StringComparison.Ordinal))
            return "EMP_EDUCATION";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.ExperienceInfo", StringComparison.Ordinal))
            return "EMP_EXPERIENCE";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.InsuranceInfo", StringComparison.Ordinal))
            return "EMP_INSURANCE";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.IdentitiesInfo", StringComparison.Ordinal))
            return "EMP_IDENTITY";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment", StringComparison.Ordinal))
            return "EMP_DEVICES";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.ResetPassword", StringComparison.Ordinal))
            return "EMP_PASSWORD_MANAGEMENT";
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo", StringComparison.Ordinal))
        {
            var requestName = typeof(TRequest).Name;
            if (requestName.Contains("EmployeeLocationAssignment", StringComparison.Ordinal))
                return "EMP_WORK_LOCATIONS";
            if (requestName.Contains("EmployeeWorkArrangement", StringComparison.Ordinal))
                return "EMP_WORK_ARRANGEMENT";
            if (requestName.Contains("EmployeeWorkPattern", StringComparison.Ordinal))
                return "EMP_WORK_PATTERN";
            if (requestName.Contains("EmployeeWorkModeOverride", StringComparison.Ordinal))
                return "EMP_OVERRIDES";

            return null;
        }
        if (requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.UpdateStatus", StringComparison.Ordinal) ||
            requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.UpdateVerification", StringComparison.Ordinal))
            return "EMP_LIST";

        if (!requestNamespace.StartsWith("axionpro.application.Features.EmployeeCmd.EmployeeBase", StringComparison.Ordinal))
        {
            return null;
        }

        return typeof(TRequest).Name switch
        {
            "CreateBaseEmployeeInfoCommand" or
            "GetAllEmployeeInfoQuery" or
            "GetEmployeeSummaryQuery" or
            "DeleteEmployeeQuery" or
            "ActivateAllEmployeeQuery" or
            "UpdateSectionBulkCommand" => "EMP_LIST",

            "GetBaseEmployeeInfoQuery" or
            "UpdateEmployeeCommand" or
            "UpdateBaseEmployeeByAdminCommand" or
            "GetEmployeeImageQuery" or
            "CreateEmployeeImageCommand" or
            "UpdateProfileImageCommand" or
            "GetEmployeeProfileSummaryQuery" or
            "GetEmployeeProfileStatusQuery" => "EMP_OVERVIEW",

            _ => null
        };
    }
}
