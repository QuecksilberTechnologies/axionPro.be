using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOS.DashboardSummaryDTO
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
