using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Module
{
    public int Id { get; set; }

    public long? TenantId { get; set; }

    public string? ModuleCode { get; set; }

    public string ModuleName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Urlpath { get; set; }

    public int? ParentModuleId { get; set; }

    public bool? IsLeafNode { get; set; }

    public bool IsModuleDisplayInUi { get; set; }

    public bool IsCommonMenu { get; set; }

    public bool IsActive { get; set; }

    public string? ImageIconWeb { get; set; }

    public string? ImageIconMobile { get; set; }

    public int? ItemPriority { get; set; }

    public string? Remark { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<Module> InverseParentModule { get; set; } = new List<Module>();

    public virtual ICollection<ModuleOperationMapping> ModuleOperationMapping { get; set; } = new List<ModuleOperationMapping>();

    public virtual Module? ParentModule { get; set; }

    public virtual ICollection<PlanModuleMapping> PlanModuleMapping { get; set; } = new List<PlanModuleMapping>();

    public virtual ICollection<RoleModuleAndPermission> RoleModuleAndPermission { get; set; } = new List<RoleModuleAndPermission>();

    public virtual ICollection<TenantEnabledModule> TenantEnabledModule { get; set; } = new List<TenantEnabledModule>();

    public virtual ICollection<TenantEnabledOperation> TenantEnabledOperation { get; set; } = new List<TenantEnabledOperation>();
}
