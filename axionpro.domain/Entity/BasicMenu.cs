using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class BasicMenu
{
    public int Id { get; set; }

    public string MenuName { get; set; } = null!;

    public string? MenuUrl { get; set; }

    public int? ParentMenuId { get; set; }

    public string Remark { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? ImageIcon { get; set; }

    public virtual ICollection<EmployeeTypeBasicMenu> EmployeeTypeBasicMenus { get; set; } = new List<EmployeeTypeBasicMenu>();

    public virtual ICollection<BasicMenu> InverseParentMenu { get; set; } = new List<BasicMenu>();

    public virtual BasicMenu? ParentMenu { get; set; }
}
