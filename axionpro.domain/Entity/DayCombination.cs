using System;
using System.Collections.Generic;



public partial class Daycombination
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Combinationname { get; set; }

    public int Startday { get; set; }

    public int Endday { get; set; }

    public string? Remark { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }
}
