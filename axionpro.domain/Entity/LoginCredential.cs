using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class LoginCredential
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public long EmployeeId { get; set; }

    public string LoginId { get; set; } = null!;

    public string? Password { get; set; }

    public bool HasFirstLogin { get; set; }

    public bool? IsPasswordChangeRequired { get; set; }

    public string? MacAddress { get; set; }

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? LoginDevice { get; set; }

    public string? IpAddressLocal { get; set; }

    public string? IpAddressPublic { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<ForgotPasswordOTPDetail> ForgotPasswordRequests { get; set; } = new List<ForgotPasswordOTPDetail>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
