using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Employee.EnrolledPolicy
{
    public class DeleteEnrolledEmployeePolicyRequestDTO
    {
        public   string? EmployeeId { get; set; } = string.Empty;
        public   long? EmployeeInsuranceMappingId { get; set; } 
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
