using System;
using System.Collections.Generic;



public partial class Tickettype
{
    public long Id { get; set; }

    public string? Tickettypename { get; set; }

    public long? Ticketheaderid { get; set; }

    public long? Tenantid { get; set; }

    public int? Responsibleroleid { get; set; }

    public string? Description { get; set; }

    public bool Isactive { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Softdeletedtime { get; set; }
}
