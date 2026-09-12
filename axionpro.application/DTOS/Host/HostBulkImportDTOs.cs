using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOS.Host;

/// <summary>Host upload. TenantId is required only for card inventory and applies to every row.</summary>
public sealed class HostBulkImportPreviewRequestDTO : BulkImportPreviewRequestDTO
{
    public string? TenantId { get; set; }
}

/// <summary>Host job action. Supply the same selected TenantId for card jobs.</summary>
public sealed class HostBulkImportJobRequestDTO : BulkImportJobRequestDTO
{
    public string? TenantId { get; set; }
}
