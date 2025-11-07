using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
 
    public class GetEmployeeImageRequestDTO : BaseRequest
    {

        public string? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public int? ImageType { get; set; }
    }
}
