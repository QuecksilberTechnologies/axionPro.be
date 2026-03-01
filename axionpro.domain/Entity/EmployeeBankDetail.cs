using System;
using System.Collections.Generic;



public partial class Employeebankdetail
{
    public int Id { get; set; }

    public long Employeeid { get; set; }

    public string? Bankname { get; set; }

    public string? Accountnumber { get; set; }

    public string? Ifsccode { get; set; }

    public string? Branchname { get; set; }

    public string? Accounttype { get; set; }

    public string? Upiid { get; set; }

    public bool? Isprimaryaccount { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool? Issoftdeleted { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public bool? Isinfoverified { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool? Iseditallowed { get; set; }

    public bool? Isactive { get; set; }

    public bool? Haschequedocuploaded { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public int? Filetype { get; set; }
}
