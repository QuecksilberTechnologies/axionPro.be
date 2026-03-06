using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class City
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public int StateId { get; set; }

    public bool? IsActive { get; set; }

    public virtual State State { get; set; } = null!;
}
