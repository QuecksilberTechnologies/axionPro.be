using System;
using System.Collections.Generic;



public partial class Employeeworkprofile
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public bool Isfresher { get; set; }

    public string? Comment { get; set; }

    public bool Isactive { get; set; }

    public bool Iseditallowed { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
