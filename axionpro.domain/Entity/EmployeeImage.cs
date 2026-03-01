using System;
using System.Collections.Generic;



public partial class Employeeimage
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Filepath { get; set; }

    public long? Employeeid { get; set; }

    public bool? Isactive { get; set; }

    public bool Isprimary { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public int? Filetype { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Hasimageuploaded { get; set; }

    public string? Filename { get; set; }
}
