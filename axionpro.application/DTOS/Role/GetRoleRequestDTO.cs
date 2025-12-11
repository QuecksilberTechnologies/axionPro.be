using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class GetRoleRequestDTO:BaseRequest
    {      
        public int Id { get; set; }
        public int RoleType { get; set; }
        public bool IsActive{ get; set; } = true;  
        public string? RoleName { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }
}
