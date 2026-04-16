using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; } = null!;

    public string? CountryCode { get; set; }

    public bool? IsActive { get; set; }

    public string Stdcode { get; set; } = null!;

    public virtual ICollection<CountryIdentityRule> CountryIdentityRule { get; set; } = new List<CountryIdentityRule>();

    public virtual ICollection<CountryStatutoryRule> CountryStatutoryRule { get; set; } = new List<CountryStatutoryRule>();

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();

    public virtual ICollection<InsurancePolicy> InsurancePolicy { get; set; } = new List<InsurancePolicy>();

    public virtual ICollection<State> State { get; set; } = new List<State>();

    public virtual ICollection<StatutoryType> StatutoryType { get; set; } = new List<StatutoryType>();
}
