using System;
using System.Collections.Generic;



public partial class Employeestatutoryaccount
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public int Statutorytypeid { get; set; }

    public string? Accountnumber { get; set; }

    public string? Employercode { get; set; }

    public DateOnly Contributionstartdate { get; set; }

    public DateOnly? Contributionenddate { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public long? Deletedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
