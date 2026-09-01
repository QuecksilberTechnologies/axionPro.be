// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Tenant employee authorization and operational-navigation bootstrap endpoints.
// ================================================================

using axionpro.application.Features.UserAccessCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.TenantUserAccess;

/// <summary>
/// Exposes the current Tenant employee operational-access bootstrap.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TenantUserAccessController : ControllerBase
{
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantUserAccessController"/> class.
    /// </summary>
    /// <param name="mediator">Dispatches Tenant access bootstrap queries.</param>
    public TenantUserAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Tenant Access Endpoints
    /// <summary>
    /// Used-In-Angular: retrieves tenant bootstrap.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves tenant user access.</para>
    /// <para>Handler flow: GetTenantUserAccessQuery is processed by GetTenantUserAccessQueryHandler; operation(s): GetEmployeeRolesWithDetailsByIdAsync, GetCurrentTenantOperationalAccessAsync.</para>
    /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantUserAccessResponseDTO: OperationalMenus (IReadOnlyCollection&lt;MainModuleDto&gt;)</para>
    /// <para>Angular function(s): AccessBootstrapApi.getTenantBootstrap (app/core/services/access-bootstrap-api.ts:34).</para>
    /// <para>Angular purpose: retrieves tenant bootstrap.</para>
    /// <para>Integrated UI page(s): /app/policies/attendance-policies; /auth/login; /app/admin-dashboard; /app/departments; /app/designations; /app/device-masters; /app/modules/module-operations; /app/modules/operations</para>
    /// <para>Angular UI component(s): CurrentUserPermissionsStore (app/core/stores/current-user-permissions.store.ts); hasModuleOperationGuard (app/core/guards/has-module-operation-guard.ts); hasModulePermissionGuard (app/core/guards/has-module-permission-guard.ts); superAdminGuard (app/core/guards/super-admin-guard.ts); AttendancePolicies (app/features/attendance-policies/attendance-policies.ts); Login (app/features/authentication/login/login.ts); DashboardAdmin (app/features/dashboard/dashboard-admin/dashboard-admin.ts); LocationsCard (app/features/dashboard/dashboard-admin/locations-card/locations-card.ts)</para>
    /// </remarks>

    [HttpGet("bootstrap")]
    public async Task<IActionResult> GetBootstrap(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantUserAccessQuery(), cancellationToken);
        return Ok(result);
    }

    #endregion
}
