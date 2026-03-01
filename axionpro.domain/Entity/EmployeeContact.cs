using System;
using System.Collections.Generic;



public partial class Employeecontact
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public int? Contacttype { get; set; }

    public int? Relation { get; set; }

    public string? Contactname { get; set; }

    public string? Contactnumber { get; set; }

    public string? Alternatenumber { get; set; }

    public string? Email { get; set; }

    public bool? Isprimary { get; set; }

    public int? Countryid { get; set; }

    public int? Stateid { get; set; }

    public int? Districtid { get; set; }

    public string? Houseno { get; set; }

    public string? Landmark { get; set; }

    public string? Street { get; set; }

    public string? Address { get; set; }

    public string? Remark { get; set; }

    public bool? Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isinfoverified { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public string? Description { get; set; }
}
