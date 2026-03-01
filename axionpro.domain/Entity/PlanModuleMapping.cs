using System;
using System.Collections.Generic;



public partial class Planmodulemapping
{
    public int Id { get; set; }

    public int Subscriptionplanid { get; set; }

    public int Moduleid { get; set; }

    public bool? Isactive { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
