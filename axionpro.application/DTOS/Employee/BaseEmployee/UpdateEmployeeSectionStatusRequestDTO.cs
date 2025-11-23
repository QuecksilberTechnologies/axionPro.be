using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{

    
    public class UpdateEmployeeSectionStatusRequestDTO
    {
        public string? UserEmployeeId { get; set; }       

        public List<SectionStatusDTO>? Sections { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SectionStatusDTO
    {
        public string? SectionName { get; set; } // "education", "bank", "experience"
        public bool? IsVerified { get; set; }
        public bool? IsEditAllowed { get; set; }

        // Optional: Primary key for that section (EducationId, BankId)
         public string? EmployeeId { get; set; }
    
    
    }

}
