using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeBankDetail
{
    public int Id { get; set; }

    public long EmployeeId { get; set; }

    public string? BankName { get; set; }

    public string? AccountNumber { get; set; }

    public string? IFSCCode { get; set; }

    public string? BranchName { get; set; }

    public string? AccountType { get; set; }

    public string? UPIId { get; set; }

    public bool? IsPrimaryAccount { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public long? SoftDeletedById { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? InfoVerifiedById { get; set; }

    public bool? IsInfoVerified { get; set; }

    public DateTime? InfoVerifiedDateTime { get; set; }

    public bool? IsEditAllowed { get; set; }

    public bool IsActive { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
