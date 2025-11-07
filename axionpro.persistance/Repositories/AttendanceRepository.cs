using axionpro.application.Constants;
using axionpro.application.DTOs.Attendance;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;
        public AttendanceRepository(WorkforceDbContext? context, ILogger<AttendanceRepository>? logger)
        {
            _context = context;
            _logger = logger;
        }     

      
            public  async Task<UserAttendanceSetting?> GetUserAttendanceSettingByIdAsync(AttendanceRequestDTO attendanceRequestDTO)
            {
                try
                {
                    // Validate context
                    if (_context == null)
                    {
                        _logger?.LogError("Database context is null.");
                        throw new InvalidOperationException("Database context is not initialized.");
                    }

                    // Log the incoming request details
                    _logger?.LogInformation("Fetching UserAttendanceSetting for EmployeeId: {EmployeeId}, DeviceTypeId: {DeviceTypeId}, WorkstationTypeId: {WorkstationTypeId}",
                        attendanceRequestDTO.EmployeeId, attendanceRequestDTO.AttendanceDeviceTypeId, attendanceRequestDTO.WorkstationTypeId);
             //   2  desktop or 1 office
                    // Fetch UserAttendanceSetting
                    var userAttendanceSetting = await _context.UserAttendanceSettings
                        .FirstOrDefaultAsync(uas => uas.EmployeeId == attendanceRequestDTO.EmployeeId
                            && uas.AttendanceDeviceTypeId == attendanceRequestDTO.AttendanceDeviceTypeId
                            && uas.WorkstationTypeId == attendanceRequestDTO.WorkstationTypeId
                            && uas.IsActive);

                    // If no record found, log and return null
                    if (userAttendanceSetting == null)
                    {
                        _logger?.LogWarning("No UserAttendanceSetting found for EmployeeId: {EmployeeId}, DeviceTypeId: {DeviceTypeId}, WorkstationTypeId: {WorkstationTypeId}.",
                            attendanceRequestDTO.EmployeeId, attendanceRequestDTO.AttendanceDeviceTypeId, attendanceRequestDTO.WorkstationTypeId);
                        return null;
                    }
           

                // Check EmployeeDailyAttendance for matching conditions
                //    var isAttendanceMarked = await _context.EmployeeDailyAttendances.AnyAsync(eda => eda.EmployeeId == attendanceRequestDTO.EmployeeId);

                // var marked =  await AddEmployeeAttendanceAsync(attendanceRequestDTO);



                //var isBreakCondition = await _context.EmployeeDailyAttendances.AnyAsync(eda => eda.EmployeeId == attendanceRequestDTO.EmployeeId
                //        && eda.WorkstationTypeId == 1
                //        && userAttendanceSetting.WorkstationTypeId == 1 && eda.IsMarked);


                return userAttendanceSetting;
                }
                catch (Exception ex)
                {
                    // Log exception details
                    _logger?.LogError(ex, "Error occurred while fetching UserAttendanceSetting.");
                    throw; // Re-throw the exception to be handled by the caller
                }
            }







        public async Task<bool> AddEmployeeAttendanceAsync(AttendanceRequestDTO attendanceRequestDTO)
        {
            try
            {
                // Validation logic...

                // Check if attendance already exists
                /*
                bool isAttendanceExists = await _context.EmployeeDailyAttendances.AnyAsync(eda =>
                    eda.EmployeeId == attendanceRequestDTO.EmployeeId.Value &&
                    eda.AttendanceDate.Date == attendanceRequestDTO.AttendanceDate.Date &&
                    eda.WorkstationTypeId == attendanceRequestDTO.WorkstationTypeId.Value);

                if (isAttendanceExists)
                {
                    _logger?.LogInformation("Attendance already exists for EmployeeId: {EmployeeId} on {AttendanceDate}.", attendanceRequestDTO.EmployeeId, attendanceRequestDTO.AttendanceDate.Date);
                    return false; // Prevent duplicate entry
                }
                */

                // Map DTO to Entity
                var newAttendance = new EmployeeDailyAttendance
                {
                    EmployeeId = attendanceRequestDTO.EmployeeId.Value,
                    AttendanceDate = attendanceRequestDTO.AttendanceDate,
                    AttendanceDeviceTypeId = attendanceRequestDTO.AttendanceDeviceTypeId.Value,
                    WorkstationTypeId = attendanceRequestDTO.WorkstationTypeId.Value,
                    Latitude = (double?)attendanceRequestDTO.Latitude,
                    Longitude = (double?)attendanceRequestDTO.Longitude,
                    ClickedImage = attendanceRequestDTO.ClickedImage,
                    IsLate = false, // Default
                    IsActive = true,
                    IsMarked = true,
                    AddedById = attendanceRequestDTO.EmployeeId.Value,
                    AddedDateTime = DateTime.UtcNow
                };

                // Save record
                await _context.EmployeeDailyAttendances.AddAsync(newAttendance);
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Attendance record added successfully for EmployeeId: {EmployeeId}.", attendanceRequestDTO.EmployeeId);
                return true; // Success
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while adding attendance for EmployeeId: {EmployeeId}.", attendanceRequestDTO.EmployeeId);
                throw; // Re-throw exception after logging
            }
        }




    }
}
                            