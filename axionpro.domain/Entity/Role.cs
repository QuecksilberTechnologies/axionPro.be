using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Role
{
    public int Id { get; set; }

    public long? TenantId { get; set; }  

    public string? RoleName { get; set; }

    public int RoleType { get; set; }

    public string? Remark { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSystemDefault { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<AssetTicketTypeDetail> AssetTicketTypeDetails { get; set; } = new List<AssetTicketTypeDetail>();

  
    public virtual ICollection<RoleModuleAndPermission> RoleModuleAndPermissions { get; set; } = new List<RoleModuleAndPermission>();

    public virtual Tenant? Tenant { get; set; }

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
