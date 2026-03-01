using System;
using System.Collections.Generic;



public partial class Assettickettypedetail
{
    public long Id { get; set; }

    public long Tickettypeid { get; set; }

    public int Assettypeid { get; set; }

    public int Responsibleroleid { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeletedtime { get; set; }
}
