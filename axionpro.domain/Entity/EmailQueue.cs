using System;
using System.Collections.Generic;



public partial class Emailqueue
{
    public int Id { get; set; }

    public int Templateid { get; set; }

    public string? Toemail { get; set; }

    public string? Ccemail { get; set; }

    public string? Bccemail { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public bool? Issent { get; set; }

    public DateTime? Senddatetime { get; set; }

    public string? Errormessage { get; set; }

    public int? Retrycount { get; set; }

    public DateTime? Addeddatetime { get; set; }
}
