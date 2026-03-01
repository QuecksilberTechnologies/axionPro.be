using System;
using System.Collections.Generic;



public partial class Tenantencryptionkey
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public string? Encryptionkey { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
