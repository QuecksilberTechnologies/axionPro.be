using System;
using System.Collections.Generic;



public partial class Identitycategory
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
