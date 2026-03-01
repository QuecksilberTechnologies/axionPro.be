using System;
using System.Collections.Generic;



public partial class Tenant
{
    public long Id { get; set; }

    public int Tenantindustryid { get; set; }

    public string? Companyname { get; set; }

    public string? Tenantcode { get; set; }

    public string? Companyemaildomain { get; set; }

    public string? Tenantemail { get; set; }

    public string? Contactpersonname { get; set; }

    public int? Genderid { get; set; }

    public string? Contactnumber { get; set; }

    public int Countryid { get; set; }

    public bool Isverified { get; set; }

    public bool Isactive { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
