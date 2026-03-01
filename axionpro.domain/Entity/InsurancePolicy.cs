using System;
using System.Collections.Generic;



public partial class Insurancepolicy
{
    public int Id { get; set; }

    public long Tenantid { get; set; }

    public string? Insurancepolicyname { get; set; }

    public string? Insurancepolicynumber { get; set; }

    public string? Providername { get; set; }

    public DateTime? Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public string? Agentname { get; set; }

    public string? Agentcontactnumber { get; set; }

    public string? Agentofficenumber { get; set; }

    public bool Isactive { get; set; }

    public string? Remark { get; set; }

    public string? Description { get; set; }

    public long? Addedbyid { get; set; }

    public DateTime? Addeddatetime { get; set; }

    public long? Updatedbyid { get; set; }

    public DateTime? Updateddatetime { get; set; }

    public bool Issoftdeleted { get; set; }

    public long? Softdeletedbyid { get; set; }

    public DateTime? Deleteddatetime { get; set; }

    public bool Employeeallowed { get; set; }

    public int Maxspouseallowed { get; set; }

    public int Maxchildallowed { get; set; }

    public int? Countryid { get; set; }

    public bool Parentsallowed { get; set; }

    public bool Inlawsallowed { get; set; }

    public int? Policytypeid { get; set; }
}
