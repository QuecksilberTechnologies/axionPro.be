using System;
using System.Collections.Generic;



public partial class Leaverule
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Policyleavetypeid { get; set; }

    public bool Applysandwichrule { get; set; }

    public bool? Islinkedsandwichrule { get; set; }

    public bool Ishalfdayallowed { get; set; }

    public int? Halfdaynoticehours { get; set; }

    public int? Noticeperioddays { get; set; }

    public int? Maxcontinuousleaves { get; set; }

    public int? Mingapbetweenleaves { get; set; }

    public bool Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public string? Remark { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public long? Softdeletebyid { get; set; }
}
