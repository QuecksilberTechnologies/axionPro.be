using System;
using System.Collections.Generic;



public partial class Module1
{
    public int Id { get; set; }

    public long? Tenantid { get; set; }

    public string? Modulecode { get; set; }

    public string? Modulename { get; set; }

    public string? Displayname { get; set; }

    public string? Urlpath { get; set; }

    public int? Parentmoduleid { get; set; }

    public bool? Isleafnode { get; set; }

    public bool Ismoduledisplayinui { get; set; }

    public bool Iscommonmenu { get; set; }

    public bool Isactive { get; set; }

    public string? Imageiconweb { get; set; }

    public string? Imageiconmobile { get; set; }

    public int? Itempriority { get; set; }

    public string? Remark { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
