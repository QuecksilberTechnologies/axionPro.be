using System;
using System.Collections.Generic;



public partial class Tenantenabledoperation
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public int Moduleid { get; set; }

    public int Operationid { get; set; }

    public bool? Isoperationused { get; set; }

    public bool Isenabled { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
