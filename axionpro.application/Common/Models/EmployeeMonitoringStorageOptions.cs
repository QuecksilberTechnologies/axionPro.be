namespace axionpro.application.Common.Models;

/// <summary>Configures the server-side screenshot object-storage adapter.</summary>
public sealed class EmployeeMonitoringStorageOptions
{
    public const string SectionName = "EmployeeMonitoringStorage";
    public string Provider { get; set; } = string.Empty;
    public string RootPath { get; set; } = string.Empty;
    public long MaximumUploadBytes { get; set; }
    public string[] AllowedContentTypes { get; set; } = [];
}
