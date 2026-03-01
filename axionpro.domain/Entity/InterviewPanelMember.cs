using System;
using System.Collections.Generic;



public partial class Interviewpanelmember
{
    public int Id { get; set; }

    public int Panelid { get; set; }

    public long Userroleid { get; set; }

    public bool Isactive { get; set; }

    public bool Isapproved { get; set; }

    public string? Description { get; set; }

    public string? Remarks { get; set; }
}
