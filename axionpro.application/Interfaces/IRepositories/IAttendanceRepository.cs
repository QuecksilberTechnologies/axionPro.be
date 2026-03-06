using axionpro.application.DTOs.Attendance;
using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IAttendanceRepository
    {
      public Task<UserAttendanceSetting> GetUserAttendanceSettingByIdAsync(AttendanceRequestDTO attendanceRequestDTO);
      public Task<bool> AddEmployeeAttendanceAsync(AttendanceRequestDTO attendanceRequestDTO);

    }
}
