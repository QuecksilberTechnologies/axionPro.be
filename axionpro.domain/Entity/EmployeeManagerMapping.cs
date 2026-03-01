using System;
using System.Collections.Generic;



public partial class Employeemanagermapping
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public long Employeeid { get; set; }

    public long Managerid { get; set; }

    public int? Departmentid { get; set; }

    public int? Designationid { get; set; }

    public int Reportingtypeid { get; set; }

    public DateTime Effectivefrom { get; set; }

    public DateTime? Effectiveto { get; set; }

    public bool Isactive { get; set; }

    public string? Description { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }
}
