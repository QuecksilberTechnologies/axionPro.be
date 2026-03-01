using System;
using System.Collections.Generic;



public partial class Assetassignment
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public long Employeeid { get; set; }

    public long Assetid { get; set; }

    public bool? Isapproved { get; set; }

    public long? Approvalid { get; set; }

    public DateTime? Assigneddate { get; set; }

    public DateTime? Expectedreturndate { get; set; }

    public DateTime? Actualreturndate { get; set; }

    public int Assignmentstatusid { get; set; }

    public string? Assetconditionatassign { get; set; }

    public string? Assetconditionatreturn { get; set; }

    public string? Identificationmethod { get; set; }

    public string? Identificationvalue { get; set; }

    public bool? Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addedbydatetime { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Updatedbydatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
