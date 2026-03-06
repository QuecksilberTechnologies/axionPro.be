using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class InsurancePolicy
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public string InsurancePolicyName { get; set; } = null!;

    public string InsurancePolicyNumber { get; set; } = null!;

    public string? ProviderName { get; set; }

    public DateTime? StartDate { get; set; }
    public int PolicyTypeId { get; set; }   // FK


    public DateTime? EndDate { get; set; }

    public string? AgentName { get; set; }

    public string? AgentContactNumber { get; set; }

    public string? AgentOfficeNumber { get; set; }

    // 🌍 Coverage Rules (NEW – UAE Ready)
    public bool EmployeeAllowed { get; set; }
    public int MaxSpouseAllowed { get; set; }
    public int MaxChildAllowed { get; set; }
    public bool ParentsAllowed { get; set; }
    public bool InLawsAllowed { get; set; }

    public int? CountryId { get; set; }

    // 🔘 Status
    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    // 🔹 Audit
    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    // 🔗 Navigation (optional but clean)
    public virtual Country? Country { get; set; }
    public virtual PolicyType PolicyType { get; set; } = null!;
    public virtual ICollection<PolicyTypeInsuranceMapping> PolicyTypeInsuranceMappings { get; set; } = new List<PolicyTypeInsuranceMapping>();
    public virtual ICollection<InsurancePolicyDocument> InsurancePolicyDocuments { get; set; } = new List<InsurancePolicyDocument>();
    public virtual ICollection<EmployeeInsuranceMapping> EmployeeInsuranceMappings { get; set; } = new List<EmployeeInsuranceMapping>();


}
