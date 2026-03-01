using System;
using System.Collections.Generic;



public partial class Employeedependent
{
    public long Id { get; set; }

    public long? Employeeid { get; set; }

    public string? Dependentname { get; set; }

    public int? Relation { get; set; }

    public DateTime? Dateofbirth { get; set; }

    public bool? Iscoveredinpolicy { get; set; }

    public bool? Ismarried { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public bool? Isinfoverified { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isactive { get; set; }

    public string? Filepath { get; set; }

    public string? Filename { get; set; }

    public bool? Hasproofuploaded { get; set; }

    public int? Filetype { get; set; }
}
