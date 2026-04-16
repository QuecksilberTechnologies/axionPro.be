using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class DemoRequest
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string WorkEmail { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? IndustryType { get; set; }

    public int? NumberOfEmployees { get; set; }

    public string? CurrentHrms { get; set; }

    public bool? IsBiometricRequired { get; set; }

    public bool? HasExistingBiometric { get; set; }

    public bool? RequiresIntegration { get; set; }

    public int? RequiredMachineCount { get; set; }

    public string? DeploymentPreference { get; set; }

    public string? Hrchallenges { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? AddedDateTime { get; set; }

    public virtual ICollection<DemoRequestBiometricDetail> DemoRequestBiometricDetail { get; set; } = new List<DemoRequestBiometricDetail>();
}
