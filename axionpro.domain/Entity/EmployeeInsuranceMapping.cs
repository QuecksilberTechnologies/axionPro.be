using System;
using System.Collections.Generic;



public partial class Employeeinsurancemapping
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Employeeid { get; set; }

    public int Insurancepolicyid { get; set; }

    public DateOnly Coveragestartdate { get; set; }

    public DateOnly? Coverageenddate { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
