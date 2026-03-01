using System;
using System.Collections.Generic;



public partial class Tenantsubscription
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public int Subscriptionplanid { get; set; }

    public DateTime Subscriptionstartdate { get; set; }

    public DateTime Subscriptionenddate { get; set; }

    public bool Isactive { get; set; }

    public bool Istrial { get; set; }

    public string? Paymenttxnid { get; set; }

    public string? Paymentmode { get; set; }
}
