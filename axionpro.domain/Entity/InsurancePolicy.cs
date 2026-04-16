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

    public bool EmployeeAllowed { get; set; }

    public int MaxSpouseAllowed { get; set; }

    public int MaxChildAllowed { get; set; }

    public int CountryId { get; set; }

    public bool ParentsAllowed { get; set; }

    public bool InLawsAllowed { get; set; }

    public int PolicyTypeId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<EmployeePolicyEnrollment> EmployeePolicyEnrollment { get; set; } = new List<EmployeePolicyEnrollment>();

    public virtual ICollection<InsurancePolicyDocument> InsurancePolicyDocument { get; set; } = new List<InsurancePolicyDocument>();

    public virtual PolicyType? PolicyType { get; set; }

    public virtual ICollection<PolicyTypeInsuranceMapping> PolicyTypeInsuranceMapping { get; set; } = new List<PolicyTypeInsuranceMapping>();
}
