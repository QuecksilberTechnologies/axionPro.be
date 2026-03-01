using System;
using System.Collections.Generic;



public partial class Leaverequest
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Employeeid { get; set; }

    public int Leavetypeid { get; set; }

    public DateOnly Fromdate { get; set; }

    public DateOnly Todate { get; set; }

    public bool? Ishalfday { get; set; }

    public DateOnly? Halfdaydate { get; set; }

    public bool? Isfirsthalf { get; set; }

    public decimal? Totalleavedays { get; set; }

    public string? Reason { get; set; }

    public int Status { get; set; }

    public long? Approvedbyid { get; set; }

    public DateTime? Approveddate { get; set; }

    public long Leavepolicyid { get; set; }

    public bool? Issandwich { get; set; }

    public long Createdbyid { get; set; }

    public DateTime? Createddatetime { get; set; }

    public DateTime? Cancellationdate { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public string? Remark { get; set; }

    public bool? Isdocumentattached { get; set; }
}
