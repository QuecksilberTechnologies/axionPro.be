using System;
using System.Collections.Generic;



public partial class Tenderserviceprovider
{
    public int Id { get; set; }

    public int Tenderservicespecificationid { get; set; }

    public int Serviceproviderid { get; set; }

    public bool? Isinhouse { get; set; }

    public decimal? Contractamount { get; set; }

    public DateOnly? Contractstartdate { get; set; }

    public DateOnly? Contractenddate { get; set; }

    public bool? Isprimaryprovider { get; set; }

    public int Statusid { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }
}
