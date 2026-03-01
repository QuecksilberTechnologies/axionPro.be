using System;
using System.Collections.Generic;



public partial class Employeetypebasicmenu
{
    public int Id { get; set; }

    public int Basicmenuid { get; set; }

    public int Employeetypeid { get; set; }

    public int? Forplatform { get; set; }

    public bool Ismenudisplayinui { get; set; }

    public bool Isdisplayable { get; set; }

    public bool Isactive { get; set; }

    public bool Hasaccess { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
