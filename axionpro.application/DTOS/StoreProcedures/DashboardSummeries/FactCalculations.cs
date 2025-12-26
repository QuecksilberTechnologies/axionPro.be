using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.EntityFrameworkCore;

namespace axionpro.application.DTOS.StoreProcedures.DashboardSummeries
{


    /// <summary>
    /// Represents  Employee statistics for the Admin Dashboard.
    /// </summary>
    /// 

    
    public class EmployeeStatsResponseDTO : BaseRequest
    {
        // 👥 Employee Section
        public EmployeeStats stat { get; set; }       
    }

    /// <summary>
    /// Represents  Asset statistics for the Admin Dashboard.
    /// </summary>
    /// 
    public class AssetStatResponseDTO
    {
        // 🧰 Asset Section
        public AssetStats stats { get; set; }
    }

    /// <summary>
    /// Employee-related dashboard statistics.
    /// </summary>
    public class EmployeeStats
    {
        public int TotalEmployees { get; set; }
        public int NewHiresThisMonth { get; set; }
        public int OpenPositions { get; set; }
        public int PendingApprovals { get; set; }
    }


    /// <summary>
    /// Employee-related dashboard statistics.
    /// </summary>
    /// 
    [Keyless]
    public class EmployeeCountResponseStatsSp
    {
          
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InActiveCount { get; set; }
        public int OnLeaveCount { get; set; }
    }

    public class EmployeeCountRequestStatsSp
    {

        public required string UserEmployeeId { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }

    /// <summary>
    /// Asset-related dashboard statistics.
    /// </summary>
    public class AssetStats
    {
        public int TotalAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int UnderMaintenance { get; set; }
    }
}
