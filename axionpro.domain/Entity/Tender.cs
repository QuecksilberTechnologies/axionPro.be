using System;
using System.Collections.Generic;



public partial class Tender
{
    public int Id { get; set; }

    public int Clientid { get; set; }

    public int Tenderstatusid { get; set; }

    public string? Tendername { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public decimal? Tendervalue { get; set; }

    public DateOnly? Enddate { get; set; }

    public DateOnly Startdate { get; set; }

    public bool Isactive { get; set; }
}
