using axionpro.application.DTOs.Attendance;

namespace axionpro.application.Interfaces.IRepositories;

public interface IAttendanceRepository
{
    Task<AttendancePunchResponseDTO> MarkAsync(long tenantId, long employeeId,
        AttendanceRequestDTO request, DateTime serverUtcNow, CancellationToken cancellationToken);
    Task<AttendanceTodayResponseDTO> GetTodayAsync(long tenantId, long employeeId,
        DateTime serverUtcNow, CancellationToken cancellationToken);
}
