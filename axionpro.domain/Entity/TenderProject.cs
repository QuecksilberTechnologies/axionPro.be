using System;
using System.Collections.Generic;



public partial class Tenderproject
{
    public int Id { get; set; }

    public int Tenderserviceproviderid { get; set; }

    public string? Projectname { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public long Userroleid { get; set; }

    public int Statusid { get; set; }

    public DateTime? Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public decimal? Estimatedbudget { get; set; }

    public bool? Isactive { get; set; }

    public int? Expectedteamsize { get; set; }
}
