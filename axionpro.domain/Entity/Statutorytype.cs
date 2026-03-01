using System;
using System.Collections.Generic;



public partial class Statutorytype
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int Countryid { get; set; }

    public bool Isemployeecontributionrequired { get; set; }

    public bool Isemployercontributionrequired { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public long? Deletedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
