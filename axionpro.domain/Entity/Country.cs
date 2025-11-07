using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; } = null!;

    public string? CountryCode { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
