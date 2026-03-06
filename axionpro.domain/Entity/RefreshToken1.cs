using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class RefreshToken1
{
    public int Id { get; set; }

    public string LoginId { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public virtual LoginCredential Login { get; set; } = null!;
}
