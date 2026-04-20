using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.EmployeeManagerMappings
{
    public class AddEmployeeManagerMappingDTO
    {
        public long EmployeeId { get; set; }
        public long ManagerId { get; set; }

        public int ReportingTypeId { get; set; } // Primary / Secondary

        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public string? Description { get; set; }
        public string? Remark { get; set; }

        public ExtraPropRequestDTO Prop { get; set; } = new();
    }
}
