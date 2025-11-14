using axionpro.application.Common.Helpers.Converters;
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
    public class GetAllEmployeeInfoRequestDTO : BaseRequest
    {
        /// <summary> TenantId Required</summary>       

  
        public string? EmployementCode { get; set; }
        public string? LastName { get; set; } 
        public string? FirstName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfOnBoarding { get; set; }
        public string? GenderId { get; set; } // user se yeh string mei aayegi mujhe isko int mei change karna hai or aage forward karna hai parameter mei 
        public bool? IsMarried { get; set; }
        public string? DepartmentId { get; set; }
        public string? DesignationId { get; set; }
        public string? EmployeeTypeId { get; set; }
        public int? _EmployeeTypeId { get; set; }
        public int? _GenderId { get; set; }
        public int  _DesignationId { get; set; }
        public int? _DepartmentId { get; set; }
        public string? EmailId { get; set; }






    }


}
 
