using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Type
{
    public class GetEmployeeTypeResponseDTO: BaseRequest
    {
       
        public int? EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public bool IsActive { get; set; }  
        public int GenderId { get ; set; }  
        public string? GenderType { get ; set; }
        public bool? IsMarried { get; set; }          
        public string? EmployeeName { get; set; }
        public string? OfficialEmail { get; set; }
        public string? HasPermanent { get; set; }
        public DateTime? DateOfExit  { get; set; }

    }
}
