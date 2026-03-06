using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmailQueue
{
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public string ToEmail { get; set; } = null!;

    public string? CcEmail { get; set; }

    public string? BccEmail { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public bool? IsSent { get; set; }

    public DateTime? SendDateTime { get; set; }

    public string? ErrorMessage { get; set; }

    public int? RetryCount { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual EmailTemplate Template { get; set; } = null!;
}
