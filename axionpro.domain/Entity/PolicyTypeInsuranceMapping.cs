using System;
using System.Collections.Generic;



public partial class Policytypeinsurancemapping
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public int Policytypeid { get; set; }

    public int Insurancepolicyid { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool Issoftdeleted { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public long? Softdeletebyid { get; set; }
}
