using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class OrganizationHolidayCalendar
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? StateCode { get; set; }

    public int HolidayYear { get; set; }

    public string HolidayName { get; set; } = null!;

    public DateTime HolidayDate { get; set; }

    public bool IsOptional { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual Tenant? Tenant { get; set; }
}
