using axionpro.application.DTOS.Employee.EnrolledPolicy;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetAllEnrolledEmployeeResponseDTO
    {
        public string EmployeeId { get; set; } = string.Empty;

        public List<GetEmployeeEnrolledResponseDTO> Policies { get; set; } = new();
    }

    
}
