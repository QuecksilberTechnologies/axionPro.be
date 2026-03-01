using System;
using System.Collections.Generic;



public partial class Assetstatus
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public string? Statusname { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public string? Colorkey { get; set; }
}
