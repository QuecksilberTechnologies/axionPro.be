namespace axionpro.domain.Entity;

/// <summary>Tenant-wide employee self-service defaults for operational sections.</summary>
public sealed class TenantEmployeeSectionDefault
{
    public long TenantId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public bool IsEditAllowed { get; set; }
    public long UpdatedById { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
