using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.category
{
    public class UpdateCategoryReqestDTO
    {

        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long TenantId { get; set; }
        public string? CategoryName { get; set; }
        public long  Id { get; set; } 

        public string? Remark { get; set; }

        public bool? IsActive { get; set; }
 
 

    }
}
