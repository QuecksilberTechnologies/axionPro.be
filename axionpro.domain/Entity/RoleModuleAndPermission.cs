using System;
using System.Collections.Generic;



public partial class Rolemoduleandpermission
{
    public int Id { get; set; }

    public int? Roleid { get; set; }

    public int? Moduleid { get; set; }

    public int? Operationid { get; set; }

    public bool? Hasaccess { get; set; }

    public bool? Isactive { get; set; }

    public string? Remark { get; set; }

    public bool? Isoperational { get; set; }

    public string? Imageicon { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updatedatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool Issoftdeleted { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
