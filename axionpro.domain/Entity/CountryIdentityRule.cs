using System;
using System.Collections.Generic;



public partial class Countryidentityrule
{
    public int Id { get; set; }

    public int Countryid { get; set; }

    public int Identitycategorydocumentid { get; set; }

    public bool Ismandatory { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }
}
