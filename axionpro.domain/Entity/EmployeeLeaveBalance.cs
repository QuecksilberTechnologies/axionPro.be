using System;
using System.Collections.Generic;



public partial class Employeeleavebalance
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public long? Employeeleavepolicymappingid { get; set; }

    public int Leaveyear { get; set; }

    public decimal? Openingbalance { get; set; }

    public decimal? Availed { get; set; }

    public decimal? Currentbalance { get; set; }

    public decimal? Carryforwarded { get; set; }

    public decimal? Encashed { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public decimal? Leavesonhold { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public bool? Isallbalanceonhold { get; set; }

    public long? Updatedbyid { get; set; }
}
