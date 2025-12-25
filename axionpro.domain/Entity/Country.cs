using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; } = null!;

    public string? CountryCode { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CountryIdentityRule> CountryIdentityRules { get; set; } = new List<CountryIdentityRule>();

 
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<State> States { get; set; } = new List<State>();

   
}
