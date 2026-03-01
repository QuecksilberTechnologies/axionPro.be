using System;
using System.Collections.Generic;



public partial class Assethistory
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public int Assetid { get; set; }

    public long? Employeeid { get; set; }

    public DateTime? Assigneddate { get; set; }

    public DateTime? Returneddate { get; set; }

    public int Assignmentstatusid { get; set; }

    public string? Assetconditionatassign { get; set; }

    public string? Assetconditionatreturn { get; set; }

    public string? Identificationmethod { get; set; }

    public string? Identificationvalue { get; set; }

    public bool? Isscrapped { get; set; }

    public string? Scrapreason { get; set; }

    public long? Scrapapprovedby { get; set; }

    public DateTime? Scrapdate { get; set; }

    public string? Remarks { get; set; }

    public bool Issoftdeleted { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
