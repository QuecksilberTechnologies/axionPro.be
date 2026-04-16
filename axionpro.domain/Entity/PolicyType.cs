using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PolicyType
{
    public int Id { get; set; }

    public long? TenantId { get; set; }

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

    public bool IsStructured { get; set; } = false; 

    public int PolicyTypeEnumVal { get; set; }

    public bool HasPolicyDocUploaded { get; set; }

    public virtual ICollection<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignation { get; set; } = new List<AccoumndationAllowancePolicyByDesignation>();

    public virtual ICollection<EmployeePolicyEnrollment> EmployeePolicyEnrollment { get; set; } = new List<EmployeePolicyEnrollment>();

    public virtual ICollection<InsurancePolicy> InsurancePolicy { get; set; } = new List<InsurancePolicy>();

    public virtual ICollection<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignation { get; set; } = new List<MealAllowancePolicyByDesignation>();

    public virtual ICollection<PolicyTypeDocument> PolicyTypeDocument { get; set; } = new List<PolicyTypeDocument>();

    public virtual ICollection<PolicyTypeInsuranceMapping> PolicyTypeInsuranceMapping { get; set; } = new List<PolicyTypeInsuranceMapping>();

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignation { get; set; } = new List<TravelAllowancePolicyByDesignation>();

    public virtual ICollection<UnStructuredPolicyTypeMappingWithEmployeeType> UnStructuredPolicyTypeMappingWithEmployeeType { get; set; } = new List<UnStructuredPolicyTypeMappingWithEmployeeType>();
}
