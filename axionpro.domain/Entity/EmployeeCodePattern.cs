using System;
using System.Collections.Generic;



public partial class Employeecodepattern
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public string? Prefix { get; set; }

    public bool Includeyear { get; set; }

    public bool Includemonth { get; set; }

    public bool Includedepartment { get; set; }

    public string? Separator { get; set; }

    public int Runningnumberlength { get; set; }

    public int Lastusednumber { get; set; }

    public bool Isactive { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
