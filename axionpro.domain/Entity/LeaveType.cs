using System;
using System.Collections.Generic;



public partial class Leavetype
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Leavename { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Updatedatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Softdeletedby { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }
}
