using System;
using System.Collections.Generic;



public partial class Companypolicydocument
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public int Policytypeid { get; set; }

    public string? Documenttitle { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }
}
