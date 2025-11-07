using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    public class CreateReportingTypeRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }

        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Common audit fields
        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }


    }
}
