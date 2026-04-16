using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Designation
{
    public int Id { get; set; }

    public long TenantId { get; set; }

    public int DepartmentId { get; set; }

    public string DesignationName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = false;

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool IsSoftDeleted { get; set; } = false;

    public DateTime? SoftDeletedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public virtual ICollection<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignation { get; set; } = new List<AccoumndationAllowancePolicyByDesignation>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMapping { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignation { get; set; } = new List<MealAllowancePolicyByDesignation>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignation { get; set; } = new List<TravelAllowancePolicyByDesignation>();
}
