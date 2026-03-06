using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class IdentityCategoryDocument
{
    public int Id { get; set; }

    public int IdentityCategoryId { get; set; }

    public string Code { get; set; } = null!;

    public string DocumentName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsUnique { get; set; }

    public bool IsActive { get; set; }

    public long? AddedById { get; set; }

    public long? UpdatedById { get; set; }

    public DateTime AddedDateTime { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public virtual ICollection<CountryIdentityRule> CountryIdentityRules { get; set; } = new List<CountryIdentityRule>();

    public virtual ICollection<EmployeeIdentity> EmployeeIdentities { get; set; } = new List<EmployeeIdentity>();

    public virtual IdentityCategory IdentityCategory { get; set; } = null!;
}
