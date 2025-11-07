using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InsurancePolicy
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public string InsurancePolicyName { get; set; } = null!;

    public string InsurancePolicyNumber { get; set; } = null!;

    public string? CoverageType { get; set; }

    public string? ProviderName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? AgentName { get; set; }

    public string? AgentContactNumber { get; set; }

    public string? AgentOfficeNumber { get; set; }

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }
}
