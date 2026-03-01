using System;
using System.Collections.Generic;



public partial class Employeeleavepolicymapping
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public bool? Isleavebalanceassigned { get; set; }

    public long Employeeid { get; set; }

    public long Policyleavetypemappingid { get; set; }

    public DateTime Effectivefrom { get; set; }

    public DateTime? Effectiveto { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
