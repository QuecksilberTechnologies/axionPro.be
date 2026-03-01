using System;
using System.Collections.Generic;



public partial class Forgotpasswordotpdetail
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Userid { get; set; }

    public string? Otp { get; set; }

    public DateTime Otpexpiredatetime { get; set; }

    public bool Isused { get; set; }

    public DateTime Createddatetime { get; set; }

    public DateTime? Useddatetime { get; set; }

    public long Employeeid { get; set; }

    public bool? Isvalidate { get; set; }
}
