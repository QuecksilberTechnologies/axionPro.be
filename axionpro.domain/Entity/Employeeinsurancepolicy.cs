using System;
using System.Collections.Generic;



public partial class Employeeinsurancepolicy
{
    public int Id { get; set; }

    public long? Employeeid { get; set; }

    public int? Insurancepolicyid { get; set; }

    public DateOnly? Assigneddate { get; set; }

    public string? Description { get; set; }

    public DateOnly? Createddate { get; set; }

    public long? Approvedbyid { get; set; }

    public bool Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
