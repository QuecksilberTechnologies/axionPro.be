using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Attendance
{
    public class AttendanceRequestDTO
    {
        public long? Id { get; set; } // Primary Key
        public int? EmployeeId { get; set; } // Foreign Key to Employee
        public string? LoginId { get; set; } //  
        public DateTime AttendanceDate { get; set; } // Date of Attendance
        public int? AttendanceDeviceTypeId { get; set; } // Device Type (e.g., Mobile, Biometric)
        public int? WorkstationTypeId { get; set; } // Workstation Type
        public decimal? Latitude { get; set; } // Latitude for Geolocation
        public decimal? Longitude { get; set; } // Longitude for Geolocation
        public byte[]? ClickedImage { get; set; } // Path or URL of the Image

    }

}
