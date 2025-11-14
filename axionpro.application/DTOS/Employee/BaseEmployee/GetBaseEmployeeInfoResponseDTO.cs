using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetBaseEmployeeResponseDTO
    {
                  
             public string? Id { get; set; }
            public string? EmployementCode { get; set; } 
            public string? LastName { get; set; } = string.Empty;
            public string? MiddleName { get; set; }
            public string? FirstName { get; set; } = string.Empty;
            public string? GenderId { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public DateTime? DateOfOnBoarding { get; set; }
            public DateTime? DateOfExit { get; set; }
            public string? DesignationId { get; set; }
            public string? EmployeeTypeId { get; set; }
            public string? DepartmentId { get; set; }
            public string? OfficialEmail { get; set; }
            public bool? HasPermanent { get; set; } = false;
            public bool IsActive { get; set; } = true;         
            public bool? IsEditAllowed { get; set; } 
            public bool? IsInfoVerified { get; set; }
        
        }
    



}
 
