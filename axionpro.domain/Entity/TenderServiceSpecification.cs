using System;
using System.Collections.Generic;



public partial class Tenderservicespecification
{
    public int Id { get; set; }

    public int Tenderserviceid { get; set; }

    public string? Specificationtype { get; set; }

    public string? Specificationname { get; set; }

    public int Quantity { get; set; }

    public string? Productplatform { get; set; }

    public string? Productspecification { get; set; }

    public int? Experiencerequired { get; set; }

    public int? Noticeperiodconsidered { get; set; }

    public decimal? Estimatedbudget { get; set; }

    public bool? Isactive { get; set; }
}
