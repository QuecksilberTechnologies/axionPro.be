using System;
using System.Collections.Generic;



public partial class Workflowstep
{
    public long Id { get; set; }

    public long Approvalworkflowid { get; set; }

    public int Approvallevel { get; set; }

    public int Approvertype { get; set; }

    public long Approverreferenceid { get; set; }

    public bool? Ismandatory { get; set; }

    public int? Escalateafterdays { get; set; }

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
