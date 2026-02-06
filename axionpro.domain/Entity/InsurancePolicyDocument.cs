using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InsurancePolicyDocument
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public int InsurancePolicyId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int FileType { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public virtual InsurancePolicy InsurancePolicy { get; set; } = null!;
    public virtual ICollection<InsurancePolicyDocument> InsurancePolicyDocuments { get; set; } = new List<InsurancePolicyDocument>();
 
    public virtual PolicyType? PolicyType { get; set; }

    public virtual ICollection<PolicyTypeInsuranceMapping> PolicyTypeInsuranceMappings { get; set; } = new List<PolicyTypeInsuranceMapping>();

}
