using System;
using System.Collections.Generic;



public partial class Insurancepolicydocument
{
    public long Id { get; set; }

    public long Tenantid { get; set; }

    public int Insurancepolicyid { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public int Filetype { get; set; }

    public string? Languagecode { get; set; }

    public string? Documenttype { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeleteddatetime { get; set; }
}
