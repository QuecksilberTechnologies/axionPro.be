using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{

    /// <summary>
    /// post-request to fetch employee-info  
    /// </summary>
    public class GetBaseEmployeeRequestDTO: BaseRequest
    {
        /// <summary> TenantId Required</summary>

        
       
        public int? EmployeeDocumentId { get; set; }

        public string? EmployementCode { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; } 

        public DateOnly? DateOfBirth { get; set; }

        public DateTime? DateOfOnBoarding { get; set; }

        public int? DesignationId { get; set; }  

        public int? DepartmentId { get; set; }

        public string? OfficialEmail { get; set; }

        public bool? HasPermanent { get; set; }

        public int? TypeId { get; set; }
        public bool IsEditAllowed { get; set; }  
        public bool IsInfoVerified { get; set; } 



    }


}
