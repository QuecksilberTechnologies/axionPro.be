using System;
using System.Collections.Generic;



public partial class Identitycategorydocument
{
    public int Id { get; set; }

    public int Identitycategoryid { get; set; }

    public string? Code { get; set; }

    public string? Documentname { get; set; }

    public string? Description { get; set; }

    public bool Isunique { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
