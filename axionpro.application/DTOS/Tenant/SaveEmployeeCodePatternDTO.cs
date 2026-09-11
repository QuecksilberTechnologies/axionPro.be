using System.Text.Json.Serialization;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.Tenant;

namespace axionpro.application.DTOS.Tenant;

/// <summary>Previews or confirms a tenant-owned pattern and employee-code change.</summary>
public sealed class SaveEmployeeCodePatternRequestDTO : PermissionRequestDTO
{
    public NewTenantEmployeeCodePatternRequestDTO Pattern { get; set; } = new();
    public bool Confirm { get; set; }
    public string? PreviewHash { get; set; }
}

/// <summary>Lists the exact code changes requiring approval before any writes.</summary>
public sealed class SaveEmployeeCodePatternResponseDTO
{
    public string PreviewHash { get; set; } = string.Empty;
    public bool Applied { get; set; }
    public bool CanCommit => Errors.Count == 0;
    public int LastUsedNumber { get; set; }
    public List<EmployeeCodeChangeDTO> Employees { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

/// <summary>Shows an existing employee's current and proposed code.</summary>
public sealed class EmployeeCodeChangeDTO
{
    [JsonIgnore]
    public long EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CurrentCode { get; set; }
    public string? ProposedCode { get; set; }
    public DateTime? JoiningDate { get; set; }
    public List<string> Errors { get; set; } = new();
}
