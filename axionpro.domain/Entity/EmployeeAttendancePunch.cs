namespace axionpro.domain.Entity;

public enum AttendanceChannel : short { Mobile = 1, Web = 2, Biometric = 3, Manual = 4 }
public enum AttendancePunchAction : short { CheckIn = 1, CheckOut = 2 }

/// <summary>Immutable attendance event recorded from an authorized attendance channel.</summary>
public sealed class EmployeeAttendancePunch
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long EmployeeId { get; set; }
    public long EmployeeWorkArrangementId { get; set; }
    public long PolicyVersionId { get; set; }
    public long? TenantLocationId { get; set; }
    public DateOnly WorkDate { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ClientOccurredAt { get; set; }
    public short Channel { get; set; }
    public short PunchAction { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? AccuracyMeters { get; set; }
    public decimal? DistanceFromLocationMeters { get; set; }
    public Guid IdempotencyKey { get; set; }
    public long AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
}
