using System;
using System.Collections.Generic;



public partial class Employeeschangedtypehistory
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public int Oldemployeetypeid { get; set; }

    public int Newemployeetypeid { get; set; }

    public DateTime? Changedatetime { get; set; }

    public long Changedbyid { get; set; }

    public string? Remark { get; set; }

    public bool Isactive { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
