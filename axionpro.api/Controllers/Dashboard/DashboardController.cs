 
using axionpro.application.DTOS.DashboardSummaryDTO;
using axionpro.application.Features.AssetFeatures.Category.Commands;

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
    [Route("api/[controller]/admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public DashboardController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
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
