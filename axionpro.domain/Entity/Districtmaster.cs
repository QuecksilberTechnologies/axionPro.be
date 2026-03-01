using System;
using System.Collections.Generic;



public partial class Districtmaster
{
    public int Id { get; set; }

    public int Stateid { get; set; }

    public string? Districtcode { get; set; }

    public string? Pincode { get; set; }

    public string? Districtname { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
