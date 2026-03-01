using System;
using System.Collections.Generic;



public partial class Policyleavetypemapping
{
    public long Id { get; set; }

    public int Policytypeid { get; set; }

    public int Leavetypeid { get; set; }

    public long? Tenantid { get; set; }

    public int? Employeetypeid { get; set; }

    public int? Applicablegenderid { get; set; }

    public bool? Ismarriedapplicable { get; set; }

    public bool? Isemployeemapped { get; set; }

    public int Totalleavesperyear { get; set; }

    public bool Monthlyaccrual { get; set; }

    public bool Carryforward { get; set; }

    public int? Maxcarryforward { get; set; }

    public int? Carryforwardexpirymonths { get; set; }

    public bool Encashable { get; set; }

    public int? Maxencashable { get; set; }

    public bool Isproofrequired { get; set; }

    public string? Proofdocumenttype { get; set; }

    public DateTime Effectivefrom { get; set; }

    public DateTime? Effectiveto { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public long? Softdeletebyid { get; set; }
}
