using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Designation
{
    public class GetDesignationOptionRequestDTO: BaseRequest
    {
       
        public required string DepartmentId { get; set; }       
        public bool IsActive { get; set; } = true;

        public string? DesignationName { get; set; }
    }
}
