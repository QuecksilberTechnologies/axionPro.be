using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class HolidayMaster
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public int Year { get; set; }

    public DateOnly HolidayDate { get; set; }

    public string HolidayName { get; set; } = null!;

    public string? Region { get; set; }

    public bool IsRegionalHoliday { get; set; }

    public bool IsWeekend { get; set; }

    public bool IsActive { get; set; }

    public bool IsSoftDeleted { get; set; }

    public string? Remark { get; set; }

    public long? AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }
}
