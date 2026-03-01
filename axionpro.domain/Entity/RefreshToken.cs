using System;
using System.Collections.Generic;



public partial class Refreshtoken
{
    public long Id { get; set; }

    public string? Loginid { get; set; }

    public string? Token { get; set; }

    public DateTime Expirydate { get; set; }

    public bool? Isrevoked { get; set; }

    public DateTime? Createdat { get; set; }

    public string? Createdbyip { get; set; }

    public DateTime? Revokedat { get; set; }

    public string? Revokedbyip { get; set; }

    public string? Replacedbytoken { get; set; }
}
