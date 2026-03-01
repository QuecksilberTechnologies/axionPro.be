using System;
using System.Collections.Generic;



public partial class Policytype
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Policyname { get; set; }

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdelete { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Updatedatetime { get; set; }

    public long? Softdeletebyid { get; set; }

    public DateTime? Softdeletedatetime { get; set; }

    public bool? Isstructured { get; set; }
}
