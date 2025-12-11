using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Department
{
    public class GetDepartmentResponseDTO 
    {
               
        public int Id { get; set; } 
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public string? Remark { get; set; }
        


    }
        
}
