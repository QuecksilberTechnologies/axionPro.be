using System;
using System.Collections.Generic;



public partial class Designation
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public int? Departmentid { get; set; }

    public string? Designationname { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }
}
