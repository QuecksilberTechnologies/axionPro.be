// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines client-editable values for creating a direct SubModule.
// ============================================================================

using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Module.SubModule
{
    /// <summary>
    /// Captures the values a Host user may provide to create a direct child module.
    /// </summary>
    /// <remarks>
    /// Tenant ownership, actor, audit, identity, and child hierarchy flags are established by the server.
    /// Legacy context fields remain only to preserve existing request compatibility and are ignored by SubModule CRUD.
    /// </remarks>
    public class CreateSubModuleRequestDTO
    {
        #region Legacy Compatibility Fields

        /// <summary>Gets or sets the legacy employee identifier supplied by older clients.</summary>
        /// <remarks>The authenticated Host actor is authoritative; this value is ignored.</remarks>
        [Obsolete("The authenticated Host actor determines the module audit user.")]
        public long EmployeeId { get; set; }

        /// <summary>Gets or sets the legacy tenant identifier supplied by older clients.</summary>
        /// <remarks>The parent Header Module determines tenant ownership; this value is ignored.</remarks>
        [Obsolete("Tenant ownership is inherited from the selected Parent Module.")]
        public long TenantId { get; set; }

        /// <summary>Gets or sets the legacy role identifier supplied by older clients.</summary>
        /// <remarks>SubModule CRUD does not use a client-supplied role identifier.</remarks>
        [Obsolete("Authorization is determined from the authenticated request context.")]
        public int RoleId { get; set; }

        /// <summary>Gets or sets the legacy leaf-node value supplied by older clients.</summary>
        /// <remarks>Direct SubModules are always persisted as leaf nodes; this value is ignored.</remarks>
        [Obsolete("SubModule hierarchy is established by the server.")]
        public bool? IsLeafNode { get; set; }

        /// <summary>Provides compatibility with the existing legacy UI-display property casing.</summary>
        [Obsolete("Use IsModuleDisplayInUI.")]
        public bool IsModuleDisplayInUi
        {
            get => IsModuleDisplayInUI;
            set => IsModuleDisplayInUI = value;
        }

        #endregion

        #region Editable Module Fields

        /// <summary>Gets or sets the direct parent Header Module identifier.</summary>
        [Range(1, int.MaxValue)]
        public int ParentModuleId { get; set; }

        /// <summary>Gets or sets the module code.</summary>
        [MaxLength(50)]
        public string ModuleCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the module name.</summary>
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>Gets or sets the UI display name.</summary>
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>Gets or sets the URL path used to open the module.</summary>
        [MaxLength(500)]
        public string? URLPath { get; set; }

        /// <summary>Gets or sets whether the module is shown in the UI.</summary>
        public bool IsModuleDisplayInUI { get; set; }

        /// <summary>Gets or sets whether the direct child represents the common menu.</summary>
        public bool IsCommonMenu { get; set; }

        /// <summary>Gets or sets the required requested module scope.</summary>
        public short ModuleScope { get; set; }

        /// <summary>Gets or sets whether the new child module is active.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Gets or sets the web icon path.</summary>
        [MaxLength(255)]
        public string? ImageIconWeb { get; set; }

        /// <summary>Gets or sets the mobile icon path.</summary>
        [MaxLength(255)]
        public string? ImageIconMobile { get; set; }

        /// <summary>Gets or sets the display order.</summary>
        public int? ItemPriority { get; set; }

        /// <summary>Gets or sets an optional operational remark.</summary>
        [MaxLength(200)]
        public string? Remark { get; set; }

        #endregion
    }
}
