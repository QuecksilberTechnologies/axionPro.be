using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetEnrolledEmployeeRequestDTO
    {
        public required string EmployeeId { get; set; }  // 🔐 Encoded
    }
}
