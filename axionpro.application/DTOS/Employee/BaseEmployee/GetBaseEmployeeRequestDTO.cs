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
    public class GetBaseEmployeeRequestDTO:BaseRequest
    {
        /// <summary> TenantId Required</summary>       
        
        public string? EmployementCode { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; } 

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfOnBoarding { get; set; }

        public string? DesignationId { get; set; }  

        public bool? HasPermanent { get; set; }
        public string? TypeId { get; set; }
        public bool? IsEditAllowed { get; set; }  
        public bool? IsInfoVerified { get; set; } 



    }


}
