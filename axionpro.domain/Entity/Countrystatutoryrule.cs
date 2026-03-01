using System;
using System.Collections.Generic;



public partial class Countrystatutoryrule
{
    public int Id { get; set; }

    public int Countryid { get; set; }

    public int Statutorytypeid { get; set; }

    public bool Ismandatory { get; set; }

    public decimal? Salarythreshold { get; set; }

    public bool Isactive { get; set; }

    public long? Addedbyid { get; set; }

    public long? Updatedbyid { get; set; }

    public long? Deletedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public DateTime? Deleteddatetime { get; set; }
}
