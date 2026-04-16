using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class TenantEncryptionKeys
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public string EncryptionKey { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }
}
