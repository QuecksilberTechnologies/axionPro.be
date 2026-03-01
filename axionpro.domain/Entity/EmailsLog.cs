using System;
using System.Collections.Generic;



public partial class Emailslog
{
    public long Id { get; set; }

    public long? Emailqueueid { get; set; }

    public int Templateid { get; set; }

    public string? Toemail { get; set; }

    public string? Ccemail { get; set; }

    public string? Bccemail { get; set; }

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;

    public string? Status { get; set; }

    public string? Errormessage { get; set; }

    public int Retrycount { get; set; }

    public DateTime? Sentdatetime { get; set; }

    public DateTime Createddatetime { get; set; }

    public int Tenantid { get; set; }

    public string? Triggeredby { get; set; }

    public string? Additionalinfojson { get; set; }

    public long? Addedbyid { get; set; }

    public string? Addedfromip { get; set; }
}
