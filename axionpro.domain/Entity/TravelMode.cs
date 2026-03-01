using System;
using System.Collections.Generic;



public partial class Travelmode
{
    public int Id { get; set; }

    public string? Travelmodename { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public int? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public int? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
