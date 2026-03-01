using System;
using System.Collections.Generic;



public partial class Employeeexperiencepayslipupload
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public long Experiencedetailid { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public bool Hasuploadedpayslip { get; set; }

    public string? Payslipdocname { get; set; }

    public string? Payslipdocpath { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public long Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
