using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Department
{
    public class CreateDepartmentRequestDTO
    {


        public required string UserEmployeeId { get; set; }
        public string DepartmentName { get; set; } = null!;   
        public string? Description { get; set; }          
        public bool IsActive { get; set; } = true;  
        public string? Remark { get; set; }  

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

  
    }

}
