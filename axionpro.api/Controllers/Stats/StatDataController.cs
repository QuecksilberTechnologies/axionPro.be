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
        /// Supports the Angular UI flow for get employee dashboard summary async.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves employee statistics.</para>
        /// <para>Angular page(s): /app/admin-dashboard; /app/employees.</para>
        /// <para>Angular API service call(s): EmployeesApi.getEmployeeStatistics (app/core/services/employee-api.ts:146).</para>
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
        /// Supports the Angular UI flow for dashboard.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves dashboard statistics.</para>
        /// <para>Angular page(s): /app/admin-dashboard; /app/setting/billing.</para>
        /// <para>Angular API service call(s): DashboardApi.getDashboardStatistics (app/core/services/dashboard-api.ts:56).</para>
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
        /// Supports the Angular UI flow for asset.
        /// </summary>
        /// <remarks>
        /// <para>Angular purpose: retrieves asset statistics.</para>
        /// <para>Angular page(s): /app/assets/list.</para>
        /// <para>Angular API service call(s): DashboardApi.getAssetStatistics (app/core/services/dashboard-api.ts:63).</para>
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
