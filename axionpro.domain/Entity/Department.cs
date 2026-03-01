using System;
using System.Collections.Generic;



public partial class Department
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Departmentname { get; set; }

    public string? Description { get; set; }

    public bool? Isexecutiveoffice { get; set; }

    public bool? Isactive { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
