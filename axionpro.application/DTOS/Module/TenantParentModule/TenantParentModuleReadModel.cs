// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents raw Tenant entitlement and global Module metadata returned from Tenant Parent Module persistence queries.
// ================================================================

namespace axionpro.application.DTOS.Module.TenantParentModule;

/// <summary>
/// Represents the repository-safe Tenant entitlement record used before Host-facing Tenant identifier protection.
/// </summary>
public sealed class TenantParentModuleReadModel
{
    /// <summary>Gets or sets the numeric Tenant identifier from persistence.</summary>
    public long TenantId { get; set; }

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

    /// <summary>Gets or sets the preserved Tenant entitlement parent global Module identifier.</summary>
    public int? ParentModuleId { get; set; }

    /// <summary>Gets or sets whether the Tenant entitlement represents a leaf Module.</summary>
    public bool? IsLeafNode { get; set; }

    /// <summary>Gets or sets whether the Tenant has this Module enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Gets or sets the global Module scope.</summary>
    public short ModuleScope { get; set; }

    /// <summary>Gets or sets direct entitled Header children.</summary>
    public List<TenantParentModuleReadModel> Children { get; set; } = new();
}
