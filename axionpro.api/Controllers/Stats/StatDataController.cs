using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.Features.AssetFeatures.Category.Commands;
using axionpro.application.Features.StatsFeatures.EmployeesCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.Dashboard
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
        [HttpGet("Dashboard/Employees/Statistics")]
        public async Task<IActionResult> GetEmployeeDashboardSummaryAsync(
            [FromQuery] EmployeeCountRequestStatsSp dto)
        {
            try
            {
                _logger.LogInfo(
                    "Fetching employee dashboard summary. TenantId: {TenantId}");

                var command = new GetEmployeeCountsQuery(dto);
                var result = await _mediator.Send(command);

                if (!result.IsSucceeded)
                {
                    _logger.LogInfo("Error while fetching employee dashboard summary");

                    return BadRequest(result);
                }

                _logger.LogInfo("Employee data updated successfully.");
                return Ok(result);

              

              

                 
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Error while fetching employee dashboard summary");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving employee dashboard summary."
                });
            }
        }


    
        /// <summary>
        ///Dashboard statistics.
        /// </summary>
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
            
            try
            {
                _logger.LogInfo("Fetching all EmployeeStat for tenant...");
               
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching EmployeeStat : {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while retrieving EmployeeStat ." });
            }
        }

     
        /// <summary>
        /// Asset-related dashboard statistics.
        /// </summary>
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
           
            try
            {
                _logger.LogInfo("Fetching all asset categories for tenant...");

                return Ok(assetStats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching asset categories: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while retrieving asset categories." });
            }
        }








    }
}
