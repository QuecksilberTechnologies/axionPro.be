using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetDependentRequestDTO: BaseRequest
    {

       public string? EmployeeId { get; set; }
       
        public string? Relation { get; set; }      
        public bool? IsCoveredInPolicy { get; set; }
        public bool? IsMarried { get; set; }
        public bool? IsActive { get; set; }
        
    }


}
