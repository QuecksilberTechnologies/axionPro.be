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

    public bool IsActive { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public virtual ICollection<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignations { get; set; } = new List<AccoumndationAllowancePolicyByDesignation>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<EmployeeManagerMapping> EmployeeManagerMappings { get; set; } = new List<EmployeeManagerMapping>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignations { get; set; } = new List<MealAllowancePolicyByDesignation>();

 

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignations { get; set; } = new List<TravelAllowancePolicyByDesignation>();
}
