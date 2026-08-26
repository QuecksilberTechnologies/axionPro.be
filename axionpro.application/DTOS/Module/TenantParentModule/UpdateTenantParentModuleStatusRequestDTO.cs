// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-managed Tenant Parent Module status update request.
// ================================================================

namespace axionpro.application.DTOS.Module.TenantParentModule;

/// <summary>
/// Represents the encrypted Tenant identifier and requested enabled state for one Tenant Parent Module status cascade.
/// </summary>
public sealed class UpdateTenantParentModuleStatusRequestDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the requested enabled state for the target Header Module and its descendants.
    /// </summary>
    public bool IsActive { get; set; }
}
