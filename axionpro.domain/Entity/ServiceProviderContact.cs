using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class ServiceProviderContact
{
    public int Id { get; set; }

    public int ServiceProviderId { get; set; }

    public string Name { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string OfficialMobile { get; set; } = null!;

    public string? PersonalMobile { get; set; }

    public string Email { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}
