using System;
using System.Collections.Generic;



public partial class Tenantindustry
{
    public int Id { get; set; }

    public string? Industryname { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }
}
