using System;
using System.Collections.Generic;



public partial class Holidaymaster
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public int Year { get; set; }

    public DateOnly Holidaydate { get; set; }

    public string? Holidayname { get; set; }

    public string? Region { get; set; }

    public bool Isregionalholiday { get; set; }

    public bool Isweekend { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }
}
