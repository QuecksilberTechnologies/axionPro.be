using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmailsLog
{
    public long Id { get; set; }

    public long? EmailQueueId { get; set; }

    public int TemplateId { get; set; }

    public string ToEmail { get; set; } = null!;

    public string? CcEmail { get; set; }

    public string? BccEmail { get; set; }

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public int RetryCount { get; set; }

    public DateTime? SentDateTime { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public int TenantId { get; set; }

    public string? TriggeredBy { get; set; }

    public string? AdditionalInfoJson { get; set; }

    public long? AddedById { get; set; }

    public string? AddedFromIp { get; set; }
}
