using axionpro.application.DTOS.Common;
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
        public required string EmployeeId { get; set; }   
        public bool? IsActive { get; set; }


        public List<SectionStatusDTO>? Sections { get; set; }
       public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }

    public class SectionStatusDTO
    {
        public required int TabInfoType { get; set; } // "education", "bank", "experience"
        public bool? IsVerified { get; set; }
        public bool? IsEditAllowed { get; set; }

        // Optional: Primary key for that section (EducationId, BankId)
      //   public string? EmployeeId { get; set; }
    
    
    }

}
