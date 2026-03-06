using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PolicyTypeInsuranceMapping
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public int PolicyTypeId { get; set; }

    public int InsurancePolicyId { get; set; }

    public bool IsActive { get; set; }
   
    public bool? IsSoftDeleted { get; set; }
    public long? SoftDeleteById { get; set; }
    public DateTime? SoftDeleteDateTime { get; set; }


    public long? AddedById { get; set; }


    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual InsurancePolicy InsurancePolicy { get; set; } = null!;

    public virtual PolicyType PolicyType { get; set; } = null!;

}

