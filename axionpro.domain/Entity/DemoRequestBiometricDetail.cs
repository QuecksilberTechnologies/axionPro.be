using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DemoRequestBiometricDetail
{
    public string Id { get; set; } = null!;

    public string DemoRequestId { get; set; } = null!;

    public string? BiometricCompanyName { get; set; }

    public string? ModelNumber { get; set; }

    public int? MachineCount { get; set; }

    public string? MachineLocation { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual DemoRequest DemoRequest { get; set; } = null!;
}
