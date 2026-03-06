using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeType
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? SoftDeletedDateTime { get; set; }

    public virtual ICollection<AccoumndationAllowancePolicyByDesignation> AccoumndationAllowancePolicyByDesignations { get; set; } = new List<AccoumndationAllowancePolicyByDesignation>();

    public virtual ICollection<EmployeeTypeBasicMenu> EmployeeTypeBasicMenus { get; set; } = new List<EmployeeTypeBasicMenu>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<EmployeesChangedTypeHistory> EmployeesChangedTypeHistoryNewEmployeeTypes { get; set; } = new List<EmployeesChangedTypeHistory>();

    public virtual ICollection<EmployeesChangedTypeHistory> EmployeesChangedTypeHistoryOldEmployeeTypes { get; set; } = new List<EmployeesChangedTypeHistory>();

    public virtual ICollection<MealAllowancePolicyByDesignation> MealAllowancePolicyByDesignations { get; set; } = new List<MealAllowancePolicyByDesignation>();

    public virtual ICollection<TravelAllowancePolicyByDesignation> TravelAllowancePolicyByDesignations { get; set; } = new List<TravelAllowancePolicyByDesignation>();
}
