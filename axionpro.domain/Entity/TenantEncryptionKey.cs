using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenantEncryptionKey
{
    public long Id { get; set; }

    public long ?TenantId { get; set; }

    public string? EncryptionKey { get; set; } = string.Empty;

    public bool? IsActive { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }
}
