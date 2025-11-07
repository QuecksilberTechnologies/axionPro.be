using axionpro.application.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee.AccessControlReadOnlyType
{
    public class EmployeePersonalEditableFieldDTO
    {
        [AccessControl(ReadOnly = true)]
        public long TenantId { get; set; }
        [AccessControl(ReadOnly = true)]
        public long EmployeeId { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? AadhaarNumber { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? PanNumber { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? PassportNumber { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? DrivingLicenseNumber { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? VoterId { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? BloodGroup { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? MaritalStatus { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? Nationality { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? EmergencyContactName { get; set; }
        [AccessControl(ReadOnly = false)]

        public string? EmergencyContactNumber { get; set; }

 
      
 
    }
}
