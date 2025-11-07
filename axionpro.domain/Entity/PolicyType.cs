using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PolicyType
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public string PolicyName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDelete { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdateById { get; set; }

    public DateTime? UpdateDateTime { get; set; }

    public long? SoftDeleteById { get; set; }

    public DateTime? SoftDeleteDateTime { get; set; }

    public virtual ICollection<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignations { get; set; } = new List<AccoumndationAllowancePolicyByDesignation>();

    public virtual ICollection<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignations { get; set; } = new List<MealAllowancePolicyByDesignation>();

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignations { get; set; } = new List<TravelAllowancePolicyByDesignation>();
}
