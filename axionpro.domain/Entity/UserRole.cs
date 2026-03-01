using System;
using System.Collections.Generic;



public partial class Userrole
{
    public long Id { get; set; }

    public long? Employeeid { get; set; }

    public int? Roleid { get; set; }

    public bool? Isactive { get; set; }

    public bool? Isprimaryrole { get; set; }

    public string? Remark { get; set; }

    public DateTime? Assigneddatetime { get; set; }

    public DateTime? Removeddatetime { get; set; }

    public long? Assignedbyid { get; set; }

    public DateTime? Rolestartdate { get; set; }

    public bool? Approvalrequired { get; set; }

    public string? Approvalstatus { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
