using System;
using System.Collections.Generic;



public partial class Employeeidentity
{
    public long Id { get; set; }

    public long Employeeid { get; set; }

    public int Identitycategorydocumentid { get; set; }

    public string? Identityvalue { get; set; }

    public string? Documentfilename { get; set; }

    public string? Documentfilepath { get; set; }

    public bool Isinfoverified { get; set; }

    public long? Infoverifiedbyid { get; set; }

    public DateTime? Infoverifieddatetime { get; set; }

    public bool Iseditallowed { get; set; }

    public bool Hasidentityuploaded { get; set; }

    public DateOnly Effectivefrom { get; set; }

    public DateOnly? Effectiveto { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
