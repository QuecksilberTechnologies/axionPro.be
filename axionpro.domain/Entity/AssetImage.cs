using System;
using System.Collections.Generic;



public partial class Assetimage
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Assetid { get; set; }

    public int Assetimagetype { get; set; }

    public string? Assetimagepath { get; set; }

    public string? Remark { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool Isprimary { get; set; }
}
