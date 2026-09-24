using System.ComponentModel.DataAnnotations;
using axionpro.domain.Entity;

namespace axionpro.application.DTOs.Attendance;

public sealed class AttendanceRequestDTO
{
    public AttendancePunchAction Action { get; set; }
    public AttendanceChannel Channel { get; set; }
    public long? TenantLocationId { get; set; }
    [Range(-90, 90)] public decimal? Latitude { get; set; }
    [Range(-180, 180)] public decimal? Longitude { get; set; }
    [Range(0, 10000)] public decimal? AccuracyMeters { get; set; }
    public DateTime? ClientOccurredAt { get; set; }
    public Guid IdempotencyKey { get; set; }
}

public sealed record AttendancePunchResponseDTO(long Id, AttendancePunchAction Action,
    AttendanceChannel Channel, DateOnly WorkDate, DateTime OccurredAtUtc,
    long? TenantLocationId, decimal? DistanceFromLocationMeters, bool IsCurrentlyCheckedIn);

public sealed record AttendancePunchItemDTO(long Id, AttendancePunchAction Action,
    AttendanceChannel Channel, DateTime OccurredAtUtc, long? TenantLocationId);

public sealed record AttendanceTodayResponseDTO(DateOnly WorkDate, bool IsCurrentlyCheckedIn,
    IReadOnlyList<AttendancePunchItemDTO> Punches);
