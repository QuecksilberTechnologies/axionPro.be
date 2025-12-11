using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Role
{
   
    public class GetRoleOptionRequestDTO
    {

        public required string UserEmployeeId { get; set; }
        public int? RoleType { get; set; } 
        public bool IsActive { get; set; } = true;
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
        

    }
}
