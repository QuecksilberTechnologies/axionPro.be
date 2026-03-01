using System;
using System.Collections.Generic;



public partial class Logincredential
{
    public long Id { get; set; }

    public long? Tenantid { get; set; }

    public long Employeeid { get; set; }

    public string? Loginid { get; set; }

    public string? Password { get; set; }

    public bool Hasfirstlogin { get; set; }

    public bool? Ispasswordchangerequired { get; set; }

    public string? Macaddress { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? Logindevice { get; set; }

    public string? Ipaddresslocal { get; set; }

    public string? Ipaddresspublic { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
