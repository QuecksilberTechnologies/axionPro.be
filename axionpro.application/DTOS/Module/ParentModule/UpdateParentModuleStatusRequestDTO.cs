// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines the active-state change requested for a Parent/Header Module.
// ============================================================================

namespace axionpro.application.DTOS.Module.ParentModule
{
    /// <summary>
    /// Captures the supported non-destructive status change for a Parent/Header Module.
    /// </summary>
    public class UpdateParentModuleStatusRequestDTO
    {
        /// <summary>
        /// Gets or sets the target active state.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
