using System;
using System.Collections.Generic;



public partial class Tenderservice
{
    public int Id { get; set; }

    public int Tenderid { get; set; }

    public int Tenderservicetypeid { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool? Isactive { get; set; }
}
