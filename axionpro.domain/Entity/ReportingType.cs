using System;
using System.Collections.Generic;



public partial class Reportingtype
{
    public int Id { get; set; }

    public string? Typename { get; set; }

    public string? Description { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
