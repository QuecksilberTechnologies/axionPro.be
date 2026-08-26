// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines encrypted-Tenant filters for Tenant Parent Module retrieval by global Module identifier.
// ================================================================

namespace axionpro.application.DTOS.Module.TenantParentModule;

/// <summary>
/// Represents the Host-managed Tenant and Module-scope filters required to retrieve one entitled Header Module.
/// </summary>
public sealed class TenantParentModuleByIdRequestDTO
{
    /// <summary>
    /// Gets or sets the encrypted Tenant identifier.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the required global Module scope.
    /// </summary>
    public short ModuleScope { get; set; }
}
