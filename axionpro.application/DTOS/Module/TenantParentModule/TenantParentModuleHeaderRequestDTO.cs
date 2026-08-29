// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines encrypted-Tenant filters for Tenant Parent Module header retrieval.
// ================================================================

namespace axionpro.application.DTOS.Module.TenantParentModule;

/// <summary>
/// Represents the Host-managed filters used to retrieve a Tenant's entitled module-header tree.
/// </summary>
public sealed class TenantParentModuleHeaderRequestDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the optional Tenant entitlement enabled-state filter.
    /// </summary>
    public bool? IsEnabled { get; set; }
}
