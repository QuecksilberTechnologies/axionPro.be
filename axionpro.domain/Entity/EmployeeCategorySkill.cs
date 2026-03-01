using System;
using System.Collections.Generic;



public partial class Employeecategoryskill
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public int Categoryid { get; set; }

    public string? Description { get; set; }

    public bool Isactive { get; set; }
}
