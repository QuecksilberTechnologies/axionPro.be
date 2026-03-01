using System;
using System.Collections.Generic;



public partial class Approvalworkflow
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Projectchildmoduledetailid { get; set; }

    public string? Actionname { get; set; }

    public string? Workflowname { get; set; }

    public bool? Issoftdeleted { get; set; }

    public bool? Isactive { get; set; }

    public bool? Isdeleted { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
