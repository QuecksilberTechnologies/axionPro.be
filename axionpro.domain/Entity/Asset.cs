using System;
using System.Collections.Generic;



public partial class Asset
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public string? Assetname { get; set; }

    public int Assettypeid { get; set; }

    public string? Company { get; set; }

    public string? Color { get; set; }

    public bool? Isrepairable { get; set; }

    public decimal? Price { get; set; }

    public string? Serialnumber { get; set; }

    public string? Barcode { get; set; }

    public string? Qrcode { get; set; }

    public DateTime? Purchasedate { get; set; }

    public DateTime? Warrantyexpirydate { get; set; }

    public string? Size { get; set; }

    public string? Modelno { get; set; }

    public string? Weight { get; set; }

    public int Assetstatusid { get; set; }

    public bool? Isassigned { get; set; }

    public bool? Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public long Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
