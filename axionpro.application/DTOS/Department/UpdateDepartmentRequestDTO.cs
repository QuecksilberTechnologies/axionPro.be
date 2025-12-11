using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Department
{
    public class UpdateDepartmentRequestDTO 
    {
        public required int Id { get; set; }
        public string? UserEmployeeId { get; set; }         
        public string? DepartmentName { get; set; }  // अनिवार्य फ़ील्ड
        public string? Description { get; set; } // वैकल्पिक (nullable) फ़ील्ड
        public bool? IsActive { get; set; } 
        public string? Remark { get; set; } // वैकल्पिक (nullable) फ़ील्ड

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();



    }
}
