using System;
using System.Collections.Generic;



public partial class Subscriptionplan
{
    public int Id { get; set; }

    public string? Planname { get; set; }

    public int Maxusers { get; set; }

    public decimal? Perdayprice { get; set; }

    public decimal? Monthlyprice { get; set; }

    public decimal? Yearlyprice { get; set; }

    public bool? Isfree { get; set; }

    public bool Isactive { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public string? Currencykey { get; set; }

    public bool? Ismostpopular { get; set; }

    public bool? Iscustom { get; set; }
}
