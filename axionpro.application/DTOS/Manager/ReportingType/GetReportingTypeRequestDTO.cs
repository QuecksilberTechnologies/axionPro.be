using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    public class GetReportingTypeRequestDTO: BaseRequest
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
      
        public bool? IsActive { get; set; }
        public string? TypeName { get; set; }
     
    }
}
