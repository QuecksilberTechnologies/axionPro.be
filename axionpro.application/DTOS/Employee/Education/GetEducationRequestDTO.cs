
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Education
{
    public class GetEducationRequestDTO :BaseRequest
    {    
       
        
        public string? EmployeeId { get; set; }= string.Empty;
        public string? Degree { get; set; }

        public string? InstituteName { get; set; }

        public string? Remark { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? GradeOrPercentage { get; set; } 
       
        public bool? EducationGap { get; set; }       
   
        public bool? IsInfoVerified { get; set; }     
        public bool HasEducationDocUploded { get; set; }     
        

        public bool? IsEditAllowed { get; set; } 

        public bool? IsActive { get; set; }
    }


}
