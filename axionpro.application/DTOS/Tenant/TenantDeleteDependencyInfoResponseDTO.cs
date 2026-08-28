// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the read-only Tenant deletion dependency information response.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>Describes the transactional data groups that a future Tenant deletion workflow must process.</summary>
public sealed class TenantDeleteDependencyInfoResponseDTO
{
    public string TenantId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> TransactionalDataGroups { get; set; } = new();
}
