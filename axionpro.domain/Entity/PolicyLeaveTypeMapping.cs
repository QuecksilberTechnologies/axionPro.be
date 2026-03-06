using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PolicyLeaveTypeMapping
{
    public long Id { get; set; }

    public int PolicyTypeId { get; set; }

    public int LeaveTypeId { get; set; }

    public long TenantId { get; set; }

    public int EmployeeTypeId { get; set; }

    public int? ApplicableGenderId { get; set; }

    public bool? IsMarriedApplicable { get; set; }

    public bool? IsEmployeeMapped { get; set; }

    public int TotalLeavesPerYear { get; set; }

    public bool MonthlyAccrual { get; set; }

    public bool CarryForward { get; set; }

    public int? MaxCarryForward { get; set; }

    public int? CarryForwardExpiryMonths { get; set; }

    public bool Encashable { get; set; }

    public int? MaxEncashable { get; set; }

    public bool IsProofRequired { get; set; }

    public string? ProofDocumentType { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Remark { get; set; }

    public long AddedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public DateTime? SoftDeleteDateTime { get; set; }

    public long? SoftDeleteById { get; set; }

    public virtual Gender? ApplicableGender { get; set; }

    public virtual ICollection<EmployeeLeavePolicyMapping> EmployeeLeavePolicyMappings { get; set; } = new List<EmployeeLeavePolicyMapping>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRule> LeaveRules { get; set; } = new List<LeaveRule>();
}
