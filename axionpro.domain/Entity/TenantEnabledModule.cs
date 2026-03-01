using System;
using System.Collections.Generic;



public partial class Tenantenabledmodule
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public int? Parentmoduleid { get; set; }

    public int Moduleid { get; set; }

    public bool? Isleafnode { get; set; }

    public bool Isenabled { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
