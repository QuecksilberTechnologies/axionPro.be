// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-facing Tenant Parent Module response with an encrypted Tenant identifier.
// ================================================================

namespace axionpro.application.DTOS.Module.TenantParentModule;

/// <summary>
/// Represents one Tenant-entitled Header Module and its direct entitled Header children.
/// </summary>
public sealed class TenantParentModuleResponseDTO
{
    /// <summary>Gets or sets the encrypted Tenant identifier.</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>Gets or sets the global Module identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the global Module code.</summary>
    public string? ModuleCode { get; set; }

    /// <summary>Gets or sets the global Module name.</summary>
    public string? ModuleName { get; set; }

    /// <summary>Gets or sets the global Module display name.</summary>
    public string? DisplayName { get; set; }

    /// <summary>Gets or sets the global Module URL path.</summary>
    public string? UrlPath { get; set; }

    /// <summary>Gets or sets the global Module web icon.</summary>
    public string? ImageIconWeb { get; set; }

    /// <summary>Gets or sets the global Module mobile icon.</summary>
    public string? ImageIconMobile { get; set; }

    /// <summary>Gets or sets the global Module display priority.</summary>
    public int? ItemPriority { get; set; }

    /// <summary>Gets or sets the preserved parent global Module identifier.</summary>
    public int? ParentModuleId { get; set; }

    /// <summary>Gets or sets whether this Module is a leaf Module.</summary>
    public bool? IsLeafNode { get; set; }

    /// <summary>Gets or sets whether the Tenant has this Module enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Gets or sets the global Module scope.</summary>
    public short ModuleScope { get; set; }

    /// <summary>Gets or sets direct entitled Header children.</summary>
    public List<TenantParentModuleResponseDTO> Children { get; set; } = new();
}
