using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeImage
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public string? EmployeeImagePath { get; set; }

    public long? EmployeeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsPrimary { get; set; }
    public bool HasImageUploaded { get; set; }
    

    public bool IsSoftDeleted { get; set; }

    public long? SoftDeletedById { get; set; }
    public long? UpdateById { get; set; }
    public long? AddedById { get; set; }
 
    public DateTime? AddedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public DateTime? DeletedDateTime { get; set; }
   
    public int? ImageType { get; set; }

    public virtual Employee? Employee { get; set; }


}
