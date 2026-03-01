using System;
using System.Collections.Generic;



public partial class Moduleoperationmapping
{
    public int Id { get; set; }

    public int? Moduleid { get; set; }

    public int Operationid { get; set; }

    public int? Dataviewstructureid { get; set; }

    public int? Pagetypeid { get; set; }

    public string? Pageurl { get; set; }

    public string? Iconurl { get; set; }

    public bool? Iscommonitem { get; set; }

    public bool? Isoperational { get; set; }

    public int? Priority { get; set; }

    public string? Remark { get; set; }

    public bool? Isactive { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
