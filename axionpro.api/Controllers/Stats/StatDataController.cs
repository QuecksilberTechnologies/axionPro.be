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
        /// Get Employee Dashboard Summary.
        /// </summary>
        /// <remarks>
        /// Handles the request to get employee dashboard summary.
        /// </remarks>
        /// <param name="dto">The query parameters used to get employee dashboard summary.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// dashboard.
        /// </summary>
        /// <remarks>
        /// Handles the request to dashboard.
        /// </remarks>
        /// <param name="request">The query parameters used to dashboard.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
        /// Asset.
        /// </summary>
        /// <remarks>
        /// Handles the request to asset.
        /// </remarks>
        /// <param name="request">The query parameters used to asset.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
