using System;
using System.Collections.Generic;



public partial class Tenderservicetype
{
    public int Id { get; set; }

    public int? Parentserviceid { get; set; }

    public string? Servicename { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }
}
