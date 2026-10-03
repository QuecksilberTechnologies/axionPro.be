// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents tenant-scoped employee screenshot monitoring state.
// ================================================================

namespace axionpro.domain.Entity;

public enum EmployeeMonitoringConnectionStatus
{
    NeverConnected = 1,
    Online = 2,
    UploadFailure = 3,
    Delayed = 4,
    Unreachable = 5,
    Inactive = 6
}

/// <summary>Defines one Tenant's configurable employee monitoring policy.</summary>
public sealed class EmployeeMonitoringPolicy
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public int MinimumCaptureIntervalSeconds { get; set; }
    public int MaximumCaptureIntervalSeconds { get; set; }
    public int HeartbeatIntervalSeconds { get; set; }
    public int DelayedAfterSeconds { get; set; }
    public int UnreachableAfterSeconds { get; set; }
    public int OfflineRetentionDays { get; set; }
    public long MaximumOfflineBytes { get; set; }
    public int ImageQuality { get; set; }
    public bool CaptureAllMonitors { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}

/// <summary>Represents one installed employee PC monitoring agent.</summary>
public sealed class EmployeeMonitoringAgent
{
    public long Id { get; set; }
    public Guid AgentInstanceId { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long PolicyId { get; set; }
    public string DeviceName { get; set; } = null!;
    public string CredentialHash { get; set; } = null!;
    public string? AgentVersion { get; set; }
    public DateTime? LastHeartbeatDateTime { get; set; }
    public DateTime? LastCaptureDateTime { get; set; }
    public DateTime? LastSuccessfulUploadDateTime { get; set; }
    public int PendingCaptureCount { get; set; }
    public string? LastErrorCode { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public EmployeeMonitoringPolicy Policy { get; set; } = null!;
}

/// <summary>Stores tenant-resolved metadata for one uploaded screenshot.</summary>
public sealed class EmployeeScreenCapture
{
    public long Id { get; set; }
    public Guid CaptureId { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long MonitoringAgentId { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public int MonitorNumber { get; set; }
    public string ContentType { get; set; } = null!;
    public long ContentLength { get; set; }
    public string ChecksumSha256 { get; set; } = null!;
    public string StorageObjectKey { get; set; } = null!;
    public EmployeeMonitoringAgent MonitoringAgent { get; set; } = null!;
}
