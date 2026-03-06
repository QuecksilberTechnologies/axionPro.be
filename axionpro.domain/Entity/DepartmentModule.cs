using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DepartmentModule
{
    public int Id { get; set; }

    public int? DepartmentId { get; set; }

    public string? ModuleName { get; set; }

    public string? Remark { get; set; }

    public string? SubModuleName { get; set; }

    public string? Technology { get; set; }

    public string? TechnologyRemark { get; set; }

    public int ForPlatform { get; set; }

    public string? ForPlatformRemark { get; set; }

    public bool? IsActive { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Department? Department { get; set; }
}
