using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class CompanyPolicyDocument
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public int PolicyTypeId { get; set; }

    public string DocumentTitle { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;
       
    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public virtual PolicyType PolicyType { get; set; } = null!;
}
