using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class PayrollEmployee
{
    public long Id { get; set; }

    public long PayrollRunId { get; set; }

    public long EmployeeId { get; set; }

    public decimal? PaidDays { get; set; }

    public decimal? LOPDays { get; set; }

    public decimal? GrossSalary { get; set; }

    public decimal? TotalEarnings { get; set; }

    public decimal? TotalDeductions { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? NetSalary { get; set; }

    public string? CurrencyCode { get; set; }

    public bool IsActive { get; set; }

    public bool? IsSoftDeleted { get; set; }

    public long? AddedById { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<PayrollEmployeeDetail> PayrollEmployeeDetail { get; set; } = new List<PayrollEmployeeDetail>();

    public virtual PayrollRun PayrollRun { get; set; } = null!;
}
