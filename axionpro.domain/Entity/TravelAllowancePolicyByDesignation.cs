using System;
using System.Collections.Generic;



public partial class Travelallowancepolicybydesignation
{
    public int Id { get; set; }

    public int Policytypeid { get; set; }

    public int Employeetypeid { get; set; }

    public int Designationid { get; set; }

    public int? Travelmodeid { get; set; }

    public string? Travelclass { get; set; }

    public int? Mintraveldistance { get; set; }

    public int? Maxtraveldistance { get; set; }

    public decimal? Reimbursementperkm { get; set; }

    public bool? Ismetro { get; set; }

    public decimal? Metrobonus { get; set; }

    public string? Requireddocuments { get; set; }

    public bool? Advanceallowed { get; set; }

    public decimal? Maxadvanceamount { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdelete { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public int Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }
}
