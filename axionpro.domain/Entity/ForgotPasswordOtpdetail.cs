using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ForgotPasswordOtpdetail
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long UserId { get; set; }

    public string Otp { get; set; } = null!;

    public DateTime OtpexpireDateTime { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? UsedDateTime { get; set; }

    public long EmployeeId { get; set; }

    public bool? IsValidate { get; set; }
}
