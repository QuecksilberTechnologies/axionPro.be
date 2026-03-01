using System;
using System.Collections.Generic;



public partial class Tenantemailconfig
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public string? Smtphost { get; set; }

    public int? Smtpport { get; set; }

    public string? Smtpusername { get; set; }

    public string? Smtppasswordencrypted { get; set; }

    public string? Fromemail { get; set; }

    public string? Fromname { get; set; }

    public bool Isactive { get; set; }
}
