using System;
using System.Collections.Generic;



public partial class Employeeexperience
{
    public long Id { get; set; }

    public long? Employeeid { get; set; }

    public decimal? Ctc { get; set; }

    public string? Comment { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isactive { get; set; }

    public bool? Hasepfaccount { get; set; }

    public bool? Isfresher { get; set; }

    public bool? Isforeignexperience { get; set; }
}
