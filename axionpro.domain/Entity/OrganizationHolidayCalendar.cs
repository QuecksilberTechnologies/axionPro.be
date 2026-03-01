using System;
using System.Collections.Generic;



public partial class Organizationholidaycalendar
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Countrycode { get; set; }

    public string? Statecode { get; set; }

    public int Holidayyear { get; set; }

    public string? Holidayname { get; set; }

    public DateTime Holidaydate { get; set; }

    public bool Isoptional { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
