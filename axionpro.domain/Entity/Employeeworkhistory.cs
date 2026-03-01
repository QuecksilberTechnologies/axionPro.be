using System;
using System.Collections.Generic;



public partial class Employeeworkhistory
{
    public long Id { get; set; }

    public long Employeeworkprofileid { get; set; }

    public string? Companyname { get; set; }

    public string? Designation { get; set; }

    public decimal? Ctc { get; set; }

    public DateOnly Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public string? Reasonforleaving { get; set; }

    public long? Workingcountryid { get; set; }

    public long? Workingstateid { get; set; }

    public long? Workingdistrictid { get; set; }

    public bool Iswfh { get; set; }

    public bool Isforeignexperience { get; set; }

    public string? Reportingmanagername { get; set; }

    public string? Reportingmanagernumber { get; set; }

    public string? Verificationemail { get; set; }

    public bool Isverified { get; set; }

    public long? Verifiedbyid { get; set; }

    public DateTime? Verifieddatetime { get; set; }

    public string? Verificationmode { get; set; }

    public bool Isactive { get; set; }

    public bool Iseditallowed { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
