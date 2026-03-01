using System;
using System.Collections.Generic;

 

public partial class Accoumndationallowancepolicybydesignation
{
    public int Id { get; set; }

    public int Policytypeid { get; set; }

    public int Employeetypeid { get; set; }

    public int Designationid { get; set; }

    public int? Mindaysrequired { get; set; }

    public decimal? Fixedstayallowance { get; set; }

    public bool? Ismetro { get; set; }

    public decimal? Metrobonus { get; set; }

    public string? Requireddocuments { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdelete { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public int Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public int? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
