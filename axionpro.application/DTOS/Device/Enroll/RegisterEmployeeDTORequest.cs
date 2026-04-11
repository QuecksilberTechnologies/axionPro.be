using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Device.Enroll
{
    public class RegisterEmployeeDTORequest
    {
        public long EmployeeId { get; set; }
        public long TenantId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string DeviceSn { get; set; } = null!;
        public string Name { get; set; }
        public int DeviceId { get; set; }
    }
    public class DeleteEmployeeDTORequest
    {
        public long EmployeeId { get; set; }
        public long TenantId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string DeviceSn { get; set; } = null!;
        public string Name { get; set; }
        public int DeviceId { get; set; }

    }
    
}
