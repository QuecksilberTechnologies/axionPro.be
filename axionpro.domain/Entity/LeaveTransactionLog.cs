using System;
using System.Collections.Generic;



public partial class Leavetransactionlog
{
    public long Id { get; set; }

    public long? Employeeleavebalanceid { get; set; }

    public string? Transactiontype { get; set; }

    public decimal? Leavedays { get; set; }

    public DateTime? Transactiondate { get; set; }

    public long? Performedby { get; set; }

    public string? Remarks { get; set; }
}
