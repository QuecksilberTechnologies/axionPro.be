using System;
using System.Collections.Generic;



public partial class Employeeworkdocument
{
    public long Id { get; set; }

    public long Employeeworkhistoryid { get; set; }

    public int Workdocumenttypeid { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public bool Isverified { get; set; }

    public long? Verifiedbyid { get; set; }

    public DateTime? Verifieddatetime { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }
}
