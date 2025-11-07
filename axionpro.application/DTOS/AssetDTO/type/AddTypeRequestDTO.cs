using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.type
{
    public class AddTypeRequestDTO
    {
        public long? TenantId { get; set; }
        public long? EmployeeId { get; set; }
        public int? RoleId { get; set; }
        public long? AssetCategoryId { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
