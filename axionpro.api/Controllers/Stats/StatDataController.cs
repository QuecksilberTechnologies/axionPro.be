// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Stat Data operations.
// ================================================================

using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.Features.StatsFeatures.EmployeesCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Stats
{
    /// <summary>
    /// Controller to manage all Asset Category related operations
    /// for Tenant Admins (Add, Update, Delete, GetAll).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatDataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public StatDataController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        ///Dashboard statistics.
        // Returns employee summary statistics for dashboard widgets
        /// <summary>
        /// Used-In-Angular: retrieves employee statistics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves employee counts.</para>
        /// <para>Handler flow: GetEmployeeCountsQuery is processed by GetEmployeesCountQueryHandler; operation(s): GetEmployeeCountsAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
        /// <para>Angular function(s): EmployeesApi.getEmployeeStatistics (app/core/services/employee-api.ts:150).</para>
        /// <para>Angular purpose: retrieves employee statistics.</para>
        /// <para>Integrated UI page(s): /app/admin-dashboard; /app/employees</para>
        /// <para>Angular UI component(s): DashboardAdmin (app/features/dashboard/dashboard-admin/dashboard-admin.ts); EmployeesStore (app/features/employees/employees.store.ts); Employees (app/features/employees/employees.ts)</para>
        /// </remarks>
        [HttpGet("Dashboard/Employees/Statistics")]
        public async Task<IActionResult> GetEmployeeDashboardSummaryAsync(
            [FromQuery] EmployeeCountRequestStatsSp dto)
        {
                var command = new GetEmployeeCountsQuery(dto);
                var result = await _mediator.Send(command);
               _logger.LogInfo("Employee data updated successfully.");
                return Ok(result);

        }



        /// <summary>
        /// Used-In-Angular: retrieves dashboard statistics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: performs the Angular function dashboard.</para>
        /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
        /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
        /// <para>Angular function(s): DashboardApi.getDashboardStatistics (app/core/services/dashboard-api.ts:59).</para>
        /// <para>Angular purpose: retrieves dashboard statistics.</para>
        /// <para>Integrated UI page(s): /app/admin-dashboard; /app/setting/billing; /app/assets/list; /app/employees</para>
        /// <para>Angular UI component(s): DashboardAdmin (app/features/dashboard/dashboard-admin/dashboard-admin.ts); SubscriptionStore (app/features/subscription/subscription.store.ts); Subscription (app/features/subscription/subscription.ts); CdkCloseMenuOnScroll (app/shared/directives/cdk-close-menu-on-scroll.ts); AssetsManagement (app/features/assets-management/assets-management.ts); EmployeeRoleCell (app/features/employees/employee-role-cell/employee-role-cell.ts); Employees (app/features/employees/employees.ts)</para>
        /// </remarks>
        [HttpGet("Manager/Statistics/Dashboard/get")]
        public async Task<IActionResult> dashboard([FromQuery] GetSummaryRequestDTO request)
        {
            EmployeeStats employee = new EmployeeStats()
            {
                TotalEmployees = 4,
                NewHiresThisMonth = 1,
                OpenPositions = 10,
                PendingApprovals = 3
            };

                return Ok(employee);

        }


        /// <summary>
        /// Used-In-Angular: retrieves asset statistics.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: performs the Angular function asset.</para>
        /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
        /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
        /// <para>Angular function(s): DashboardApi.getAssetStatistics (app/core/services/dashboard-api.ts:66).</para>
        /// <para>Angular purpose: retrieves asset statistics.</para>
        /// <para>Integrated UI page(s): /app/assets/list</para>
        /// <para>Angular UI component(s): AssetsManagementStore (app/features/assets-management/assets-management.store.ts); AssetsManagement (app/features/assets-management/assets-management.ts)</para>
        /// </remarks>
        [HttpGet("Manager/Statistic/Asset")]
        public async Task<IActionResult> Asset([FromQuery] GetSummaryRequestDTO request)
        {
            AssetStats assetStats = new AssetStats()
            {
                TotalAssets = 12,
                AssignedAssets = 0,
                AvailableAssets = 18,
                UnderMaintenance = 9
            };


                return Ok(assetStats);

        }








    }
}
