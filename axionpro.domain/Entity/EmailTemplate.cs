using System;
using System.Collections.Generic;



public partial class Emailtemplate
{
    public int Id { get; set; }

    public string? Templatename { get; set; }

    public string? Templatecode { get; set; }

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;

    public string? Fromemail { get; set; }

    public string? Fromname { get; set; }

    public string? Ccemail { get; set; }

    public string? Bccemail { get; set; }

    public string? Category { get; set; }

    public string? Languagecode { get; set; }

    public bool Isactive { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public string? Addedfromip { get; set; }

    public string? Updatedfromip { get; set; }
}
